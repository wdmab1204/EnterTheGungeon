using GameEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.GunController
{
    class AutoRelease : MonoBehaviour
    {
        public event Action onDestroy;
        
        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }

    class ObjectPool<T> where T : UnityEngine.Component
    {
        private Queue<T> list = new();
        
        public event Action<T> onCreate;

        private T Create()
        {
            GameObject obj = new();
            var comp = obj.AddComponent<T>();
            onCreate?.Invoke(comp);
            return comp;
        }

        public T Get()
        {
            T comp;
            if (list.Count > 0)
            {
                comp = list.Dequeue();
                comp.gameObject.SetActive(true);
            }
            else
            {
                comp = Create();
            }
            
            return comp;
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            list.Enqueue(obj);
        }
    }
    
    public class BeamRenderer : MonoBehaviour
    {
        private List<AutoRelease> autoReleases = new();
        private List<Vector3> positions = new();
        private LineRenderer lineRenderer;

        public int pointsPerSegment = 20;

        private void Awake()
        {
            lineRenderer = GameUtility.CreateLineRenderer(Color.red, .2f);
            lineRenderer.transform.parent = this.transform;
        }

        private void Update()
        {
            positions.Clear();

            for (int i = 0; i < autoReleases.Count - 3; i++)
            {
                Vector3 p0 = autoReleases[i].transform.position;
                Vector3 p1 = autoReleases[i + 1].transform.position;
                Vector3 p2 = autoReleases[i + 2].transform.position;
                Vector3 p3 = autoReleases[i + 3].transform.position;

                for (int j = 0; j <= pointsPerSegment; j++)
                {
                    float t = j / (float)pointsPerSegment;
                    Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                    positions.Add(point);
                }
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                2f * p1 +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3
            );
        }

        public void Add(Transform transform)
        {
            var existing = transform.GetComponent<AutoRelease>();
            if (existing == null)
                existing = transform.gameObject.AddComponent<AutoRelease>();

            if (!autoReleases.Contains(existing))
            {
                existing.onDestroy += () => autoReleases.Remove(existing);
                autoReleases.Add(existing);
            }
        }
    }
}
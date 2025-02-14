using GameEngine;
using GameEngine.DataSequence.Graph;
using GameEngine.DataSequence.Shape;
using System.Collections.Generic;
using UnityEngine;


public class DTScene : MonoBehaviour
{
    private DelaunayTriangulation graph = new();
    private List<Vector3> points = new();
    private GameObject rootObject => this.gameObject;

    private Dictionary<Triangle, LineRenderer> triSet = new();

    private void Awake()
    {
        graph.onCreated += OnCreate;
        graph.onDestroyed += OnDest;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;
            points.Add(worldPosition);

            graph.Clear();

            foreach (var pos in points)
            {
                graph.AddVertex(pos);
                Debug.Log(pos);
            }

            graph.Process();
        }
    }

    private void OnCreate(Triangle tri)
    {
        var triangleRenderer = new GameObject("Triangle Renderer").AddComponent<LineRenderer>();
        triangleRenderer.transform.parent = rootObject.transform;
        triangleRenderer.positionCount = 4;
        triangleRenderer.SetPosition(0, tri.a);
        triangleRenderer.SetPosition(1, tri.b);
        triangleRenderer.SetPosition(2, tri.c);
        triangleRenderer.SetPosition(3, tri.a);

        Color randColor = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        triangleRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        triangleRenderer.material.SetColor("_Color", randColor);
        
        triangleRenderer.name = tri.ID.ToString();
        triangleRenderer.transform.position = tri.GetCenter();
        triSet[tri] = triangleRenderer;

        //var circleRenderer = new GameObject("Circle Renderer").AddComponent<LineRenderer>();
        //circleRenderer.transform.parent = triangleRenderer.transform;
        //const int cnt = 50;
        //circleRenderer.positionCount = cnt + 2;
        //var list = tri.GetCircumCircle().GetPoints(cnt);
        //for(int i=0; i<cnt; i += 2)
        //{
        //    circleRenderer.SetPosition(i, list[i]);
        //    circleRenderer.SetPosition(i + 1, list[i + 1]);
        //}

        //circleRenderer.SetPosition(cnt, list.Last());
        //circleRenderer.SetPosition(cnt + 1, list.First());
    }

    private void OnDest(Triangle tri)
    {
        if (triSet.ContainsKey(tri) == false)
            return;

        GameUtil.Destroy(triSet[tri].material);
        GameUtil.Destroy(triSet[tri].gameObject);
        triSet.Remove(tri);
    }
}

using UnityEngine;

namespace GameEngine.Character
{
    public class MouseTracker : MonoBehaviour
    {
        private Camera mainCamera;
        private Transform ownerTransform;

        [SerializeField] private Transform gunTransform;


        private void Awake()
        {
            mainCamera = Camera.main;
            ownerTransform = transform;
        }

        private void Update()
        {
            Vector3? mouseWorldPos = GameUtility.GetMouseWoirldPosition(mainCamera, Input.mousePosition);
            if (mouseWorldPos == null)
                return;

            Vector2 direction = (mouseWorldPos.Value - ownerTransform.position);

            bool flipX = direction.x < 0f;

            var localScale = ownerTransform.localScale;
            localScale.x = flipX ? -1 : 1;
            ownerTransform.localScale = localScale;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (flipX)
                angle -= 180f;

            gunTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
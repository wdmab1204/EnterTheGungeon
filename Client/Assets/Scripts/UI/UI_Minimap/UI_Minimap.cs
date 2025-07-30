using GameEngine.DataSequence.Graph;
using GameEngine.UI.Minimap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.UI
{
    public class UI_Minimap : MonoBehaviour
    {
        [SerializeField] private MinimapScreen minimapScreen;
        [SerializeField] private MinimapOveray minimapOveray;

        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private RoomRenderer roomRenderer;
        [SerializeField] private RoadRenderer roadRenderer;
        [SerializeField] private Transform roomLayer;
        [SerializeField] private Transform roadLayer;
        [SerializeField] private Transform minimapObject;

        private Vector3 initPlayerPosition;
        private Func<Vector3> getPlayerPosition;
        private IMinimapDisplay currentMinimapDisplay;

        public enum Mode
        {
            Overay,
            Screen
        };

        private void Start()
        {
            SetMode(Mode.Overay);
        }

        public void SetMode(Mode mode)
        {
            if(mode == Mode.Screen)
            {
                minimapOveray.gameObject.SetActive(false);
                minimapScreen.gameObject.SetActive(true);
                minimapScreen.SetGameObject(minimapObject.gameObject);
                minimapScreen.OnMovePlayer(getPlayerPosition() - initPlayerPosition);
                currentMinimapDisplay = minimapScreen;
            }
            else if(mode == Mode.Overay)
            {
                minimapOveray.gameObject.SetActive(true);
                minimapScreen.gameObject.SetActive(false);
                minimapOveray.SetGameObject(minimapObject.gameObject);
                minimapOveray.OnMovePlayer(getPlayerPosition() - initPlayerPosition);
                currentMinimapDisplay = minimapOveray;
            }
        }

        public void Render(
            IEnumerable<RoomNode> rooms,
            IEnumerable<RoomEdge> roads,
            Func<Vector3> getPlayerPosition,
            int gridCellSize)
        {
            var playerPos = getPlayerPosition();
            this.getPlayerPosition = getPlayerPosition;
            initPlayerPosition = playerPos;
            Vector3 centerOffset = playerPos;

            RenderRooms(rooms, centerOffset);
            RenderRoads(roads, gridCellSize, centerOffset);

            playerIcon.anchoredPosition = Vector2.zero;
        }

        public void OnMovePlayer(Vector3 playerPosition)
        {
            currentMinimapDisplay?.OnMovePlayer(playerPosition - initPlayerPosition);
        }

        private void RenderRooms(IEnumerable<RoomNode> rooms, Vector3 centerOffset)
        {
            foreach (var room in rooms)
            {
                var roomRenderer = Instantiate(this.roomRenderer, roomLayer);
                roomRenderer.name = $"Room {room.ID}";
                roomRenderer.gameObject.SetActive(true);

                var rectTransform = roomRenderer.GetComponent<RectTransform>();
                rectTransform.localPosition = room.ToVector3() - centerOffset;

                var cellPositions = new HashSet<Vector3Int>(room.GetTilemaps().SelectMany(tilemap => GameUtil.AllGetTilePosition(tilemap)));
                roomRenderer.cellPositions = cellPositions.ToArray();

                var outlines = GameUtil.CellsToOutline(cellPositions);
                foreach(var outline in outlines)
                {
                    outline.Add(outline[1]);

                    var lineRenderer = new GameObject("UI Line Renderer").AddComponent<UILineRenderer>();
                    lineRenderer.transform.SetParent(roomRenderer.transform, false);
                    lineRenderer.color = Color.black;
                    lineRenderer.thickness = 2;
                    lineRenderer.raycastTarget = false;
                    lineRenderer.points = outline.Select(p => new Vector2(p.x, p.y)).ToArray();
                }
            }
        }

        private void RenderRoads(IEnumerable<RoomEdge> roads, int gridCellSize, Vector3 centerOffset)
        {
            foreach (var road in roads)
            {
                var roadRenderer = Instantiate(this.roadRenderer, roadLayer);
                roadRenderer.gameObject.SetActive(true);

                var worldPoints = road.PathResult.Select(p => p.ToVector3()).ToList();
                var min = new Vector3(worldPoints.Min(p => p.x), worldPoints.Min(p => p.y));

                var localPoints = worldPoints
                    .Select(p => new Vector2(p.x - min.x + gridCellSize / 2f, p.y - min.y + gridCellSize / 2f))
                    .ToList();

                roadRenderer.rectTransform.anchoredPosition = min - centerOffset;
                roadRenderer.points = localPoints.ToArray();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetMode(Mode.Screen);
            }

            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetMode(Mode.Overay);
            }
        }
    }
}

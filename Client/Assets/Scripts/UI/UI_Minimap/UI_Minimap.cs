using GameEngine.DataSequence.Graph;
using GameEngine.UI.Minimap;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        private static readonly Vector3Int[] clockWiseDirections = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

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

                var cellPositions = AllGetTilePosition(room);
                roomRenderer.cellPositions = cellPositions.ToArray();

                var outlines = GetAllOutlinePoints(cellPositions);
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

        private HashSet<Vector3Int> AllGetTilePosition(RoomNode room)
        {
            HashSet<Vector3Int> allPoints = new();

            foreach (var tilemap in room.GetTilemaps())
            {
                foreach (var p in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(p))
                        allPoints.Add(p);
                }
            }

            return allPoints;
        }

        private List<List<Vector3Int>> GetAllOutlinePoints(HashSet<Vector3Int> pointSet)
        {
            HashSet<(Vector3Int, Vector3Int)> lineSet = new();
            Dictionary<Vector3Int, Vector3Int> lineConnectMap = new();

            // 1. 외곽선을 구성하는 선분 찾기
            foreach (var cellPosition in pointSet)
            {
                Vector3Int to = cellPosition, from;
                for (int i = 0; i < 4; i++)
                {
                    from = to;
                    to = from + clockWiseDirections[i];

                    if (lineSet.Remove((from, to)) || lineSet.Remove((to, from)))
                        continue;

                    lineSet.Add((from, to));
                }
            }

            // 2. 연결 정보 구성
            foreach (var line in lineSet)
            {
                lineConnectMap[line.Item1] = line.Item2;
            }

            // 3. 외곽선 여러 개 추적
            List<List<Vector3Int>> outlines = new();
            HashSet<Vector3Int> visited = new();

            foreach (var start in lineConnectMap.Keys.ToList())
            {
                if (visited.Contains(start))
                    continue;

                List<Vector3Int> line = new();
                Vector3Int current = start;

                do
                {
                    visited.Add(current);
                    line.Add(current);

                    if (!lineConnectMap.TryGetValue(current, out var next))
                        break;

                    current = next;
                }
                while (!visited.Contains(current));

                if (line.Count > 1)
                    outlines.Add(line);
            }

            return outlines;
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

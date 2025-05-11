using GameEngine.DataSequence.Graph;
using GameEngine.UI.Minimap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.UI
{
    public class UI_Minimap : MonoBehaviour
    {
        [SerializeField] private Sprite roomSprite;
        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private RoomRenderer roomRenderer;
        [SerializeField] private RoadRenderer roadRenderer;
        [SerializeField] private Transform zoomGroup;
        [SerializeField] private Transform roomLayer;
        [SerializeField] private Transform roadLayer;

        private float zoomValue = 4f;
        private float minZoomValue, maxZoomValue;
        private Vector3 initPlayerPosition;

        private static readonly Vector3Int[] clockWiseDirections = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        public void SetData(
            IEnumerable<RoomNode>   rooms,
            IEnumerable<RoomEdge>   roads,
            Vector3                 playerPosition,
            int                     gridCellSize)
        {
            initPlayerPosition = playerPosition;
            Vector3 centerOffset = playerPosition;

            RenderRooms(rooms, centerOffset);
            RenderRoads(roads, gridCellSize, centerOffset);

            playerIcon.anchoredPosition = Vector2.zero;

            minZoomValue = zoomValue / 3f;
            maxZoomValue = zoomValue * 3;
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

                var outline = GetOutlinePoints(cellPositions);
                outline.Add(outline[1]);
                roomRenderer.GetComponentInChildren<UILineRenderer>().points = outline.Select(p => new Vector2(p.x, p.y)).ToArray();
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

        public void OnMovePlayer(Vector3 playerPosition)
        {
            Vector2 playerIconAnchorPosition = playerPosition - initPlayerPosition;
            playerIcon.anchoredPosition = playerIconAnchorPosition;
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
        
        private List<Vector3Int> GetOutlinePoints(HashSet<Vector3Int> pointSet)
        {
            HashSet<(Vector3Int, Vector3Int)>   lineSet = new();
            List<Vector3Int> lineList = new();

            foreach (var cellPosition in pointSet)
            {
                Vector3Int to = cellPosition, from;
                for (int i = 0; i < 4; i++)
                {
                    from = to;
                    to = from + clockWiseDirections[i];

                    if (lineSet.Remove((from, to)))
                        continue;
                    if (lineSet.Remove((to, from)))
                        continue;

                    lineSet.Add((from, to));
                }
            }

            var lineConnectMap = lineSet.ToDictionary(line => line.Item1, line => line.Item2);

            Vector3Int current = lineConnectMap.First().Key;
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            lineList.Add(current);

            while (lineConnectMap.TryGetValue(current, out var next))
            {
                if (visited.Contains(current))
                    break;

                visited.Add(current);
                lineList.Add(next);
                current = next;
            }

            return lineList;
        }

        private void Update()
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

            zoomValue += scrollDelta * 0.5f;
            zoomValue = Mathf.Clamp(zoomValue + scrollDelta * 0.5f, minZoomValue, maxZoomValue);

            zoomGroup.localScale = Vector3.one * zoomValue;
        }
    }
}

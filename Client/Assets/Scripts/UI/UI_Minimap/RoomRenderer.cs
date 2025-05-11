using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI.Minimap
{
    public class RoomRenderer : Graphic
    {
        public Vector3Int[] cellPositions;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            for(int i=0; i<cellPositions.Length; i++)
            {
                var cell = cellPositions[i];
                UIVertex uiVertex = UIVertex.simpleVert;
                uiVertex.color = color;
                uiVertex.position = new Vector3(cell.x, cell.y);            //index : 0
                vh.AddVert(uiVertex);
                uiVertex.position = new Vector3(cell.x + 1, cell.y);        //index : 1
                vh.AddVert(uiVertex);
                uiVertex.position = new Vector3(cell.x, cell.y + 1);        //index : 2
                vh.AddVert(uiVertex);
                uiVertex.position = new Vector3(cell.x + 1, cell.y + 1);    //index : 3
                vh.AddVert(uiVertex);

                int index = i * 4;
                vh.AddTriangle(index, index + 2, index + 3); // (0,2,3)
                vh.AddTriangle(index, index + 3, index + 1); // (0,3,1)
            }
        }
    }
}
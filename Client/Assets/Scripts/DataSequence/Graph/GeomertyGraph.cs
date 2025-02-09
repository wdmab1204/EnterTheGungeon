using GameEngine.DataSequence.Shape;
using UnityEngine;

namespace GameEngine.DataSequence.Graph
{
    public class GeomertyGraph<TNode> : Graph<TNode, WeightEdge> where TNode : IGeomertyNode
    {
        public void AutoCreateEdges()
        {
            DelaunayTriangulation();
        }

        private void DelaunayTriangulation()
        {
            Triangle superTriangle = GetSuperTriangle();
            

            Triangle GetSuperTriangle()
            {
                Vector3 dot1, dot2, dot3;
                dot1 = dot2 = dot3 = default;
                return new() { a = dot1, b = dot2, c = dot3 };
            }
        }
    }
}
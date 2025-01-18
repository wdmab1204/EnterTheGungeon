using System;
using System.Drawing;
using DataSequence.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameEngine.MapGenerator
{
    public class MapGenerator
    {
        private int maxDepth = 3; //방의 개수 : 2^ maxNode
        private int minDivideSize = 2;
        private int maxDivideSize = 10;
        private Vector2Int mapSize;
        private MapRenderer mapRenderer;

        public void Init(Vector2Int mapSize, MapRenderer mapRenderer)
        {
            this.mapSize = mapSize;
            this.mapRenderer = mapRenderer;
            mapRenderer.SetMapSize(mapSize);
        }
        
        public void DivideTree(TreeNode treeNode, int depth) //재귀 함수
        {
            if (depth < maxDepth) //0 부터 시작해서 노드의 최댓값에 이를 때 까지 반복
            {
                RectInt size = treeNode.RoomSize; //이전 트리의 범위 값 저장, 사각형의 범위를 담기 위해 Rect 사용
                int length = size.width >= size.height ? size.width : size.height; //사각형의 가로와 세로 중 길이가 긴 축을, 트리를 반으로 나누는 기준선으로 사용
                int split = Random.Range(7, Math.Max(7, length - 7)); //기준선 위에서 최소 범위와 최대 범위 사이의 값을 무작위로 선택
                Debug.Log(treeNode.RoomSize);
                if (size.width >= size.height) //가로
                {
                    treeNode.LeftNode = new TreeNode(size.x, size.y, split, size.height); //기준선을 반으로 나눈 값인 split을 가로 길이로, 이전 트리의 height값을 세로 길이로 사용
                    treeNode.RightNode = new TreeNode(size.x + split, size.y, size.width - split, size.height); //x값에 split값을 더해 좌표 설정, 이전 트리의 width값에 split값을 빼 가로 길이 설정
                    mapRenderer.DrawLine(new Vector2(size.x + split, size.y), new Vector2(size.x + split, size.y + size.height)); //기준선 렌더링
                }
                else //세로
                {
                    treeNode.LeftNode = new TreeNode(size.x, size.y, size.width, split);
                    treeNode.RightNode = new TreeNode(size.x, size.y + split, size.width, size.height - split);
                    mapRenderer.DrawLine(new Vector2(size.x, size.y + split), new Vector2(size.x + size.width, size.y + split));
                }
                treeNode.LeftNode.ParentNode = treeNode; //분할한 트리의 부모 트리를 매개변수로 받은 트리로 할당
                treeNode.RightNode.ParentNode = treeNode;

                // mapRenderer.DrawDungeon(
                //     treeNode.LeftNode.RoomSize.x,
                //     treeNode.LeftNode.RoomSize.y,
                //     treeNode.LeftNode.RoomSize.width,
                //     treeNode.LeftNode.RoomSize.height);
                //
                // mapRenderer.DrawDungeon(
                //     treeNode.RightNode.RoomSize.x,
                //     treeNode.RightNode.RoomSize.y,
                //     treeNode.RightNode.RoomSize.width,
                //     treeNode.RightNode.RoomSize.height);
                
                DivideTree(treeNode.LeftNode, depth + 1); //재귀 함수, 자식 트리를 매개변수로 넘기고 노드 값 1 증가 시킴
                DivideTree(treeNode.RightNode, depth + 1);
            }
        }

        public RectInt GenerateDungeon(TreeNode treeNode, int depth) //방 생성
        {
            if (depth == maxDepth) //노드가 최하위일 때만 조건문 실행
            {
                RectInt size = treeNode.RoomSize;
                int width = Mathf.Max(Random.Range((int)(size.width / 1.3f), size.width - 1)); //트리 범위 내에서 무작위 크기 선택, 최소 크기 : width / 2
                int height = Mathf.Max(Random.Range((int)(size.height / 1.3f), size.height - 1));
                int x = treeNode.RoomSize.x + Random.Range(1, size.width - width); //최대 크기 : width / 2
                int y = treeNode.RoomSize.y + Random.Range(1, size.height - height);
                mapRenderer.DrawDungeon(x, y, width, height); //던전 렌더링
                return new RectInt(x, y, width, height); //리턴 값은 던전의 크기로 길을 생성할 때 크기 정보로 활용
            }
            treeNode.LeftNode.DungeonSize = GenerateDungeon(treeNode.LeftNode, depth + 1); //리턴 값 = 던전 크기
            treeNode.RightNode.DungeonSize = GenerateDungeon(treeNode.RightNode, depth + 1);
            return treeNode.LeftNode.DungeonSize; //부모 트리의 던전 크기는 자식 트리의 던전 크기 그대로 사용
        }
        
        public void GenerateRoad(TreeNode treeNode, int n) //길 연결
        {
            if (n == maxDepth) return; 
            var leftNodeCenter = treeNode.LeftNode.DungeonSize.GetCenterInt(); 
            var rightNodeCenter = treeNode.RightNode.DungeonSize.GetCenterInt(); 
            mapRenderer.DrawRoad(leftNodeCenter, rightNodeCenter);
            GenerateRoad(treeNode.LeftNode, n + 1);
            GenerateRoad(treeNode.RightNode, n + 1);
        }
    }
}
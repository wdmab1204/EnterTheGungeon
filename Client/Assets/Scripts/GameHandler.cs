using GameEngine.DataSequence.Graph;
using GameEngine.Item;
using GameEngine.Navigation;
using GameEngine.Pipeline;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
 
namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        [SerializeField] private UI_Minimap minimapUI;

        private DungeonGeneratorLevel dungeonGeneratorLevel;
        private CharacterController PlayerController
        {
            get => GameData.Player;
            set => GameData.Player = value;
        }
        private RoomNode CurrentVisitRoom
        {
            get => GameData.CurrentVisitRoom.Value;
            set => GameData.CurrentVisitRoom.Value = value;
        }

        private new FollowPlayer camera;
        private HashSet<RoomNode> visitedRooms = new();
        private int currentMobCount;
        private NavGrid navGrid;
        private PathFinding pathFinder;
        private HashSet<Coin> fieldCoins = new();   

        private void Awake()
        {
            InitializeComponents();
            InitializePlayer();
            InitializeMinimap();
        }

        private void InitializeComponents()
        {
            var generatorBase = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = generatorBase.Generate(GameData.Seed);

            PlayerController = GameObject.Find("Mine").GetComponent<CharacterController>();
            PlayerController.onMove += OnUserMove;

            camera = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();

            navGrid = GetComponentInChildren<NavGrid>();
            var gameGrid = dungeonGeneratorLevel.GameGrid;
            var floorTilemap = GameUtility.FindChild<UnityEngine.Tilemaps.Tilemap>(gameGrid.gameObject, "Floor");
            var collideableTilemap = GameUtility.FindChild<UnityEngine.Tilemaps.Tilemap>(gameGrid.gameObject, "Collideable");
            navGrid.CreateGrid(floorTilemap, collideableTilemap);
            navGrid.transform.position = gameGrid.transform.position;

            pathFinder = new(navGrid);
            PathFindManager.PathFinder = pathFinder;
        }

        private void InitializePlayer()
        {
            SetPlayerPosition(PlayerController.transform);
            camera.transform.position = PlayerController.transform.position;
            camera.AddTransform(PlayerController.transform);
            OnUserMove(PlayerController.transform.position);
        }

        private void InitializeMinimap()
        {
            minimapUI.Render(
                dungeonGeneratorLevel.Rooms,
                dungeonGeneratorLevel.RoadEdges,
                () => PlayerController.transform.position,
                dungeonGeneratorLevel.GridCellSize
            );
            PlayerController.onMove += minimapUI.OnMovePlayer;
        }

        private bool IsInRoom(RoomNode room, Vector3 position, int padding)
        {
            var pos = room.ToVector3() + new Vector3(padding, padding);
            var size = room.GetSize() - new Vector3(padding * 2, padding * 2);
            return MathUtility.IsPointInRectangle(position, pos, size);
        }

        private void OnUserMove(Vector3 position)
        {
            var layout = dungeonGeneratorLevel.LayoutData;

            RoomNode newRoom = dungeonGeneratorLevel.Rooms.FirstOrDefault(room =>
                !visitedRooms.Contains(room) && IsInRoom(room, position, 1));

            if (newRoom != null)
            {
                EnterRoom(newRoom, layout);
                return;
            }

            if (CurrentVisitRoom != null && !IsInRoom(CurrentVisitRoom, position, 1))
            {
                CurrentVisitRoom = null;
            }
        }

        private void EnterRoom(RoomNode room, Dictionary<RoomNode, RoomInstance> layout)
        {
            Debug.Log($"Current Visit Room : {room.ID}");
            visitedRooms.Add(room);
            CurrentVisitRoom = room;

            if (room.HasMob)
            {
                foreach (var door in layout[room].Doors)
                    door.SetActive(true);
            }

            currentMobCount = layout[room].Mobs.Count;
            foreach (var mob in layout[room].Mobs)
            {
                camera.AddTransform(mob.transform);
                mob.SetActive(true);
                mob.GetComponent<MonobehaviourExtension>().DestroyState.OnValueChanged += isDestroyed =>
                {
                    if (!isDestroyed) return;

                    currentMobCount--;
                    OnMobCountChanged(currentMobCount);

                    DropCoin(mob.transform.position, Random.Range(1, 3));
                };
            }
        }

        private void DropCoin(Vector3 mobWorldPosition, int count)
        {
            var coinPrefab = Resources.Load<Coin>("Coin");

            while (count-- > 0)
            {
                var coinObject = UnityEngine.Object.Instantiate<Coin>(coinPrefab);
                coinObject.PathRequest += GetPath;
                coinObject.transform.position = mobWorldPosition;
                coinObject.GetComponent<MonobehaviourExtension>().DestroyState.OnValueChanged += isDestroy =>
                {
                    if (isDestroy == false) return;
                    fieldCoins.Remove(coinObject);
                };
                fieldCoins.Add(coinObject);

                if(currentMobCount <= 0)
                {
                    coinObject.FollowTarget = PlayerController.transform;
                }
            }
        }

        private void OnMobCountChanged(int mobCount)
        {
            if (mobCount <= 0 && CurrentVisitRoom != null)
            {
                OnAllMobsCleared();
            }
        }

        private void OnAllMobsCleared()
        {
            foreach (var door in dungeonGeneratorLevel.LayoutData[CurrentVisitRoom].Doors)
                door.SetActive(false);

            foreach(var coin in fieldCoins)
            {
                if (coin == null || coin.gameObject.IsDestroyed())
                    return;

                coin.FollowTarget = PlayerController.transform;
            }
        }

        private void SetPlayerPosition(Transform playerTrans)
        {
            RoomNode startRoom = dungeonGeneratorLevel.Rooms.FirstOrDefault(room => room.ID == 1000);
            Vector3 worldPosition = startRoom.GetCenter();
            playerTrans.position = worldPosition;
            visitedRooms.Add(startRoom);
        }

        public PathResult GetPath(Vector3 start, Vector3 end)
        {
            //GameUtil.CreateLineRenderer(Color.yellow, .2f, new Vector3[] { new(start.x, start.y), new(end.x, end.y) });
            var result = pathFinder.FindPath(start, end);
            if (result.success == false)
            {
                UnityEngine.Debug.LogError($"Can not found destination. src :  {start}, dst : {end}");
            }
            return result;
        }
    }
}
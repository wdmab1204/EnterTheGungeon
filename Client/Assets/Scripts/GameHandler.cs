using GameEngine.DataSequence.DIContainer;
using GameEngine.DataSequence.EventBus;
using GameEngine.DataSequence.Graph;
using GameEngine.GunController;
using GameEngine.Item;
using GameEngine.Navigation;
using GameEngine.Pipeline;
using GameEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = GameEngine.DataSequence.EventBus.EventBus;

namespace GameEngine
{
    public class GameHandler : MonoBehaviour
    {
        [SerializeField] private UI_Minimap minimapUI;

        private DungeonGeneratorLevel dungeonGeneratorLevel;
        private ICharacterController PlayerController => DIContainer.Resolve<ICharacterController>();
        private RoomNode CurrentVisitRoom
        {
            get => GameData.CurrentVisitRoom.Value;
            set => GameData.CurrentVisitRoom.Value = value;
        }

        private new FollowPlayer camera;
        private HashSet<RoomNode> visitedRooms = new();
        private int currentMobCount;
        private DungeonNavigation pathFinder;
        private HashSet<Coin> fieldCoins = new();

        private void Awake()
        {
            var generatorBase = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorBase>();
            dungeonGeneratorLevel = generatorBase.Generate();

            InitializeDIContainer();
            InitializeComponents();
            InitializePlayer();
            InitializeMinimap();
        }

        private void InitializeDIContainer()
        {
            var playerController = GameObject.Find("Mine").GetComponent<CharacterController>();
            DIContainer.RegisterInstance<ICharacterController>(playerController);
            DIContainer.RegisterInstance<IUnitAbility>(playerController.GetComponent<UnitAbility>());
            DIContainer.RegisterInstance<IGunController>(playerController.GetComponent<GunController.GunController>());

            var navGrid = GetComponentInChildren<NavGrid>();
            var gameGrid = dungeonGeneratorLevel.GameGrid;
            var floorTilemap = GameUtility.FindChild<UnityEngine.Tilemaps.Tilemap>(gameGrid.gameObject, "Floor");
            navGrid.CreateGrid(floorTilemap, gameGrid.transform.position);
            DIContainer.RegisterInstance<IPathFinder>(new DungeonNavigation(navGrid));

            EventBus.Subscribe<Vector3>("PlayerMove", OnUserMove);
        }

        private void InitializeComponents()
        {
            //PlayerController.OnMove += OnUserMove;

            camera = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
        }

        private void InitializePlayer()
        {
            SetPlayerPosition(PlayerController.Transform);
            camera.transform.position = PlayerController.Transform.position;
            camera.AddTransform(PlayerController.Transform);
            OnUserMove(PlayerController.Transform.position);
        }

        private void InitializeMinimap()
        {
            minimapUI.Render(
                dungeonGeneratorLevel.Rooms,
                dungeonGeneratorLevel.RoadEdges,
                () => PlayerController.Transform.position,
                dungeonGeneratorLevel.GridCellSize
            );
            //PlayerController.OnMove += minimapUI.OnMovePlayer;
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
                coinObject.transform.position = mobWorldPosition;
                coinObject.GetComponent<MonobehaviourExtension>().DestroyState.OnValueChanged += isDestroy =>
                {
                    if (isDestroy == false) return;
                    fieldCoins.Remove(coinObject);
                };
                fieldCoins.Add(coinObject);

                if(currentMobCount <= 0)
                {
                    coinObject.FollowTarget = PlayerController.Transform;
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

                coin.FollowTarget = PlayerController.Transform;
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
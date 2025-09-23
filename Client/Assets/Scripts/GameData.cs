using GameEngine.DataSequence.Graph;
using GameEngine.GunController;
using GameEngine.Observable;

namespace GameEngine
{
    public static class GameData
    {
        public static ReactiveProperty<int> AmmoCount = new();
        public static ReactiveProperty<int> AmmoSize = new();
        public static ReactiveProperty<RoomNode> CurrentVisitRoom = new();
        public static ReactiveProperty<int> Coin = new();

        public static GunData EquipGunData { get; set; }

        public static int Seed { get; set; }

        
    }
}
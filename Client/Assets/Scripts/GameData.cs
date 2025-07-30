using GameEngine.DataSequence.Graph;
using GameEngine.GunController;
using GameEngine.Observable;

namespace GameEngine
{
    public static class GameData
    {
        public static ReactiveProperty<int> AmmoCount = new();
        public static ReactiveProperty<int> AmmoSize = new();
        public static GunData EquipGunData = new();

        public static ReactiveProperty<RoomNode> CurrentVisitRoom = new();
    }
}
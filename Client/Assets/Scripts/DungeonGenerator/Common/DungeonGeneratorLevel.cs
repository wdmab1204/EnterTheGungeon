using GameEngine.DataSequence.Graph;
using System.Collections.Generic;

namespace GameEngine
{
    public class DungeonGeneratorLevel
    {
        private IEnumerable<RoomNode> rooms;

        public DungeonGeneratorLevel(IEnumerable<RoomNode> rooms)
        {
            this.rooms = rooms;
        }

        public IEnumerable<RoomNode> GetRoomEnumerable() => rooms;
    }
}
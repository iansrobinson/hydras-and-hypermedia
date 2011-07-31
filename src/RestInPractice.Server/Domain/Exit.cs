namespace RestInPractice.Server.Domain
{
    public class Exit
    {
        public static Exit North(int roomId)
        {
            return new Exit(Direction.North, roomId);
        }

        public static Exit South(int roomId)
        {
            return new Exit(Direction.South, roomId);
        }

        public static Exit East(int roomId)
        {
            return new Exit(Direction.East, roomId);
        }

        public static Exit West(int roomId)
        {
            return new Exit(Direction.West, roomId);
        }

        private readonly Direction direction;
        private readonly int roomId;

        private Exit(Direction direction, int roomId)
        {
            this.direction = direction;
            this.roomId = roomId;
        }

        public Direction Direction
        {
            get { return direction; }
        }

        public int RoomId
        {
            get { return roomId; }
        }
    }
}
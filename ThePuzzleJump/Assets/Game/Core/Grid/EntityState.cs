using UnityEngine;
using Game.Core.Enums;

namespace Game.Core.Grid
{
    public abstract class EntityState
    {
        public Vector2Int Position { get; protected set; }
        public Direction Direction { get; protected set; }

        protected EntityState(Vector2Int position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }
        public void Rotate(bool Clockwise)
        {
            int multiplicator = Clockwise ? 1 : 3;
            Direction =(Direction)(((int)Direction + multiplicator) % 4);
        }

        public void MoveTo(Vector2Int newPosition)
        {
            Position = newPosition;
        }
    }
}

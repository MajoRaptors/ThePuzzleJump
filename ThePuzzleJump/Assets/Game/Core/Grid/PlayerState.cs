using UnityEngine;
using Game.Core.Enums;

namespace Game.Core.Grid
{
    public class PlayerState : EntityState
    {
        public PlayerState(Vector2Int position, Direction direction)
            : base(position, direction)
        {
        }
    }
}

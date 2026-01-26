using UnityEngine;
using Game.Core.Enums;

namespace Game.Core.Grid
{
    public class EnemyState : EntityState
    {
        public EnemyType Type { get; private set; }

        public EnemyState(
            Vector2Int position,
            Direction direction,
            EnemyType type
        ) : base(position, direction)
        {
            Type = type;
        }
    }
}

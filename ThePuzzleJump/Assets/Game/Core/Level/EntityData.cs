using System;
using Game.Core.Enums;

namespace Game.Core.Level
{
    /// <summary>
    /// Représente une entité logique (joueur ou ennemi).
    /// </summary>
    [Serializable]
    public class EntityData
    {
        #region Fields

        public int x;
        public int y;

        public Direction direction;

        public EnemyType enemyType;

        #endregion

        #region Constructors

        public EntityData(int x, int y, Direction direction, EnemyType enemyType)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.enemyType = enemyType;
        }

        #endregion
    }
}

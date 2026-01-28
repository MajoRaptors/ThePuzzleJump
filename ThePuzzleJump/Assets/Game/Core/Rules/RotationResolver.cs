using Game.Core.Enums;
using Game.Core.Grid;
using Game.Core.Level;

namespace Game.Core.Rules
{
    /// <summary>
    /// Gère la logique de rotation du joueur et des ennemis.
    /// </summary>
    public static class RotationResolver
    {
        public static void ApplyRotation(GridState grid, bool Clockwise)
        {
            // 1. Rotation du joueur
            grid.Player.Rotate(Clockwise);

            // 2. Rotation des ennemis
            foreach (var enemy in grid.Enemies)
            {
                RotateEnemy(enemy, Clockwise);
            }
        }

        private static void RotateEnemy(EnemyState enemy, bool Clockwise)
        {
             switch(enemy.Type)
            {
                case EnemyType.Normal:
                    {
                        enemy.Rotate(Clockwise);
                        break;
                    }
                
                case EnemyType.Inverted:
                    {
                        enemy.Rotate(!Clockwise);
                        break;
                    }
                
                default: break;
            };
        }
    }
}

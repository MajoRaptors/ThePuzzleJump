using Game.Core.Enums;
using Game.Core.Grid;
using Game.Core.Level;
using System;

namespace Game.Core.Rules
{
    /// <summary>
    /// Gère la logique de rotation du joueur et des ennemis.
    /// </summary>
    public static class RotationResolver
    {
        public static void ApplyRotation(GridState grid, bool Clockwise, int movementCount)
        {
            // 1. Rotation du joueur
            grid.Player.Rotate(Clockwise);

            // 2. Rotation des ennemis
            foreach (var enemy in grid.Enemies)
            {

                RotateEnemy(enemy, Clockwise, movementCount);
            }
        }

        private static void RotateEnemy(EnemyState enemy, bool Clockwise, int movementCount)
        {
             switch(enemy.Type)
            {                
                case EnemyType.Inverted:
                    {
                        enemy.Rotate(!Clockwise);
                        break;
                    }

                case EnemyType.SwitcherFirst:
                    {
                        if (movementCount % 2 == 0)
                        {
                            enemy.Rotate(Clockwise);
                        }
                        break;
                    }

                case EnemyType.SwitcherSecond:
                    {
                        if (movementCount % 2 != 0)
                        {
                            enemy.Rotate(Clockwise);
                        }
                        break;
                    }
                
                default:
                    {
                        enemy.Rotate(Clockwise);
                        break;
                    }
            }
            ;
        }
    }
}

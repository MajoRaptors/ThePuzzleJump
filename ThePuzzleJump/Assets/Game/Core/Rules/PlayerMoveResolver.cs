using Game.Core.Enums;
using Game.Core.Grid;
using Game.Core.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Rules
{

    public static class PlayerMoveResolver
    {
        public static PlayerMoveResult Resolve(GridState grid)
        {
            // Calcul de la cellule devant le joueur
            Vector2Int ForwardCell = grid.GetForwardCell(grid.Player);
            // Hors grille
            if (!grid.IsWalkable(ForwardCell.x, ForwardCell.y))
            {
                return Blocked();
            }

            if(grid.HasEnemyAt(ForwardCell))
            {
                return Blocked();
            }

            // Cellule cible
            var cell = grid.GetCell(ForwardCell.x, ForwardCell.y);

            //Cas de Cellule spéciale
            switch (cell.Type)
            {
                case CellType.Solid:
                    {

                        break;
                    }

                default: break;
            }

            // Déplacement valide
            return new PlayerMoveResult
            {
                CanMove = true,
                CausesGameOver = false,
                Target = ForwardCell,
            };
        }

        private static PlayerMoveResult Blocked()
        {
            return new PlayerMoveResult
            {
                CanMove = false,
                CausesGameOver = false
            };
        }

        
    }
}


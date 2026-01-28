using Game.Core.Enums;
using Game.Core.Grid;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Level
{
    /// <summary>
    /// Traduit un LevelData (JSON) en GridState utilisable par le jeu.
    /// </summary>
    public static class LevelLoader
    {
        /// <summary>
        /// Charge un niveau logique à partir de données sérialisées.
        /// </summary>
        public static GridState Load(LevelData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // 1?? Création de la grille logique
            GridState gridState = new GridState(data.width, data.height, CellType.Empty);

            // 2?? Chargement des cellules
            LoadCells(data, gridState);

            // 3?? Chargement du joueur
            LoadPlayer(data, gridState);

            // 4?? Chargement des ennemis
            LoadEnemies(data, gridState);



            return gridState;
        }

        #region Cells

        private static void LoadCells(LevelData data, GridState gridState)
        {
            int index = 0;

            for (int y = 0; y < data.height; y++)
            {
                for (int x = 0; x < data.width; x++)
                {
                    if (index >= data.cells.Length)
                        throw new Exception("Cell data length mismatch with grid size.");

                    gridState.SetCell(x, y, data.cells[index].cellType);
                    index++;
                }
            }
        }

        #endregion

        #region Player

        private static void LoadPlayer(LevelData data, GridState gridState)
        {
            if (data.player == null)
                throw new Exception("Level must contain a player.");

            ValidatePosition(data.player.x, data.player.y, gridState);
            gridState.SetPlayer(new PlayerState(new Vector2Int(data.player.x, data.player.y), data.player.direction));
        }

        #endregion

        #region Enemies

        private static void LoadEnemies(LevelData data, GridState gridState)
        {
            if (data.enemies == null || data.enemies.Count == 0)
                throw new Exception("Level must contain at least one enemy.");

            foreach (EntityData enemy in data.enemies)
            {
                ValidatePosition(enemy.x, enemy.y, gridState);

                gridState.AddEnemy(new EnemyState(new Vector2Int(enemy.x, enemy.y), enemy.direction, enemy.enemyType));
            }
        }

        #endregion

        #region Validation

        private static void ValidatePosition(int x, int y, GridState gridState)
        {
            if (!gridState.IsInside(x, y))
                throw new Exception($"Entity position out of bounds: ({x},{y})");
        }

        #endregion
    }
}

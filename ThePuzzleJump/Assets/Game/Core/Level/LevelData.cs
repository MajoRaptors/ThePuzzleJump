using System;
using System.Collections.Generic;

namespace Game.Core.Level
{
    /// <summary>
    /// Contient la description complète d'un niveau.
    /// Format pivot entre Tool, JSON et Runtime.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        #region Grid

        public int width;
        public int height;

        /// <summary>
        /// Tableau 1D de cellules (index = x + y * width)
        /// </summary>
        public CellData[] cells;

        #endregion

        #region Entities

        public EntityData player;
        public List<EntityData> enemies;

        #endregion

        #region Constructors

        public LevelData(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new CellData[width * height];
            enemies = new List<EntityData>();
        }

        #endregion

        #region Helpers

        public int GetIndex(int x, int y)
        {
            return x + y * width;
        }

        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        #endregion
    }
}

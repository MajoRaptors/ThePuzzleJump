using System;
using Game.Core.Enums;

namespace Game.Core.Level
{
    /// <summary>
    /// Représente UNE cellule logique du niveau.
    /// Pure data : aucun comportement.
    /// </summary>
    [Serializable]
    public class CellData
    {
        #region Fields

        public CellType cellType;

        #endregion

        #region Constructors

        public CellData(CellType cellType)
        {
            this.cellType = cellType;
        }

        #endregion
    }
}

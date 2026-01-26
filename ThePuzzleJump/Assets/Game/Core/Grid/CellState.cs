using Game.Core.Enums;
namespace Game.Core.Grid
{
    public class CellState
    {
        public CellType Type { get; private set; }

        public CellState(CellType type)
        {
            Type = type;
        }
    }
}

using Game.Core.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Rules
{
    public struct EnemyMoveResult
    {
        public List<Vector2Int> FinalPositions;
        public bool IsGameOver;
    }
}

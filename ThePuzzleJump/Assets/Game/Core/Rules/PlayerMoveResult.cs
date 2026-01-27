using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Rules
{
    public struct PlayerMoveResult
    {
        public bool CanMove;
        public bool CausesGameOver;
        public Vector2Int Target;
    }
}


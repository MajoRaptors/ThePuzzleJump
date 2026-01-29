using Game.Core.Grid;
using Game.Core.Level;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Rules
{
    public static class EnemyMoveResolver
    {
        public static EnemyMoveResult Resolve(GridState grid)
        {
            int enemyCount = grid.Enemies.Count;

            // 1️ Intents
            var intents = new List<EnemyMoveIntent>(enemyCount);
            var enemyIndexByPosition = new Dictionary<Vector2Int, int>();

            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = grid.Enemies[i];
                enemyIndexByPosition[enemy.Position] = i;

                Vector2Int desired = grid.GetForwardCell(enemy);
                intents.Add(new EnemyMoveIntent
                {
                    From = enemy.Position,
                    To = desired
                });
            }

            // 2️ Comptage des destinations
            var destinationCounts = new Dictionary<Vector2Int, int>();
            foreach (var intent in intents)
            {
                if (!destinationCounts.ContainsKey(intent.To))
                    destinationCounts[intent.To] = 0;

                destinationCounts[intent.To]++;
            }

            // 3️ Graphe de dépendances
            // enemy i dépend de enemy j si j est sur la case que i veut
            var dependencies = new List<int>[enemyCount];
            for (int i = 0; i < enemyCount; i++)
                dependencies[i] = new List<int>();

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2Int target = intents[i].To;

                if (enemyIndexByPosition.TryGetValue(target, out int blockingEnemy))
                {
                    dependencies[i].Add(blockingEnemy);
                }
            }

            // 4️ Résolution
            var finalPositions = new Vector2Int[enemyCount];
            var state = new ResolveState[enemyCount];

            bool gameOver = false;

            for (int i = 0; i < enemyCount; i++)
            {
                ResolveEnemy(i);
            }

            // 5️ Application finale
            for (int i = 0; i < enemyCount; i++)
            {
                finalPositions[i] = state[i] == ResolveState.Moved
                    ? intents[i].To
                    : intents[i].From;
            }

            return new EnemyMoveResult
            {
                FinalPositions = new List<Vector2Int>(finalPositions),
                IsGameOver = gameOver
            };

            // ============================
            // Local function : résolution DFS
            // ============================
            bool ResolveEnemy(int index)
            {
                if (state[index] == ResolveState.Resolved)
                    return state[index] == ResolveState.Moved;

                if (state[index] == ResolveState.Visiting)
                {
                    // Cycle détecté → blocage
                    state[index] = ResolveState.Blocked;
                    return false;
                }

                state[index] = ResolveState.Visiting;

                var intent = intents[index];

                // Règles de base
                if (!grid.IsWalkable(intent.To.x, intent.To.y))
                {
                    EnemyState enemyState = (EnemyState)grid.GetEnemyAt(intent.From);
                    if (enemyState.Type == Enums.EnemyType.Blind)
                    {
                        gameOver = true;
                        state[index] = ResolveState.Moved;
                        return true;
                    }
                    state[index] = ResolveState.Blocked;
                    return false;
                }

                if (destinationCounts[intent.To] > 1)
                {
                    state[index] = ResolveState.Blocked;
                    return false;
                }

                if (grid.Player.Position == intent.To)
                {
                    gameOver = true;
                    state[index] = ResolveState.Moved;
                    return true;
                }

                // Dépendances
                foreach (int dep in dependencies[index])
                {
                    if (!ResolveEnemy(dep))
                    {
                        state[index] = ResolveState.Blocked;
                        return false;
                    }
                }

                // La case sera libérée → mouvement autorisé
                state[index] = ResolveState.Moved;
                return true;
            }
        }

        private enum ResolveState
        {
            None,
            Visiting,
            Moved,
            Blocked,
            Resolved
        }
    }
}

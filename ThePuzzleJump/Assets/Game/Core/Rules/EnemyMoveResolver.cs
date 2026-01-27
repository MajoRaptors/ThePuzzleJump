using Game.Core.Grid;
using Game.Core.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Rules
{
    public static class EnemyMoveResolver
    {
        public static EnemyMoveResult Resolve(GridState grid)
        {
            var intents = new List<EnemyMoveIntent>();
            List<Vector2Int> finalPositions = new();

            // 1️ Chaque ennemi calcule sa case désirée
            foreach (var enemy in grid.Enemies)
            {
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

            bool gameOver = false;

            int intentCount = 0;
            // 3 Résolution finale
            foreach (var intent in intents)
            {
                // Valeur par défaut : ne bouge pas
                finalPositions.Add(intent.From);

                // Plusieurs ennemis → conflit
                if (destinationCounts[intent.To] > 1)
                {
                    Debug.Log("conflit | id : " + intentCount);
                    continue;
                }

                // Case vide → interdit
                if (!grid.IsInside(intent.To.x, intent.To.y))
                {
                    Debug.Log("case vide | id : " + intentCount + " | case voulue : " + intent.To);
                    continue;
                }

                // Joueur → Game Over
                if (grid.Player.Position == intent.To)
                {
                    finalPositions.Add(intent.To);
                    Debug.Log("gameover | id : " + intentCount);
                    gameOver = true;
                    continue;
                }

                // Ennemi présent → ne bouge pas
                // TODO : Faire une vraie verif pour autoriser les chaines
                if (grid.HasEnemyAt(intent.To))
                {
                    Debug.Log("ennemi deja present sur la case | id : " + intentCount);
                    continue;
                }


                // Mouvement valide
                finalPositions[finalPositions.Count - 1] = intent.To;
                Debug.Log("final cell : " + intent.To);
                intentCount++;
            }

            return new EnemyMoveResult
            {
                FinalPositions = finalPositions,
                IsGameOver = gameOver
            };
        }
    }
}


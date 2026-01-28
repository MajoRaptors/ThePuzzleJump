using System.Collections.Generic;
using UnityEngine;
using Game.Core.Enums;

namespace Game.Core.Grid
{
    public class GridState
    {
        public int Width { get; }
        public int Height { get; }

        private readonly CellState[,] cells;

        public PlayerState Player { get; private set; }
        public IReadOnlyList<EnemyState> Enemies => enemies;

        private readonly List<EnemyState> enemies = new();

        public readonly List<Vector2Int> Goals = new();

        // 
        // CONSTRUCTION
        // 
        public GridState(int width, int height, CellType defaultCellType)
        {
            Width = width;
            Height = height;

            cells = new CellState[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    cells[x, y] = new CellState(defaultCellType);
        }

        // 
        // CELL ACCESS
        // 
        public bool IsInside(int x, int y)
        {
            //Debug.Log("x : " + x + " | y : " + y + " Width : " + Width + " | Height : " + Height);
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public CellState GetCell(int x, int y)
        {
            if (!IsInside(x, y))
                return null;

            return cells[x, y];
        }

        public void SetCell(int x, int y, CellType celltype)
        {
            if (!IsInside(x, y))
                return;
            if(celltype == CellType.Goal)
            {
                Goals.Add(new Vector2Int(x, y));
            }


            cells[x, y] = new CellState(celltype);
        }

        public bool IsWalkable(int x, int y)
        {
            if (!IsInside(x, y))
                return false;

            return cells[x, y].Type != CellType.Empty;
        }

        public Vector2Int GetForwardCell(EntityState entity)
        {
            int targetX = entity.Position.x;
            int targetY = entity.Position.y;

            switch (entity.Direction)
            {
                case Direction.Up:
                    targetY += 1;
                    break;
                case Direction.Right:
                    targetX += 1;
                    break;
                case Direction.Down:
                    targetY -= 1;
                    break;
                case Direction.Left:
                    targetX -= 1;
                    break;
            }
            return new Vector2Int(targetX, targetY);
        }

        public EntityState GetEnemyAt(Vector2Int cellLocation)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Position == cellLocation)
                {
                    return enemy;
                }
            }
            return null;
        }

        // 
        // ENTITIES
        // 
        public void SetPlayer(PlayerState player)
        {
            Player = player;
        }

        public void AddEnemy(EnemyState enemy)
        {
            enemies.Add(enemy);
        }

        // 
        // VALIDATION
        // 
        public bool Validate(out string error)
        {
            if (Player == null)
            {
                error = "No player in grid.";
                return false;
            }

            if (!IsWalkable(Player.Position.x, Player.Position.y))
            {
                error = "Player is not on a walkable cell.";
                return false;
            }

            if (enemies.Count == 0)
            {
                error = "No enemy in grid.";
                return false;
            }

            if (Goals.Count == 0)
            {
                error = "No goals in grid.";
                return false;
            }

            if (Goals.Count > enemies.Count)
            {
                error = "Not anouth enemys to reach Victory. Check Enemies and goals";
                return false;
            }


            foreach (var enemy in enemies)
            {
                if (!IsWalkable(enemy.Position.x, enemy.Position.y))
                {
                    error = "Enemy on non-walkable cell.";
                    return false;
                }
                if(HasEnemyAt(enemy.Position))
                {
                    error = "Two Enemys on the same cell.";
                    return false;
                }
                if(enemy.Position == Player.Position)
                {
                    error = "Enemy on the Player cell.";
                    return false;
                }
            }

            error = null;
            return true;
        }
 

        public bool HasEnemyAt(Vector2Int cellLocation)
        {
            foreach (var enemy in enemies)
            {
                if(enemy.Position == cellLocation)
                {
                    return true;
                }
            }
            return false;   
        }

        // 
        // DEBUG
        // 
        public void DebugPrint()
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                string line = "";

                for (int x = 0; x < Width; x++)
                {
                    if (Player.Position == new Vector2Int(x, y))
                        line += " P ";
                    else if (enemies.Exists(e => e.Position == new Vector2Int(x, y)))
                        line += " E ";
                    else
                        line += cells[x, y].Type == CellType.Solid ? " . " : " X ";
                }

                Debug.Log(line);
            }
        }
    }
}


using Game.Core.Enums;
using Game.Core.Grid;
using Game.Core.Level;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Layout Settings")]
    [SerializeField, Range(0f, 0.4f)]
    private float spacingRatio = 0.15f;

    // Points internes (pas exposés dans l’inspector)
    [SerializeField, HideInInspector]
    private Vector3 pointA = new(-4f, 0f, -4f);

    [SerializeField, HideInInspector]
    private Vector3 pointB = new(4f, 0f, 4f);

#if UNITY_EDITOR
    public Vector3 EditorPointA
    {
        get => pointA;
        set => pointA = value;
    }

    public Vector3 EditorPointB
    {
        get => pointB;
        set => pointB = value;
    }
#endif


    private readonly List<GameObject> spawnedObjects = new();

    public void Build(GridState grid)
    {
        Clear();

        ComputeLayout(
            grid,
            out Vector3 origin,
            out float cellSize,
            out float spacing
        );

        // Cells
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var cell = grid.GetCell(x, y);
                if (cell.Type == CellType.Empty)
                    continue;

                Vector3 pos = origin
                    + new Vector3(
                        x * (cellSize + spacing),
                        0,
                        y * (cellSize + spacing)
                    );

                Spawn(cellPrefab, pos, cellSize);
            }
        }

        // Player
        if (grid.Player != null)
            SpawnEntity(playerPrefab, grid.Player, origin, cellSize, spacing);

        // Enemies
        foreach (var enemy in grid.Enemies)
            SpawnEntity(enemyPrefab, enemy, origin, cellSize, spacing);
    }

    private void ComputeLayout(
        GridState grid,
        out Vector3 origin,
        out float cellSize,
        out float spacing
    )
    {
        Vector3 min = Vector3.Min(pointA, pointB);
        Vector3 max = Vector3.Max(pointA, pointB);

        float width = max.x - min.x;
        float height = max.z - min.z;

        float spacingUnitsX = (grid.Width - 1) * spacingRatio;
        float spacingUnitsY = (grid.Height - 1) * spacingRatio;

        float unitSizeX = width / (grid.Width + spacingUnitsX);
        float unitSizeY = height / (grid.Height + spacingUnitsY);

        cellSize = Mathf.Min(unitSizeX, unitSizeY);
        spacing = cellSize * spacingRatio;

        origin = min + new Vector3(cellSize / 2f, 0f, cellSize / 2f);
    }

    private void SpawnEntity(
        GameObject prefab,
        EntityState entity,
        Vector3 origin,
        float cellSize,
        float spacing
    )
    {
        Vector3 pos = origin
            + new Vector3(
                entity.Position.x * (cellSize + spacing),
                0,
                entity.Position.y * (cellSize + spacing)
            );

        GameObject go = Spawn(prefab, pos, cellSize);

        go.transform.rotation = entity.Direction switch
        {
            Direction.Up => Quaternion.identity,
            Direction.Right => Quaternion.Euler(0, 90, 0),
            Direction.Down => Quaternion.Euler(0, 180, 0),
            Direction.Left => Quaternion.Euler(0, 270, 0),
            _ => Quaternion.identity
        };
    }

    private GameObject Spawn(GameObject prefab, Vector3 pos, float size)
    {
        GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
        go.transform.localScale = Vector3.one * size;
        spawnedObjects.Add(go);
        return go;
    }

    private void Clear()
    {
        foreach (var go in spawnedObjects)
            Destroy(go);

        spawnedObjects.Clear();
    }

    public void Refresh(GridState grid)
    {
        Clear();
        Build(grid);
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);

        Vector3 min = Vector3.Min(pointA, pointB);
        Vector3 max = Vector3.Max(pointA, pointB);

        Vector3 center = (min + max) / 2f;
        Vector3 size = max - min;

        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }

    #endregion
}

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using Game.Core.Enums;
using Game.Core.Level;

public class PuzzleLevelEditor : EditorWindow
{
    private const int CellSize = 50;
    private const int CellPadding = 4;

    private int gridWidth = 5;
    private int gridHeight = 5;

    private int pendingWidth = 5;
    private int pendingHeight = 5;

    private const float ModeOptionsHeight = 60f;


    private enum EditMode
    {
        Cell,
        Player,
        Enemy
    }


    private CellData[,] grid;
    private EditMode currentMode = EditMode.Cell;
    private CellType selectedCellType = CellType.Solid;
    private EnemyType selectedEnemyType = EnemyType.Normal;

    private EntityData player;
    private List<EntityData> enemies = new();

    [MenuItem("Tools/Puzzle Level Editor")]
    public static void Open()
    {
        GetWindow<PuzzleLevelEditor>("Puzzle Level Editor");
    }

    private void OnEnable()
    {
        InitGrid(gridWidth, gridHeight);
    }

    private void InitGrid(int width, int height)
    {
        grid = new CellData[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new CellData(CellType.Empty);
    }

    private void OnGUI()
    {
        DrawGridSizeControls();

        GUILayout.Space(10);

        DrawGrid();

        GUILayout.Space(10);

        // Calcul d'un Rect pour le titre
        Rect titleRect = GUILayoutUtility.GetRect(new GUIContent("Grid Size"), EditorStyles.boldLabel);

        // Fond gris foncé
        EditorGUI.DrawRect(titleRect, new Color(0.18f, 0.18f, 0.18f));

        // Label du titre par-dessus
        GUI.Label(titleRect, "Edit Mode :", EditorStyles.boldLabel);
        currentMode = (EditMode)GUILayout.Toolbar(
            (int)currentMode,
            new[] { "Cell", "Player", "Enemy" }
        );

        GUILayout.Space(10);

        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(ModeOptionsHeight));
        {
            DrawModeOptions();
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);
        if (GUILayout.Button("Import Level (JSON)"))
        {
            ImportJson();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Export Level (JSON)"))
        {
            ExportJson();
        }
    }

    // ================= GRID SIZE =================

    private void DrawGridSizeControls()
    {
        // Calcul d'un Rect pour le titre
        Rect titleRect = GUILayoutUtility.GetRect(new GUIContent("Grid Size"), EditorStyles.boldLabel);

        // Fond gris foncé
        EditorGUI.DrawRect(titleRect, new Color(0.18f, 0.18f, 0.18f));

        // Label du titre par-dessus
        GUI.Label(titleRect, "Grid Size", EditorStyles.boldLabel);

        // On crée un espace horizontal centré
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // pousse le contenu vers le centre

        // Ligne unique pour Width et Height
        GUILayout.BeginHorizontal();

        // Width
        GUILayout.Label("Width:", GUILayout.Width(50));
        pendingWidth = EditorGUILayout.IntField(pendingWidth, GUILayout.Width(50));
        pendingWidth = Mathf.Clamp(pendingWidth, 1, 20); // limite min/max

        // petit espace entre
        GUILayout.Space(20);

        // Height
        GUILayout.Label("Height:", GUILayout.Width(50));
        pendingHeight = EditorGUILayout.IntField(pendingHeight, GUILayout.Width(50));
        pendingHeight = Mathf.Clamp(pendingHeight, 1, 20);

        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace(); // centre horizontalement
        GUILayout.EndHorizontal();

        // Bouton Resize
        if (GUILayout.Button("Resize Grid"))
        {
            TryResizeGrid(pendingWidth, pendingHeight);
        }
    }



    private void TryResizeGrid(int newWidth, int newHeight)
    {
        bool shrinking = newWidth < gridWidth || newHeight < gridHeight;
        if (shrinking && WillLoseEntities(newWidth, newHeight))
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Resize Grid",
                "Certaines entités seront supprimées.\nContinuer ?",
                "Oui",
                "Annuler"
            );
            if (!confirm)
                return;
        }

        ResizeGrid(newWidth, newHeight);
    }

    private bool WillLoseEntities(int w, int h)
    {
        if (player != null && (player.x >= w || player.y >= h))
            return true;

        foreach (var e in enemies)
            if (e.x >= w || e.y >= h)
                return true;

        return false;
    }

    private void ResizeGrid(int w, int h)
    {
        CellData[,] newGrid = new CellData[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                newGrid[x, y] =
                    (x < gridWidth && y < gridHeight) ? grid[x, y] : new CellData(CellType.Empty);

        grid = newGrid;
        gridWidth = w;
        gridHeight = h;

        if (player != null && (player.x >= w || player.y >= h))
            player = null;

        enemies.RemoveAll(e => e.x >= w || e.y >= h);
    }

    // ================= GRID =================

    private void DrawGrid()
    {
        float totalWidth = gridWidth * (CellSize + CellPadding) - CellPadding;
        float originX = (position.width - totalWidth) / 2f;
        float originY = 80f;

        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
            {
                Rect rect = new(
                    originX + x * (CellSize + CellPadding),
                    originY + (gridHeight - 1 - y) * (CellSize + CellPadding),
                    CellSize,
                    CellSize
                );

                DrawCell(rect, x, y);
                HandleCellInput(rect, x, y);
            }

        GUILayout.Space(gridHeight * (CellSize + CellPadding) + 20);
    }

    private void DrawCell(Rect rect, int x, int y)
    {
        Color cellColor;

    switch (grid[x, y].cellType)
    {
        case CellType.Empty:
            cellColor = Color.black;
            break;

        case CellType.Solid:
            cellColor = new Color(0.6f, 0.5f, 0.4f);
            break;

        case CellType.Goal:
            cellColor = new Color(1f, 0.5f, 0f); // orange
            break;

        default:
            cellColor = Color.magenta; // debug visuel si oubli
            break;
    }
        EditorGUI.DrawRect(rect, cellColor);

        if (player != null && player.x == x && player.y == y)
            DrawArrow(rect, player.direction, Color.yellow);
        Color EnemyColor = Color.red;
        foreach (var enemy in enemies)
            if (enemy.x == x && enemy.y == y)
            {   
                switch(enemy.enemyType)
                {
                    case EnemyType.Inverted :
                        EnemyColor = new Color(0.6f, 0.2f, 0.8f);
                        break;
                    case EnemyType.Blind :
                        EnemyColor = Color.white;
                        break;
                }
                DrawArrow(rect, enemy.direction, EnemyColor);
            }
    }

    private void DrawArrow(Rect rect, Direction dir, Color color)
    {
        Vector2 forward = dir switch
        {
            Direction.Up => Vector2.down,
            Direction.Right => Vector2.right,
            Direction.Down => Vector2.up,
            Direction.Left => Vector2.left,
            _ => Vector2.down
        };

        Vector2 center = rect.center;
        Vector2 right = new(forward.y, -forward.x);

        Handles.color = color;
        Handles.DrawAAConvexPolygon(
            center + forward * 12,
            center - forward * 8 + right * 6,
            center - forward * 8 - right * 6
        );
    }

    private void HandleCellInput(Rect rect, int x, int y)
    {
        Event e = Event.current;
        if (!rect.Contains(e.mousePosition))
            return;

        bool isValidEvent =
    e.type == EventType.MouseDown ||
    (currentMode == EditMode.Cell && e.type == EventType.MouseDrag);

        if (!isValidEvent)
            return;


        // CLIC DROIT
        if (e.button == 1 && e.type == EventType.MouseDown)
        {
            if (currentMode == EditMode.Cell)
            {
                grid[x, y].cellType = CellType.Empty;
                RemoveEntitiesIfInvalid(x, y);
            }
            else
            {
                if (player != null && player.x == x && player.y == y)
                    player = null;

                enemies.RemoveAll(en => en.x == x && en.y == y);
            }

            e.Use();
            Repaint();
            return;
        }


        // CLIC GAUCHE
        if (e.button != 0)
            return;

        switch (currentMode)
        {
            case EditMode.Cell:
                grid[x, y].cellType = selectedCellType;
                RemoveEntitiesIfInvalid(x, y);
                break;

            case EditMode.Player:
                if (grid[x, y].cellType == CellType.Empty)
                    break;

                var anEnemy = enemies.Find(en => en.x == x && en.y == y);
                if (anEnemy != null)
                {
                    anEnemy.direction = RotateClockwise(anEnemy.direction);
                    break;
                }

                if (player != null && player.x == x && player.y == y)
                {
                    player.direction = RotateClockwise(player.direction);
                }
                else
                {
                    player = new EntityData(x, y, Direction.Up, EnemyType.Normal);
                }
                break;


            case EditMode.Enemy:
                if (grid[x, y].cellType == CellType.Empty)
                    break;

                if (player != null && player.x == x && player.y == y)
                {
                    player.direction = RotateClockwise(player.direction);
                    break;
                }

                var enemy = enemies.Find(en => en.x == x && en.y == y);
                if (enemy != null)
                {
                    enemy.direction = RotateClockwise(enemy.direction);
                }
                else
                {
                    enemies.Add(new EntityData(x,y,Direction.Up, selectedEnemyType));
                }
                break;
        }

        e.Use();
        Repaint();
    }

    private Direction RotateClockwise(Direction dir)
    {
        return (Direction)(((int)dir + 1) % 4);
    }

    private void RemoveEntitiesIfInvalid(int x, int y)
    {
        if (grid[x, y].cellType == CellType.Empty)
        {
            if (player != null && player.x == x && player.y == y)
                player = null;

            enemies.RemoveAll(e => e.x == x && e.y == y);
        }
    }


    private void DrawModeOptions()
    {
        switch (currentMode)
        {
            case EditMode.Cell:
                selectedCellType =
                    (CellType)EditorGUILayout.EnumPopup("Cell Type", selectedCellType);
                break;

            case EditMode.Enemy:
                selectedEnemyType =
                    (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", selectedEnemyType);
                break;

            case EditMode.Player:
                // Rien à afficher
                // MAIS l'espace est toujours réservé
                GUILayout.Label(" ");
                break;
        }
    }

    private void ImportJson()
    {
        string defaultFolder = Path.Combine(Application.dataPath, "Prefabs/JSON_LEVELS");

        if (!Directory.Exists(defaultFolder))
        {
            Directory.CreateDirectory(defaultFolder);
        }

        string path = EditorUtility.OpenFilePanel(
            "Ouvrir Niveau (JSON)",
            defaultFolder,
            "json"
        );

        if (string.IsNullOrEmpty(path))
            return;

        string json = File.ReadAllText(path);

        LevelData data;
        try
        {
            data = JsonUtility.FromJson<LevelData>(json);
        }
        catch
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Le fichier JSON est invalide ou corrompu.",
                "OK"
            );
            return;
        }

        // On récupère la taille de la grille
        gridWidth = data.width;
        gridHeight = data.height;

        // On initialise la grille
        grid = new CellData[gridWidth, gridHeight];
        int i = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (i < data.cells.Length)
                    grid[x, y] = data.cells[i++];
                else
                    grid[x, y] = new CellData(CellType.Empty);
            }
        }

        // On récupère le joueur et les ennemis
        player = data.player;
        enemies = data.enemies != null ? new List<EntityData>(data.enemies) : new List<EntityData>();

        Repaint();
    }


    private void ExportJson()
    {
        if (player == null || enemies.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Export impossible",
                "Le niveau doit contenir un joueur et au moins un ennemi.",
                "OK"
            );
            return;
        }

        LevelData data = new(gridWidth,gridHeight)
        {
            cells = new CellData[gridWidth * gridHeight],
            player = player,
            enemies = enemies
        };
        int goalcount = 0;
        int i = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                data.cells[i++] = grid[x, y];
                if (grid[x, y].cellType == CellType.Goal)
                    goalcount++;
            }
        }
        if (goalcount == 0)
        {
            EditorUtility.DisplayDialog(
                "Export impossible",
                "Le niveau doit contenir au moins une case de Victoire.",
                "OK"
            );
            return;
        }
        if (goalcount > enemies.Count)
        {
            EditorUtility.DisplayDialog(
                "Export impossible",
                "Le niveau doit contenir au moins autant d'ennemis que de case de Victoire.",
                "OK"
            );
            return;
        }

        string defaultFolder = Path.Combine(Application.dataPath, "Prefabs/JSON_LEVELS");

        if (!Directory.Exists(defaultFolder))
        {
            Directory.CreateDirectory(defaultFolder);
        }

        string path = EditorUtility.SaveFilePanel(
            "Export Level",
            defaultFolder,
            "Level.json",
            "json"
        );

        

        if (!string.IsNullOrEmpty(path))
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }
}

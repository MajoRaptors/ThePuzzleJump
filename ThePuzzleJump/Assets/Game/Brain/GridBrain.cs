using Game.Core.Grid;
using Game.Core.Level;
using Game.Core.Rules;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;


public class GridBrain : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshPro UIText;

    [Header("References")]
    [SerializeField] private GridVisualizer gridVisualizer;
    [Header("Level Input")]
    [SerializeField] private TextAsset levelJson;

    private GridState gridState;
    private bool GameOver = false;
    private bool Victory = false;
    private int MovementsCount = 0;

    private void Start()
    {
        MovementsCount = 0;
        UIText.gameObject.SetActive(false);
        LevelData data = JsonUtility.FromJson<LevelData>(levelJson.text);
        gridState = LevelLoader.Load(data);
        gridVisualizer.Build(gridState);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (GameOver || Victory) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(false);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovementsCount ++;
            MoveForward();
            if (CheckVictory())
            {
                Victory = true;
                UIText.text = "Victory !";
                UIText.gameObject.SetActive(true);
            }
        }
    }

    private void Rotate(bool Clockwise)
    {
        // 1️ Rotation logique
        RotationResolver.ApplyRotation(gridState, Clockwise);

        // 2️ Mise à jour visuelle
        gridVisualizer.Refresh(gridState);
    }

    private void MoveForward()
    {
        // 1️ Déplacement du joueur
        var playerResult = PlayerMoveResolver.Resolve(gridState);

        if (playerResult.CausesGameOver)
        {
            GameOver = true;

            UIText.text = "Game Over !";
            UIText.gameObject.SetActive(true);

            Debug.Log("GAME OVER (player)");
            return;
        }

        if (!playerResult.CanMove)
        {
            Debug.Log("Can't Move (player)");
            return;
        }

        gridState.Player.MoveTo(playerResult.Target);

        // 2️ Déplacement des ennemis
        var enemyResult = EnemyMoveResolver.Resolve(gridState);

        if (enemyResult.IsGameOver)
        {
            GameOver = true;


            UIText.text = "Game Over !";
            UIText.gameObject.SetActive(true);


            Debug.Log("GAME OVER (enemy)");
            return;
        }
        int enemyId = 0;
        foreach (var enemy in gridState.Enemies)
        {
            
            enemy.MoveTo(enemyResult.FinalPositions[enemyId]);
            enemyId++;
        }

        // 3️ Mise à jour visuelle
        gridVisualizer.Refresh(gridState);
    }
    private bool CheckVictory()
    {
        if (gridState.Goals.Count == 0)
        {
            Debug.Log("No goals register in Goal List");
            return false;
        }
        bool everyGoalIsReached = true;
        foreach (var goal in gridState.Goals)
        {
            if (!gridState.HasEnemyAt(goal))
            {
                everyGoalIsReached = false;
                Debug.Log("A goal is not reach");
                break;
            }
        }
        return everyGoalIsReached;
    }
}


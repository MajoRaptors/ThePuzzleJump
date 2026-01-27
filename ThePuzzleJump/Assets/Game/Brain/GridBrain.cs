using Game.Core.Grid;
using Game.Core.Level;
using Game.Core.Rules;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridVisualizer gridVisualizer;
    [Header("Level Input")]
    [SerializeField] private TextAsset levelJson;

    private GridState gridState;

    private void Start()
    {
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
            MoveForward();
        }
    }

    private void Rotate(bool Clockwise)
    {
        // 1️⃣ Rotation logique
        RotationResolver.ApplyRotation(gridState, Clockwise);

        // 2️⃣ Mise à jour visuelle
        gridVisualizer.Refresh(gridState);
    }

    private void MoveForward()
    {
        // 1️⃣ Déplacement du joueur
        var playerResult = PlayerMoveResolver.Resolve(gridState);

        if (playerResult.CausesGameOver)
        {
            Debug.Log("GAME OVER (player)");
            return;
        }

        if (!playerResult.CanMove)
        {
            Debug.Log("Can't Move (player)");
            return;
        }

        gridState.Player.MoveTo(playerResult.Target);

        // 2️⃣ Déplacement des ennemis
        var enemyResult = EnemyMoveResolver.Resolve(gridState);

        if (enemyResult.IsGameOver)
        {
            Debug.Log("GAME OVER (enemy)");
            return;
        }
        int enemyId = 0;
        foreach (var enemy in gridState.Enemies)
        {
            
            enemy.MoveTo(enemyResult.FinalPositions[enemyId]);
            enemyId++;
        }

        /*int nenemyId = 0;
        foreach (var enemy in enemyResult.FinalPositions)
        {

            Debug.Log("inex : " + nenemyId +" |pos in final: " + enemy + " | Final Position at index: " + enemyResult.FinalPositions[nenemyId]);

            nenemyId++;
        }*/

        // 3️⃣ Mise à jour visuelle
        gridVisualizer.Refresh(gridState);
    }
}


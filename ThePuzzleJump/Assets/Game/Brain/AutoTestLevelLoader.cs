using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class AutoTestLevelLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridBrain gridBrain;

    [Header("Levels Folder")]
    [Tooltip("Example: Assets/Prefabs/JSON_LEVELS/WORLD 0")]
    [SerializeField] private string levelsFolderPath;

    [Header("Timing")]
    [SerializeField] private float delayAfterEnd = 10f;

    private string[] levelFiles;
    private int currentLevelIndex = 0;
    private bool waiting = false;

    private void Awake()
    {
        if (gridBrain == null)
        {
            Debug.LogError("AutoTestLevelLoader : GridBrain reference missing");
            enabled = false;
            return;
        }

        if (!Directory.Exists(levelsFolderPath))
        {
            Debug.LogError($"AutoTestLevelLoader : Folder not found\n{levelsFolderPath}");
            enabled = false;
            return;
        }

        levelFiles = Directory
            .GetFiles(levelsFolderPath, "*.json")
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        if (levelFiles.Length == 0)
        {
            Debug.LogError($"AutoTestLevelLoader : No JSON files found in {levelsFolderPath}");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        LoadCurrentLevel();
    }

    private void Update()
    {
        if (waiting)
            return;

        if (gridBrain.Victory)
        {
            waiting = true;
            StartCoroutine(HandleVictory());
        }
        else if (gridBrain.GameOver)
        {
            waiting = true;
            StartCoroutine(HandleDefeat());
        }
    }

    private void LoadCurrentLevel()
    {
        if (currentLevelIndex >= levelFiles.Length)
            return;

        string filePath = levelFiles[currentLevelIndex];
        string json = File.ReadAllText(filePath);

        Debug.Log($"[AutoTest] Loading level : {Path.GetFileName(filePath)}");

        gridBrain.Victory = false;
        gridBrain.GameOver = false;

        gridBrain.levelJson = new TextAsset(json);
        gridBrain.Start();
    }

    private IEnumerator HandleVictory()
    {
        yield return new WaitForSeconds(delayAfterEnd);

        currentLevelIndex++;

        if (currentLevelIndex >= levelFiles.Length)
        {
            Debug.Log("[AutoTest] All levels completed 🎉");
            yield break;
        }

        LoadCurrentLevel();
        waiting = false;
    }

    private IEnumerator HandleDefeat()
    {
        yield return new WaitForSeconds(delayAfterEnd);

        LoadCurrentLevel();
        waiting = false;
    }
}

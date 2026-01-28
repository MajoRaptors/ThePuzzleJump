using UnityEngine;
using Game.Core.Enums;

public class EnemyView : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer enemyRenderer;

    [Header("Materials")]
    [SerializeField] private Material normalMat;
    [SerializeField] private Material invertedMat;

    private EnemyType currentType;

    public void SetEnemyType(EnemyType type)
    {
        currentType = type;
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        switch (currentType)
        {
            case EnemyType.Normal:
                enemyRenderer.material = normalMat;
                break;

            case EnemyType.Inverted:
                enemyRenderer.material = invertedMat;
                break;

            default:
                Debug.LogWarning($"CellType non géré : {currentType}");
                break;
        }
    }
}


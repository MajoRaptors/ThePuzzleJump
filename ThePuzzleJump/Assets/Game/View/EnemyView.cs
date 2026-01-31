using UnityEngine;
using Game.Core.Enums;

public class EnemyView : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer enemyRenderer;

    [Header("Materials")]
    [SerializeField] private Material normalMat;
    [SerializeField] private Material invertedMat;
    [SerializeField] private Material blindMat;
    [SerializeField] private Material switcherFirstMat;
    [SerializeField] private Material switcherSecondMat;

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

            case EnemyType.Blind:
                enemyRenderer.material = blindMat;
                break;

            case EnemyType.SwitcherFirst:
                enemyRenderer.material = switcherFirstMat;
                break;

            case EnemyType.SwitcherSecond:
                enemyRenderer.material = switcherSecondMat;
                break;

            default:
                Debug.LogWarning($"EnemyType non géré : {currentType}");
                break;
        }
    }
}


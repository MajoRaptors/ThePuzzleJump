using UnityEngine;
using Game.Core.Enums;

public class CellView : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer cellRenderer;

    [Header("Materials")]
    [SerializeField] private Material solidMat;
    [SerializeField] private Material goalMat;
    [SerializeField] private Material lockGoalMat;

    private CellType currentType;

    public void SetCellType(CellType type)
    {
        currentType = type;
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        switch (currentType)
        {
            case CellType.Solid:
                cellRenderer.material = solidMat;
                break;

            case CellType.Goal:
                cellRenderer.material = goalMat;
                break;

            case CellType.LockerGoal:
                cellRenderer.material = lockGoalMat;
                break;

            default:
                Debug.LogWarning($"CellType non géré : {currentType}");
                break;
        }
    }
}

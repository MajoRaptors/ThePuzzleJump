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
    [SerializeField] private Material weakenedMat;

    [Header(" Active Weakened Materials")]
    [SerializeField] private int VisibleWeakenedStates = 3;

    private CellType currentType;

    public void SetCellType(CellType type)
    {
        currentType = type;
        ApplyMaterial();
    }

    public void SetWeakenedState(int state)
    {
        Debug.Log("State is :" + state);
        /*switch (state)
        {
            case 3:
                cellRenderer.material = littleWeakenedMat;
                break;

            case 2:
                cellRenderer.material = middleWeakenedMat;
                break;

            case 1:
                cellRenderer.material = veryWeakenedMat;
                break;
        }*/
        SetWeakenedStage(state);

    }
    static readonly int CrackAmountID = Shader.PropertyToID("_CrackAmount");

    public void SetWeakenedStage(int stage)
    {
        float value = (1f / VisibleWeakenedStates) * (stage - 1f);
        Debug.Log("value is :" + value);
        var mpb = new MaterialPropertyBlock();
        var renderer = GetComponent<Renderer>();

        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat(CrackAmountID, value);
        renderer.SetPropertyBlock(mpb);
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

            case CellType.Weakened:
                cellRenderer.material = weakenedMat;
                break;

            default:
                Debug.LogWarning($"CellType non géré : {currentType}");
                break;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [SerializeField] private Battlefield battlefield;
    public Battlefield Battlefield => battlefield;

    private Unit selectedUnit;
    private Cell selectedTargetCell;  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (battlefield == null)
        {
            Debug.LogError("BattleController: Battlefield is not assigned in inspector.");
            return;
        }
        battlefield.Init();
    }

    private void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("BattleController: PlayerController.Instance is null.");
            return;
        }
        PlayerController.Instance.SetTurn(Team.White);
    }
 
    public void OnCellClicked(Cell cell)
    {
        if (cell == null) return;
        if (PlayerController.Instance == null || PlayerController.Instance.IsBusy) return;
        if (selectedUnit == null) return; 

        var moves = MoveValidator.GetAvailableMoves(selectedUnit, battlefield);
        
        if (moves.Contains(cell))
        {
            if (selectedTargetCell != null)
            {
                selectedTargetCell.Highlight(false); // Снимаем подсветку со старой цели
            }

            selectedTargetCell = cell;
             Debug.Log($"Выбрана клетка {cell.X}, {cell.Y}. Нажмите Space для подтверждения.");
        }
    }
 
    public void OnUnitClicked(Unit unit)
    {
        if (unit == null) return;
        if (PlayerController.Instance == null || PlayerController.Instance.IsBusy) return;
        if (unit.Team != PlayerController.Instance.CurrentTurn) return;

        ClearSelection();
        selectedUnit = unit;
        HighlightMoves(unit);
    }

    private void HighlightMoves(Unit unit)
    {
        if (battlefield == null) return;
        var moves = MoveValidator.GetAvailableMoves(unit, battlefield);
        foreach (var move in moves)
        {
            if (move != null) move.Highlight(true);
        }
    }

    public void ClearSelection()
{
    if (selectedUnit != null && battlefield != null)
    {
        var moves = MoveValidator.GetAvailableMoves(selectedUnit, battlefield);
        foreach (var move in moves)
        {
            if (move != null) move.Highlight(false);
        }
    }
 
    if (selectedTargetCell != null)
    { 
        selectedTargetCell.Highlight(false);
        selectedTargetCell = null;
    }

    selectedUnit = null;
}
 
    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ClearSelection();
    }
  
    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return; 
         
        if (selectedUnit == null || selectedTargetCell == null) return;
 
        IGameplayCommand command = new MoveCommand(selectedUnit, selectedTargetCell);
        command.Interact(selectedTargetCell); 
    }
}
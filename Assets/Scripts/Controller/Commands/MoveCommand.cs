using UnityEngine;

public class MoveCommand : IGameplayCommand
{
    private readonly BattleController _controller;
    private readonly Unit _selectedUnit;

    public MoveCommand(BattleController controller, Unit selectedUnit)
    {
        _controller = controller;
        _selectedUnit = selectedUnit;
    }

    public void Interact(Cell targetCell)
    {
        Cell fromCell = _selectedUnit.CurrentCell;
        MoveValidator.Move move = _controller.FindMove(fromCell, targetCell);

        bool wasCapture = move.IsCapture;
        if (wasCapture && move.CapturedUnit != null)
        {
            if (move.CapturedCell != null) move.CapturedCell.OccupiedUnit = null;
            Object.Destroy(move.CapturedUnit.gameObject);
        }

        _selectedUnit.MoveTo(targetCell);
 
        if (_selectedUnit.Type == PieceType.Man)
        {
            int promotionRow = _selectedUnit.Team == Team.White ? 7 : 0;
            if (targetCell.GridPos.y == promotionRow)
                _selectedUnit.PromoteToKing();
        }
 
        if (wasCapture && MoveValidator.HasMoreCaptures(_selectedUnit, _controller.Battlefield))
        {
            _controller.StartRecursiveCapture(_selectedUnit);
            return;
        }

        _controller.FinishTurn();
    }
}
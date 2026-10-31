using UnityEngine;

public class MoveCommand : IGameplayCommand
{
    private readonly Unit unit;
    private readonly Cell targetCell;

    public MoveCommand(Unit unit, Cell targetCell)
    {
        this.unit = unit;
        this.targetCell = targetCell;
    }

    public void Interact(Cell cell)
    {
        if (unit == null || targetCell == null) return;
        if (unit.CurrentCell == null) return;
        if (BattleController.Instance == null || BattleController.Instance.Battlefield == null) return;

        var battlefield = BattleController.Instance.Battlefield;

        int fromX = unit.CurrentCell.X;
        int fromY = unit.CurrentCell.Y;
        int dx = targetCell.X - fromX;
        int dy = targetCell.Y - fromY;
 
        if (Mathf.Abs(dx) >= 2 || Mathf.Abs(dy) >= 2)
        {
            TryCaptureAlongPath(battlefield, fromX, fromY, targetCell);
        }
 
        unit.MoveTo(targetCell);
 
        if (unit.Type == UnitType.Man)
        {
            if ((unit.Team == Team.White && targetCell.Y == Battlefield.Size - 1) ||
                (unit.Team == Team.Black && targetCell.Y == 0))
            {
                unit.Promote();
            }
        }
 
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EndTurn();
        }
    }

    private void TryCaptureAlongPath(Battlefield battlefield, int fromX, int fromY, Cell target)
    {
        int dx = target.X - fromX;
        int dy = target.Y - fromY;

        int stepX = dx > 0 ? 1 : -1;
        int stepY = dy > 0 ? 1 : -1;

        int cx = fromX + stepX;
        int cy = fromY + stepY;
 
        while (cx != target.X || cy != target.Y)
        {
            var mid = battlefield.Cells[cx, cy];
            if (!mid.IsEmpty() && mid.OccupiedUnit != null && mid.OccupiedUnit.Team != unit.Team)
            { 
                Object.Destroy(mid.OccupiedUnit.gameObject);
                mid.ClearUnit();
                return;
            }

            cx += stepX;
            cy += stepY;
        }
    }
}
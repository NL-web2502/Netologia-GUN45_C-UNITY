using System.Collections.Generic;
using UnityEngine;

public static class MoveValidator
{
    public static List<Cell> GetAvailableMoves(Unit unit, Battlefield field)
    {
        var moves = new List<Cell>();
        if (unit == null || field == null || unit.CurrentCell == null) return moves;

        int dir = unit.Team == Team.White ? 1 : -1;
        int[] dx = { 1, -1 };

        if (unit.Type == UnitType.King)
        {
            foreach (int xDir in dx)
            {
                foreach (int yDir in new[] { 1, -1 })
                {
                    int cx = unit.CurrentCell.X + xDir;
                    int cy = unit.CurrentCell.Y + yDir;

                    while (IsInside(cx, cy))
                    {
                        var cell = field.Cells[cx, cy];
                        if (cell == null) break;

                        if (cell.IsEmpty())
                        {
                            moves.Add(cell);
                        }
                        else
                        {
                            var occupant = cell.OccupiedUnit;
                            if (occupant != null && occupant.Team != unit.Team)
                            {
                                int nx = cx + xDir;
                                int ny = cy + yDir;
                                if (IsInside(nx, ny))
                                {
                                    var landing = field.Cells[nx, ny];
                                    if (landing != null && landing.IsEmpty())
                                    {
                                        moves.Add(landing);
                                    }
                                }
                            }
                            break;
                        }

                        cx += xDir;
                        cy += yDir;
                    }
                }
            }

            return moves;
        }
 
        foreach (int xDir in dx)
        {
            int nx = unit.CurrentCell.X + xDir;
            int ny = unit.CurrentCell.Y + dir;

            if (IsInside(nx, ny))
            {
                var target = field.Cells[nx, ny];
                if (target != null)
                {
                    if (target.IsEmpty())
                    {
                        moves.Add(target);
                    }
                    else
                    {
                        var occupant = target.OccupiedUnit;
                        if (occupant != null && occupant.Team != unit.Team)
                        {
                            int bx = nx + xDir;
                            int by = ny + dir;
                            if (IsInside(bx, by))
                            {
                                var landing = field.Cells[bx, by];
                                if (landing != null && landing.IsEmpty())
                                {
                                    moves.Add(landing);
                                }
                            }
                        }
                    }
                }
            }
 
            int backY = unit.CurrentCell.Y - dir;
            int backX = unit.CurrentCell.X + xDir;
            if (IsInside(backX, backY))
            {
                var backTarget = field.Cells[backX, backY];
                if (backTarget != null)
                {
                    var occupant = backTarget.OccupiedUnit;
                    if (occupant != null && occupant.Team != unit.Team)
                    {
                        int bx = backX + xDir;
                        int by = backY - dir;
                        if (IsInside(bx, by))
                        {
                            var landing = field.Cells[bx, by];
                            if (landing != null && landing.IsEmpty())
                            {
                                moves.Add(landing);
                            }
                        }
                    }
                }
            }
        }

        return moves;
    }

    private static bool IsInside(int x, int y)
    {
        return x >= 0 && x < Battlefield.Size && y >= 0 && y < Battlefield.Size;
    }
}
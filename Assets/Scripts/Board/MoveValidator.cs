using System.Collections.Generic;
using UnityEngine;

public static class MoveValidator
{
    public struct Move
    {
        public Cell From;
        public Cell To;
        public Unit CapturedUnit;
        public Cell CapturedCell;
        public bool IsCapture => CapturedUnit != null;
    }

    private static readonly Vector2Int[] DiagDirs =
    {
        new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
    };

    public static List<Move> GetMoves(Unit unit, Battlefield field, bool capturesOnly = false)
    {
        var moves = new List<Move>();
        if (unit == null || unit.CurrentCell == null) return moves;

        if (unit.Type == PieceType.King) GetKingMoves(unit, field, moves, capturesOnly);
        else GetManMoves(unit, field, moves, capturesOnly);

        return moves;
    }

    private static void GetManMoves(Unit unit, Battlefield field, List<Move> moves, bool capturesOnly)
    {
        Cell from = unit.CurrentCell;
        int forward = unit.Team == Team.White ? 1 : -1;

        foreach (var d in DiagDirs)
        {
            bool isForward = d.y == forward;
 
            if (!capturesOnly && isForward)
            {
                Vector2Int p = from.GridPos + d;
                if (field.IsValidPos(p))
                {
                    Cell c = field.GetCell(p);
                    if (c.OccupiedUnit == null)
                        moves.Add(new Move { From = from, To = c });
                }
            }

            // Взятие — в любом направлении (в т.ч. назад)
            Vector2Int enemyPos = from.GridPos + d;
            Vector2Int jumpPos = enemyPos + d;
            if (!field.IsValidPos(enemyPos) || !field.IsValidPos(jumpPos)) continue;

            Cell enemyCell = field.GetCell(enemyPos);
            Cell jumpCell = field.GetCell(jumpPos);

            if (enemyCell.OccupiedUnit != null
                && enemyCell.OccupiedUnit.Team != unit.Team
                && jumpCell.OccupiedUnit == null)
            {
                moves.Add(new Move
                {
                    From = from,
                    To = jumpCell,
                    CapturedUnit = enemyCell.OccupiedUnit,
                    CapturedCell = enemyCell
                });
            }
        }
    }

    private static void GetKingMoves(Unit unit, Battlefield field, List<Move> moves, bool capturesOnly)
    {
        Cell from = unit.CurrentCell;

        foreach (var d in DiagDirs)
        {
            int x = from.GridPos.x + d.x;
            int y = from.GridPos.y + d.y;
            Cell firstEnemy = null;

            while (true)
            {
                Vector2Int p = new Vector2Int(x, y);
                if (!field.IsValidPos(p)) break;
                Cell c = field.GetCell(p);

                if (c.OccupiedUnit == null)
                {
                    if (!capturesOnly && firstEnemy == null)
                        moves.Add(new Move { From = from, To = c });
                }
                else
                {
                    if (c.OccupiedUnit.Team == unit.Team) break;
                    if (firstEnemy == null) firstEnemy = c;
                    else break;
                }

                if (firstEnemy != null)
                { 
                    x += d.x; y += d.y;
                    Vector2Int afterPos = new Vector2Int(x, y);
                    if (field.IsValidPos(afterPos))
                    {
                        Cell after = field.GetCell(afterPos);
                        if (after.OccupiedUnit == null)
                        {
                            moves.Add(new Move
                            {
                                From = from,
                                To = after,
                                CapturedUnit = firstEnemy.OccupiedUnit,
                                CapturedCell = firstEnemy
                            });
                        }
                    }
                    break;
                }

                x += d.x; y += d.y;
            }
        }
    }

    public static List<Move> FilterByMandatoryCapture(List<Move> moves)
    {
        var captures = moves.FindAll(m => m.IsCapture);
        return captures.Count > 0 ? captures : moves;
    }

    public static bool HasMoreCaptures(Unit unit, Battlefield field)
    {
        return GetMoves(unit, field, capturesOnly: true).Count > 0;
    }
}
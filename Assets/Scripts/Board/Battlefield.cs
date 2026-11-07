using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private Transform boardRoot;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Material blackCellMat;
    [SerializeField] private Material whiteCellMat;
    [SerializeField] private Material whiteUnitMat;
    [SerializeField] private Material blackUnitMat;

    public Color BlackColor { get; private set; }
    public Color WhiteColor { get; private set; }

    public List<Cell> AllCells { get; private set; } = new List<Cell>();
    private Cell[,] _grid = new Cell[8, 8];

    public void GenerateBoard(BattleController controller)
    {
        BlackColor = blackCellMat.color;
        WhiteColor = whiteCellMat.color;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Cell cell = Instantiate(cellPrefab, boardRoot);
                cell.transform.position = new Vector3(x * cellSize, 0, y * cellSize);
                
                bool isBlack = (x + y) % 2 != 0;
                cell.GetComponent<MeshRenderer>().material = isBlack ? blackCellMat : whiteCellMat;
                
                cell.Init(new Vector2Int(x, y), controller, this); 
                
                _grid[x, y] = cell;
                AllCells.Add(cell);
            }
        }
        SpawnUnits(controller);
    }

    private void SpawnUnits(BattleController controller)
    {
        for (int y = 0; y < 3; y++) 
            for (int x = 0; x < 8; x++)
                if ((x + y) % 2 != 0) 
                    SpawnUnit(x, y, Team.White, controller);

        for (int y = 5; y < 8; y++) 
            for (int x = 0; x < 8; x++)
                if ((x + y) % 2 != 0) 
                    SpawnUnit(x, y, Team.Black, controller);
    }

    private void SpawnUnit(int x, int y, Team team, BattleController controller)
    {
        Unit unit = Instantiate(unitPrefab, _grid[x, y].transform.position + Vector3.up * 0.1f, Quaternion.identity, boardRoot);
        
        Material matToUse = team == Team.White ? whiteUnitMat : blackUnitMat;
        
        unit.Init(team, PieceType.Man, _grid[x, y], controller, matToUse);
    }

    public Cell GetCell(Vector2Int pos) => _grid[pos.x, pos.y];
    public bool IsValidPos(Vector2Int pos) => pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
}
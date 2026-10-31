using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public const int Size = 8;
    public Cell[,] Cells = new Cell[Size, Size];

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private Transform boardRoot;

    public void Init()
    {
     for (int x = 0; x < Size; x++)
{
    for (int y = 0; y < Size; y++)
    {
        var cell = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity, boardRoot);

        var rend = cell.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = (x + y) % 2 == 0 ? Color.gray : Color.white;
        }
        else
        {
            Debug.LogError($"Cell prefab has no Renderer at ({x},{y})");
        }

        cell.Init(x, y);
        Cells[x, y] = cell;
    }
}

        SpawnUnits();
    }

    private void SpawnUnits()
{   
    for (int y = 0; y < 3; y++)
    {
        for (int x = 0; x < Size; x++)
        {
            if ((x + y) % 2 == 0)
            {
                Vector3 spawnPos = Cells[x, y].transform.position + Vector3.up * 0.2f;  
                var unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity, boardRoot);
                unit.Init(Team.White, UnitType.Man, Cells[x, y]); 
            }
        }
    }
  
    for (int y = 5; y < Size; y++)
    {
        for (int x = 0; x < Size; x++)
        {
            if ((x + y) % 2 == 0)
            {
                Vector3 spawnPos = Cells[x, y].transform.position + Vector3.up * 0.2f; 
                var unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity, boardRoot);
                unit.Init(Team.Black, UnitType.Man, Cells[x, y]);
            }
        }
    }
}

    public List<Cell> GetNeighbors(Cell cell, Team team, bool forwardOnly)
    { 
        return new List<Cell>();
    }
}
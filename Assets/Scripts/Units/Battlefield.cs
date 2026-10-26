using System;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public const int BoardSize = 8;

    public event Action<Cell> CellClicked;

    public Cell[,] Cells { get; private set; }

    private Sprite _squareSprite;
    private Sprite _unitSprite;

    private void Awake()
    {
        CreateSprites();
        CreateBoard();
        CreateStartingUnits();
    }

    private void CreateSprites()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        _squareSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f);

        _unitSprite = _squareSprite;
    }

    private void CreateBoard()
    {
        Cells = new Cell[BoardSize, BoardSize];

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                GameObject cellObject = new GameObject(
                    $"Cell_{x}_{y}");

                cellObject.transform.SetParent(transform);
                cellObject.transform.position =
                    new Vector3(x, y, 0f);

                SpriteRenderer renderer =
                    cellObject.AddComponent<SpriteRenderer>();

                renderer.sprite = _squareSprite;
                renderer.sortingOrder = 0;
                renderer.drawMode = SpriteDrawMode.Sliced;
                cellObject.transform.localScale =
                    new Vector3(1f, 1f, 1f);

                bool isDark = (x + y) % 2 == 1;

                Color color = isDark
                    ? new Color(0.25f, 0.28f, 0.32f)
                    : new Color(0.85f, 0.82f, 0.75f);

                BoxCollider2D collider =
                    cellObject.AddComponent<BoxCollider2D>();

                collider.size = Vector2.one;

                Cell cell = cellObject.AddComponent<Cell>();
                cell.Initialize(x, y, this, color);

                Cells[x, y] = cell;
            }
        }
    }

    private void CreateStartingUnits()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if ((x + y) % 2 != 1)
                    continue;

                if (y < 3)
                    CreateUnit(x, y, PlayerColor.Dark);
                else if (y > 4)
                    CreateUnit(x, y, PlayerColor.White);
            }
        }
    }

    private void CreateUnit(
        int x,
        int y,
        PlayerColor color)
    {
        GameObject unitObject = new GameObject(
            $"{color}_Checker_{x}_{y}");

        unitObject.transform.SetParent(transform);
        unitObject.transform.position =
            new Vector3(x, y, -0.1f);

        SpriteRenderer renderer =
            unitObject.AddComponent<SpriteRenderer>();

        renderer.sprite = _unitSprite;
        renderer.sortingOrder = 1;

        unitObject.transform.localScale =
            new Vector3(0.78f, 0.78f, 1f);

        unitObject.AddComponent<CircleCollider2D>();

        Unit unit = unitObject.AddComponent<Unit>();
        unit.Initialize(color, Cells[x, y], this);
    }

    public void NotifyCellClicked(Cell cell)
    {
        CellClicked?.Invoke(cell);
    }
}
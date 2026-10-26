using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public PlayerColor Color { get; private set; }
    public UnitType Type { get; private set; }
    public Cell Cell { get; private set; }

    private SpriteRenderer _renderer;
    private Battlefield _battlefield;

    public void Initialize(
        PlayerColor color,
        Cell cell,
        Battlefield battlefield)
    {
        Color = color;
        Type = UnitType.Checker;
        Cell = cell;
        _battlefield = battlefield;

        _renderer = GetComponent<SpriteRenderer>();

        _renderer.color = color == PlayerColor.White
            ? new Color(0.95f, 0.9f, 0.75f)
            : new Color(0.2f, 0.2f, 0.25f);

        Cell.SetUnit(this);
    }

    public void SetCell(Cell cell)
    {
        if (Cell != null)
            Cell.ClearUnit();

        Cell = cell;
        Cell.SetUnit(this);
        transform.position = new Vector3(
            cell.X, cell.Y, -0.1f);
    }

    public void PromoteToKing()
    {
        Type = UnitType.King;
 
        _renderer.color = new Color(1f, 0.65f, 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battlefield.NotifyCellClicked(Cell);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Пока отдельная подсветка шашки не требуется.
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _battlefield.NotifyCellClicked(Cell);
    }
}






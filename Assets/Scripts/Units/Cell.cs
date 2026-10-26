using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Unit Unit { get; private set; }

    private SpriteRenderer _renderer;
    private Color _baseColor;
    private Battlefield _battlefield;

    public void Initialize(
        int x,
        int y,
        Battlefield battlefield,
        Color color)
    {
        X = x;
        Y = y;
        _battlefield = battlefield;
        _baseColor = color;

        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = color;
    }

    public void SetUnit(Unit unit)
    {
        Unit = unit;
    }

    public void ClearUnit()
    {
        Unit = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_renderer != null)
            _renderer.color = Color.Lerp(
                _baseColor, Color.yellow, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_renderer != null)
            _renderer.color = _baseColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _battlefield.NotifyCellClicked(this);
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject kingVisual;
    public Team Team { get; private set; }
    public PieceType Type { get; private set; }
    public Cell CurrentCell { get; private set; }

    private BattleController _battleController;

    public void Init(Team team, PieceType type, Cell cell, BattleController controller)
    {
        Team = team;
        Type = type;
        CurrentCell = cell;
        _battleController = controller;
        cell.OccupiedUnit = this;
        UpdateVisual();
    }

    public void MoveTo(Cell newCell)
    {
        if (CurrentCell != null) CurrentCell.OccupiedUnit = null;
        CurrentCell = newCell;
        newCell.OccupiedUnit = this;
        transform.position = newCell.transform.position + Vector3.up * 0.5f;
    }

    public void PromoteToKing()
    {
        Type = PieceType.King;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (kingVisual != null) kingVisual.SetActive(Type == PieceType.King);
    }

    public void OnPointerEnter(PointerEventData eventData) => _battleController?.OnUnitHover(this, true);
    public void OnPointerExit(PointerEventData eventData) => _battleController?.OnUnitHover(this, false);
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _battleController?.OnUnitClicked(this);
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject kingVisual;
    private MeshRenderer _meshRenderer;

    public Team Team { get; private set; }
    public PieceType Type { get; private set; }
    public Cell CurrentCell { get; private set; }

    private BattleController _battleController;

    public void Init(Team team, PieceType type, Cell cell, BattleController controller, Material material)
    {
        Team = team;
        Type = type;
        CurrentCell = cell;
        _battleController = controller;

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null && material != null)
            _meshRenderer.material = material;

        cell.OccupiedUnit = this;
        UpdateVisual();
    }

    public void MoveTo(Cell newCell)
    {
        if (CurrentCell != null) CurrentCell.OccupiedUnit = null;
        CurrentCell = newCell;
        newCell.OccupiedUnit = this;
        transform.position = newCell.transform.position + Vector3.up * 0.1f;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battleController?.OnUnitHover(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _battleController?.OnUnitHover(this, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
}
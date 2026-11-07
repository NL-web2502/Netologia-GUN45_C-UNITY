using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MeshRenderer meshRenderer;

    public Vector2Int GridPos { get; private set; }
    public Unit OccupiedUnit { get; set; }
    public bool IsBlackCell => (GridPos.x + GridPos.y) % 2 != 0;

    private BattleController _battleController;
    private Battlefield _battlefield;

    private bool _isMoveTarget;
    private bool _isSelected;

    public void Init(Vector2Int pos, BattleController controller, Battlefield battlefield)
    {
        GridPos = pos;
        _battleController = controller;
        _battlefield = battlefield; 
        SetHighlight(false, false, false);
    }

    public void SetHighlight(bool hover, bool selected, bool moveTarget)
    {
        if (meshRenderer == null) return;
        
        _isMoveTarget = moveTarget;
        _isSelected = selected;

        if (moveTarget) meshRenderer.material.color = Color.cyan;
        else if (selected) meshRenderer.material.color = Color.green;
        else if (hover) meshRenderer.material.color = Color.yellow;
        else 
        {
            if (IsBlackCell && _battlefield != null) 
                meshRenderer.material.color = _battlefield.BlackColor;
            else if (!IsBlackCell && _battlefield != null) 
                meshRenderer.material.color = _battlefield.WhiteColor;
            else 
                meshRenderer.material.color = Color.white; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battleController?.OnCellHover(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _battleController?.OnCellHover(this, false);
        if (_isMoveTarget || _isSelected)
            SetHighlight(false, _isSelected, _isMoveTarget);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
}
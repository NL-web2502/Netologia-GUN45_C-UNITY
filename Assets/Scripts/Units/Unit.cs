using UnityEngine;
using UnityEngine.EventSystems;
public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Team Team;
    public UnitType Type;
    public Cell CurrentCell { get; private set; }
  
    public void Init(Team team, UnitType type, Cell cell)
    {
        Team = team;
        Type = type;
        CurrentCell = cell;
        cell.SetUnit(this);
        UpdateVisual();
    }
   
    public void MoveTo(Cell target)
    {
        CurrentCell.ClearUnit();
        CurrentCell = target;
        target.SetUnit(this);
        transform.position = target.transform.position + Vector3.up * 0.2f;
    }
   
    public void Promote()
    {
        Type = UnitType.King;
        UpdateVisual();
    }
    
    private void UpdateVisual()
{
    var rend = GetComponent<Renderer>();
    if (rend != null)
    {
        rend.material.color = Type == UnitType.King
            ? Color.yellow
            : (Team == Team.White ? Color.white : Color.black);
    }
}
   
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
   
    public void OnPointerClick(PointerEventData eventData)
    {
        BattleController.Instance.OnUnitClicked(this);
    }
}
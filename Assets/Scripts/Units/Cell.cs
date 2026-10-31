using UnityEngine;
using UnityEngine.EventSystems;
public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Unit OccupiedUnit { get; private set; }
   
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 1f);
    
    private Color originalColor;
    private Renderer rend;
    private Material mat;
    
    public void Init(int x, int y)
    {
        X = x;
        Y = y;

        rend = GetComponent<Renderer>();
       
        if (rend != null)
        {
            mat = rend.material;
            originalColor = mat.color;
        }
       
        else
        {
            Debug.LogError($"Cell ({x},{y}): no Renderer found on {name}");
        }
    }
    
    public void SetUnit(Unit unit) => OccupiedUnit = unit;
    public void ClearUnit() => OccupiedUnit = null;
    public bool IsEmpty() => OccupiedUnit == null;
   
    public void Highlight(bool active)
    {
        if (mat == null) return;
        mat.color = active ? highlightColor : originalColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BattleController.Instance != null)
            BattleController.Instance.OnCellClicked(this);
    }
}
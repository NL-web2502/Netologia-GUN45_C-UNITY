using UnityEngine;
using TMPro;  

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public Team CurrentTurn { get; private set; }
    public bool IsBusy { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text turnText;  

    private void Awake() => Instance = this;

    public void SetTurn(Team team)
    {
        CurrentTurn = team;
        UpdateTurnUI();
        Debug.Log($"Turn: {team}");
    }

    public void EndTurn()
    {
        BattleController.Instance.ClearSelection();

        CurrentTurn = CurrentTurn == Team.White ? Team.Black : Team.White;
        
        UpdateTurnUI();
        Debug.Log($"Turn: {CurrentTurn}");
    }

    private void UpdateTurnUI()
    {
        if (turnText != null)
        {
            turnText.text = CurrentTurn == Team.White ? "White move" : "Black move"; 
            turnText.color = CurrentTurn == Team.White ? Color.white : Color.black;
        }
    }
}
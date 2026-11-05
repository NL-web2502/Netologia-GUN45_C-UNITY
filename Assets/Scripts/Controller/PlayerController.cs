using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private BattleController battleController;

    public void OnTurnChanged(Team team)
    {
        if (turnText != null)
            turnText.text = team == Team.White ? "Ход белых" : "Ход чёрных";
    }

    public void SetInputBlocked(bool blocked)
    {
        if (battleController != null)
            battleController.enabled = !blocked;
    }
}
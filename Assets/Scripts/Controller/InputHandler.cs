using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction cancelAction;
    private InputAction confirmAction;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputHandler: InputActionAsset is not assigned.");
            return;
        }

        var map = inputActions.FindActionMap("Gameplay", throwIfNotFound: true);
        cancelAction = map.FindAction("Cancel", throwIfNotFound: true);
        confirmAction = map.FindAction("Confirm", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        if (cancelAction != null)
        {
            cancelAction.performed += HandleCancel;
            cancelAction.Enable();
        }
        if (confirmAction != null)
        {
            confirmAction.performed += HandleConfirm;
            confirmAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (cancelAction != null)
        {
            cancelAction.performed -= HandleCancel;
            cancelAction.Disable();
        }
        if (confirmAction != null)
        {
            confirmAction.performed -= HandleConfirm;
            confirmAction.Disable();
        }
    }

    private void HandleCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        BattleController.Instance?.OnCancel(ctx);
    }

    private void HandleConfirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        BattleController.Instance?.OnConfirm(ctx);
    }
}
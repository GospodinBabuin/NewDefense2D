using UnityEngine;

public class InputReader : MonoBehaviour
{
    public PlayerInputActions InputActions => _inputActions;
    private PlayerInputActions _inputActions;

    public float Move => _inputActions.Player.Move.ReadValue<float>();
    public bool Attack => _inputActions.Player.Attack.triggered;
    public bool Interact => _inputActions.Player.Interact.triggered;
    public bool UpgradeObject => _inputActions.Player.UpgradeObject.triggered;
    public bool RepairObject => _inputActions.Player.RepairObject.triggered;
    public bool ConfirmAction => _inputActions.Building.Confirm.triggered;
    public bool CancelAction => _inputActions.Building.Cancel.triggered;
    public bool Chat => _inputActions.UI.Chat.triggered;
    public bool PauseMenu => _inputActions.UI.PauseMenu.triggered;
    public bool Confirm => _inputActions.UI.Confirm.triggered;
    
    public bool Escape => _inputActions.UI.PauseMenu.triggered;
    
    public void Enable()
    {
        _inputActions ??= new PlayerInputActions();
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions?.Disable();
    }
}

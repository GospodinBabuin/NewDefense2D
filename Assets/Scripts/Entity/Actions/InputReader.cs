using UnityEngine;

public class InputReader : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    public float Move => _inputActions.Player.Move.ReadValue<float>();
    public bool Attack => _inputActions.Player.Attack.triggered;
    public bool Interact => _inputActions.Player.Interact.triggered;
    public bool UpgradeObject => _inputActions.Player.UpgradeObject.triggered;
    public bool RepairObject => _inputActions.Player.RepairObject.triggered;
    public bool ConfirmAction => _inputActions.Building.Confirm.triggered;
    public bool CancelAction => _inputActions.Building.Cancel.triggered;
    
    
    public bool Escape => _inputActions.UI.PauseMenu.triggered;
    
    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}

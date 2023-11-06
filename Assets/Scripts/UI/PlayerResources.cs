using UnityEngine;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private Text currentGoldField;
    [SerializeField] private GoldBank _goldbank;

    private Animator _animator;

    private int _animIDClose;
    private int _animIDShowResources;
    private int _animIDHideResources;
    
    private enum ResourcesState
    {
        Hide,
        Show
    }

    private ResourcesState _resourcesState = ResourcesState.Hide;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SetAnimIDs();
    }

    private void Start()
    {
        currentGoldField.text = _goldbank.Gold.ToString();
        _goldbank.OnGoldValueChangedEvent += OnGoldValueChanged;
    }

    private void OnGoldValueChanged(object sender, int oldGoldValue, int newGoldValue)
    {
        currentGoldField.text = newGoldValue.ToString();
    }

    private void SetAnimIDs()
    {
        _animIDClose = Animator.StringToHash("Close");
        _animIDShowResources = Animator.StringToHash("ShowResources");
        _animIDHideResources = Animator.StringToHash("HideResources");
    }

    public void ShowOrHideResources()
    {
        switch (_resourcesState)
        {
            case ResourcesState.Show:
                _animator.SetTrigger(_animIDHideResources);
                _resourcesState = ResourcesState.Hide;
                break;
            case ResourcesState.Hide:
                _animator.SetTrigger(_animIDShowResources);
                _resourcesState = ResourcesState.Show;
                break;
        }
    }
}

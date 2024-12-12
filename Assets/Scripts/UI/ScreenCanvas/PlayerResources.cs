using System;
using GoldSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerResources : MonoBehaviour
    {
        public static PlayerResources Instance;
    
        [SerializeField] private Text currentGoldField;

        private Animator _animator;

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
        
            if (Instance == null)
            {
                Instance = this;
                return;
            }
        
            Destroy(gameObject);
        }

        private void Start()
        {
            currentGoldField.text = GoldBank.Instance.Gold.ToString();
            GoldBank.Instance.OnGoldValueChangedEvent += OnGoldValueChanged;
        }

        private void OnGoldValueChanged(object sender, int oldGoldValue, int newGoldValue)
        {
            currentGoldField.text = newGoldValue.ToString();
        }

        private void SetAnimIDs()
        {
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

        public void ShowResources()
        {
            if (_resourcesState == ResourcesState.Show)
                return;
        
            _animator.SetTrigger(_animIDShowResources);
            _resourcesState = ResourcesState.Show;
        }

        public void HideResources()
        {
            if (_resourcesState == ResourcesState.Hide)
                return;
        
            _animator.SetTrigger(_animIDHideResources);
            _resourcesState = ResourcesState.Hide;
        }

    }
}

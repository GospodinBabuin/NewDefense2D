using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class Notifications : MonoBehaviour
    {
        [SerializeField] private Text newDayNotificationText;

        private Animator _animator;

        private int _animIDShowNewDayNotification;
        private int _animIDShowNotEnoughMoneyNotification;
        private int _animIDShowImpossibleToPlaceBuildingNotification;
        private int _animIDShowImpossibleToRecruitMoreSoldiersNotification;
        private int _animIDShowCantAddMoreHealthNotification;
        private int _animIDShowCreatingLobbyNotification;
        private int _animIDShowLobbyCreatedNotification;
        private int _animIDShowOfflineModeNotification;
        private int _animIDShowDefeatNotification;
    
        private void Start()
        {
            _animator = GetComponent<Animator>();

            _animIDShowNewDayNotification = Animator.StringToHash("ShowNewDayNotification");
            _animIDShowNotEnoughMoneyNotification = Animator.StringToHash("ShowNotEnoughMoneyNotification");
            _animIDShowImpossibleToPlaceBuildingNotification = Animator.StringToHash("ShowImpossibleToPlaceBuilding");
            _animIDShowImpossibleToRecruitMoreSoldiersNotification = Animator.StringToHash("ShowImpossibleToRecruitMoreSoldiersNotification");
            _animIDShowCantAddMoreHealthNotification = Animator.StringToHash("ShowCantAddMoreHealthNotification");
            _animIDShowCreatingLobbyNotification = Animator.StringToHash("ShowCreatingLobbyNotification");
            _animIDShowLobbyCreatedNotification = Animator.StringToHash("ShowLobbyCreatedNotification");
            _animIDShowOfflineModeNotification = Animator.StringToHash("ShowOfflineModeNotification");
            _animIDShowDefeatNotification = Animator.StringToHash("ShowDefeatNotification");
        }

        public void ShowNewDayNotification(int currentDay)
        {
            newDayNotificationText.text = $"Day {currentDay}";
            _animator.SetTrigger(_animIDShowNewDayNotification);
        }
    
        public void ShowNotEnoughMoneyNotification()
        {
            _animator.SetTrigger(_animIDShowNotEnoughMoneyNotification);
        }
    
        public void ShowImpossibleToPlaceBuildingNotification()
        {
            _animator.SetTrigger(_animIDShowImpossibleToPlaceBuildingNotification);
        }
        
        public void ShowImpossibleToRecruitMoreSoldiersNotification()
        {
            _animator.SetTrigger(_animIDShowImpossibleToRecruitMoreSoldiersNotification);
        }

        public void ShowCantAddMoreHealthNotification()
        {
            _animator.SetTrigger(_animIDShowCantAddMoreHealthNotification);
        }

        public void ShowCreatingLobbyNotification()
        {
            _animator.SetTrigger(_animIDShowCreatingLobbyNotification);
        }

        public void ShowLobbyCreatedNotification()
        {
            _animator.SetTrigger(_animIDShowLobbyCreatedNotification);
        }

        public void ShowLobbyWasNotCreatedNotification()
        {
            _animator.SetTrigger(_animIDShowOfflineModeNotification);
        }

        public void ShowDefeatNotification()
        {
            _animator.SetTrigger(_animIDShowDefeatNotification);
        }
    }
}

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
    
        private void Start()
        {
            _animator = GetComponent<Animator>();

            _animIDShowNewDayNotification = Animator.StringToHash("ShowNewDayNotification");
            _animIDShowNotEnoughMoneyNotification = Animator.StringToHash("ShowNotEnoughMoneyNotification");
            _animIDShowImpossibleToPlaceBuildingNotification = Animator.StringToHash("ShowImpossibleToPlaceBuilding");
            _animIDShowImpossibleToRecruitMoreSoldiersNotification = Animator.StringToHash("ShowImpossibleToRecruitMoreSoldiersNotification");
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
    }
}

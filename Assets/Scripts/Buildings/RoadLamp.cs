using System;
using UnityEngine;

namespace Buildings
{
    public class RoadLamp : Building
    {
        private Animator _animator;

        private int _animIDTurnOn;
        private int _animIDTurnOff;

        private bool _lightsOn = false;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();

            _animIDTurnOn = Animator.StringToHash("TurnOn");
            _animIDTurnOff = Animator.StringToHash("TurnOff");
            
            DayManager.Instance.OnDayStateChangedEvent += FadeLights;

            if (DayManager.Instance.dayState == DayManager.DayState.Day) return;
            
            _animator.SetTrigger(_animIDTurnOn);
            _lightsOn = true;
        }

        private void FadeLights(DayManager.DayState dayState, int currentDay)
        {
            switch (dayState)
            {
                case DayManager.DayState.Day when _lightsOn:
                    _animator.SetTrigger(_animIDTurnOff);
                    _lightsOn = false;
                    break;
                case DayManager.DayState.Evening when !_lightsOn:
                    _animator.SetTrigger(_animIDTurnOn);
                    _lightsOn = true;
                    break;
                case DayManager.DayState.Night when !_lightsOn:
                    _animator.SetTrigger(_animIDTurnOn);
                    _lightsOn = true;
                    break;
            }
        }

        public override void UpgradeBuilding()
        {
            Debug.Log("cant upgrade this building");
        }

        protected override void OnDestroy()
        {
            if (ObjectsInWorld.Instance.Buildings.Contains(this))
                ObjectsInWorld.Instance.RemoveBuildingFromList(this, false);
            
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Health>().enabled = false;
        }
    }
}

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


        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _animIDTurnOn = Animator.StringToHash("TurnOn");
            _animIDTurnOff = Animator.StringToHash("TurnOff");
        }

        private void Start()
        {
            DayManager.Instance.OnDayStateChangedEvent += FadeLights;

            if (DayManager.Instance.dayState == DayManager.DayState.Day) return;
            
            _animator.SetTrigger(_animIDTurnOn);
        }

        private void FadeLights(DayManager.DayState dayState, int currentDay)
        {
            switch (dayState)
            {
                case DayManager.DayState.Day when _lightsOn:
                    _animator.SetTrigger(_animIDTurnOff);
                    _lightsOn = false;
                    return;
                case DayManager.DayState.Evening when !_lightsOn:
                    _animator.SetTrigger(_animIDTurnOn);
                    _lightsOn = true;
                    return;
                case DayManager.DayState.Night when !_lightsOn:
                    _animator.SetTrigger(_animIDTurnOn);
                    _lightsOn = true;
                    return;
            }
        }

        protected override void OnDestroy()
        {
            if (ObjectsInWorld.Instance.Buildings.Contains(this))
                ObjectsInWorld.Instance.RemoveBuildingFromList(this, false);
        }
    }
}

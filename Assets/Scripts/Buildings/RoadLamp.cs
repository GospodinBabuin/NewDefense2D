using UnityEngine;

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

        if (DayManager.Instance.dayState != DayManager.DayState.DAY)
        {
            _animator.SetTrigger(_animIDTurnOn);
            return;
        }
    }

    private void FadeLights(DayManager.DayState dayState, int currentDay)
    {
        if (dayState == DayManager.DayState.DAY && _lightsOn)
        {
            _animator.SetTrigger(_animIDTurnOff);
            _lightsOn = false;
            return;
        }

        if (dayState == DayManager.DayState.EVENING && !_lightsOn)
        {
            _animator.SetTrigger(_animIDTurnOn);
            _lightsOn = true;
            return;
        }

        if (dayState == DayManager.DayState.NIGHT && !_lightsOn)
        {
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

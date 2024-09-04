using UnityEngine;

public class SunRays : MonoBehaviour
{
    private Animator _animator;

    private int _animIDEvanescet;
    private int _animIDAppear;

    private bool _raysOn = true;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _animIDEvanescet = Animator.StringToHash("Evanescet");
        _animIDAppear = Animator.StringToHash("Appear");
    }

    private void Start()
    {
        if (DayManager.Instance == null) return;
        
        DayManager.Instance.OnDayStateChangedEvent += FadeRays;
    }

    private void FadeRays(DayManager.DayState dayState, int currentDay)
    {
        if (dayState == DayManager.DayState.Day && !_raysOn)
        {
            _animator.SetTrigger(_animIDAppear);
            _raysOn = true;
            return;
        }

        if (dayState == DayManager.DayState.Evening && _raysOn)
        {
            _animator.SetTrigger(_animIDEvanescet);
            _raysOn = false;
            return;
        }

        if (dayState == DayManager.DayState.Night && _raysOn)
        {
            _animator.SetTrigger(_animIDEvanescet);
            _raysOn = false;
            return;
        }
    }
}

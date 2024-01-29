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
        DayManager.Instance.OnDayStateChangedEvent += FadeRays;
    }

    private void FadeRays(DayManager.DayState dayState, int currentDay)
    {
        if (dayState == DayManager.DayState.DAY && !_raysOn)
        {
            _animator.SetTrigger(_animIDAppear);
            _raysOn = true;
            return;
        }

        if (dayState == DayManager.DayState.EVENING && _raysOn)
        {
            _animator.SetTrigger(_animIDEvanescet);
            _raysOn = false;
            return;
        }

        if (dayState == DayManager.DayState.NIGHT && _raysOn)
        {
            _animator.SetTrigger(_animIDEvanescet);
            _raysOn = false;
            return;
        }
    }
}

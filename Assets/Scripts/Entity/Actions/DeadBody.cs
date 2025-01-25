using UnityEngine;

public class DeadBody : MonoBehaviour
{
    private Animator _animator;
    private int _animIDVanish;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animIDVanish = Animator.StringToHash("Vanish");
        
        DayManager.Instance.OnDayStateChangedEvent += Vanish;
    }

    private void Vanish(DayManager.DayState daystate, int currentday)
    {
        if (daystate != DayManager.DayState.Day) return;
            
        _animator.SetTrigger(_animIDVanish);
            
        //2 seconds is the total animation playback time for all objects.
        Destroy(gameObject, 2f);
    }
}

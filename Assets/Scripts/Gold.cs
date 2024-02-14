using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Gold : MonoBehaviour
{
    private Animator _animator;

    private int _animIDCollect;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _animIDCollect = Animator.StringToHash("Collect");
    }

    private void DestroyCoinAnimEvent()
    {
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerController player)) return;
        
        player.GetComponentInChildren<GoldBank>().AddGold(this, 1);
        _animator.SetTrigger(_animIDCollect);
    }
}

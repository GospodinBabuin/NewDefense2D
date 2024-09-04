using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Gold : MonoBehaviour
{
    private Animator _animator;
    private int _animIDCollect;
    private int _animIDSpawn;
    
    public delegate void OnDisableCallback(Gold instance);
    public OnDisableCallback Disable;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _animIDCollect = Animator.StringToHash("Collect");
        _animIDSpawn = Animator.StringToHash("Spawn");
    }

    private void CoinCollectedAnimEvent()
    {
        Disable?.Invoke(this);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerController player)) return;

        if (player.GetComponent<NetworkObject>().IsOwner)
        {
            GoldBank.Instance.AddGold(this, 1);
        }
        _animator.SetTrigger(_animIDCollect);
    }

    private void OnEnable()
    {
        _animator.SetTrigger(_animIDSpawn);
    }
}

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Combat : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private float attackDelay = 1.5f;
    [SerializeField] private int attackAnimationCount;
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask attackLayers;
    [SerializeField] private bool canDamageMultipleTargets = false;

    [SerializeField] private bool IsSoldierLvl1 = false;
    public float AttackDelay => attackDelay;
    
    private bool _canAttack = true;
    private int _animIDAttack1;
    private int _animIDAttack2;
    private int _animIDAttack3;
    private int _animIDAttack4;
    
    private byte _attackAnimationIndexForSoldierLvl1;
    
    protected virtual void Start()
    {
        _animator = GetComponent<Animator>();
        
        SetAnimIDs();

        if (IsSoldierLvl1)
            _attackAnimationIndexForSoldierLvl1 = (byte)Random.Range(1, attackAnimationCount + 1);
    }
    
    public void Attack()
    { 
        if (!_canAttack) return;
        
        _animator.SetTrigger(ChoseAttackAnimation());
        _canAttack = false;
        StartCoroutine(StartNextAttackCooldown(attackDelay));
    }

    private int ChoseAttackAnimation()
    {
        byte attackAnimation;
        
        if (IsSoldierLvl1)
            attackAnimation = _attackAnimationIndexForSoldierLvl1;
        else
            attackAnimation = (byte)Random.Range(1, attackAnimationCount + 1);
        
        return attackAnimation switch
        {
            1 => _animIDAttack1,
            2 => _animIDAttack2,
            3 => _animIDAttack3,
            4 => _animIDAttack4,
            _ => _animIDAttack1
        };
    }

    private void DealDamage()
    {
        foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, attackLayers))
        {
            Health health = collider2D.GetComponent<Health>();
            
            if (health == null) continue;
            
            health.DamageServerRpc(damage);
                
            if (!canDamageMultipleTargets)
                return;
        }
    }

    public IEnumerator StartNextAttackCooldown(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);
        _canAttack = true;
    }
    
    public void AttackAnimationEvent()
    {
        DealDamage();
    }

    public void IncreaseDamage(int increaseDamage)
    {
        damage += increaseDamage;
    }
    
    private void SetAnimIDs()
    {
        _animIDAttack1 = Animator.StringToHash("Attack1");
        _animIDAttack2 = Animator.StringToHash("Attack2");
        _animIDAttack3 = Animator.StringToHash("Attack3");
        _animIDAttack4 = Animator.StringToHash("Attack4");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

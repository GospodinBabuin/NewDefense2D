using UnityEngine;

public class EntityHealth : Health
{
    private Combat _combat;
    private int _animIDTakeDamage;

    
    protected override void Awake()
    {
        base.Awake();

        _combat = GetComponent<Combat>();
    }

    public override void DamageServerRpc(int damageAmount)
    {
        animator.SetTrigger(_animIDTakeDamage);
        
        base.DamageServerRpc(damageAmount);
        
        _combat.StopAllCoroutines();
        _combat.StartCoroutine(_combat.StartNextAttackCooldown(_combat.AttackDelay / 4));
    }

    protected override void Die()
    {
        base.Die();
        
        Destroy(GetComponent<Entity>());
    }

    protected override void SetAnimIDs()
    {
        base.SetAnimIDs();
        
        _animIDTakeDamage = Animator.StringToHash("TakeDamage");
    }
}

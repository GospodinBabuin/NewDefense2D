using Environment;
using System;
using System.Collections.Generic;
using HealthSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EntityHealth))]
[RequireComponent(typeof(Combat))]
[RequireComponent(typeof(Locomotion))]
[RequireComponent(typeof(SortingGroup))]
public class Entity : NetworkBehaviour
{
    private EntityHealth _health;
    private Combat _combat;
    private Locomotion _locomotion;
    
    public Combat Combat => _combat;
    public Locomotion Locomotion => _locomotion;
    public EntityHealth Health => _health;

    [SerializeField] private byte visionRange = 15;
    [SerializeField] protected MonoBehaviour nearestFoe;

    protected virtual void Awake()
    {
        _health = GetComponent<EntityHealth>();
        _combat = GetComponent<Combat>();
        _locomotion = GetComponent<Locomotion>();

        SetPositionOnGround();
    }

    private void SetPositionOnGround()
    {
        LayerMask groundLayerMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, float.MaxValue, groundLayerMask);
        if (hit) transform.position = hit.point + new Vector2(0, GetComponent<BoxCollider2D>().size.y / 2);
        transform.position += new Vector3(0, 0, Random.Range(0f, 1f));
    }

    protected T FindNearestFoe<T>(List<T> foes, bool activateVisionRange) where T : MonoBehaviour
    {
        if (foes.Count == 0)
            return null;

        T tempNearestFoe = null;

        foreach (var foe in foes)
        {
            if (activateVisionRange)
            {
                if (Math.Abs(transform.position.x - foe.transform.position.x) > visionRange)
                    continue;
            }

            if (tempNearestFoe == null)
            {
                tempNearestFoe = foe;
                continue;
            }

            if (Math.Abs(transform.position.x - foe.transform.position.x) < Math.Abs(transform.position.x - tempNearestFoe.transform.position.x))
                tempNearestFoe = foe;
        }

        return tempNearestFoe;
    }

    public override void OnDestroy()
    {
        DeadBody deadBody = gameObject.AddComponent<DeadBody>();
        ObjectsInWorld.Instance?.AddDeadBodiesToList(deadBody);
        GetComponent<Collider2D>().enabled = false;
        _health.enabled = false;
        _combat.enabled = false;
        _locomotion.enabled = false;
        
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
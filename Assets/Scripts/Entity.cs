using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Combat))]
[RequireComponent(typeof(Locomotion))]
[RequireComponent(typeof(SortingGroup))]
public class Entity : MonoBehaviour
{
    private Health _health;
    private Combat _combat;
    private Locomotion _locomotion;
    
    public Combat Combat => _combat;
    public Locomotion Locomotion => _locomotion;

    [SerializeField] private byte visionRange = 15;
    [SerializeField] protected MonoBehaviour nearestFoe;
    
    protected virtual void Start()
    {
        _health = GetComponent<Health>();
        _combat = GetComponent<Combat>();
        _locomotion = GetComponent<Locomotion>();
        
        SetPositionOnGround();
    }
    
    private void SetPositionOnGround()
    {
        LayerMask groundLayerMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, float.MaxValue, groundLayerMask);
        if (hit) transform.position = hit.point + new Vector2(0, GetComponent<BoxCollider2D>().size.y/2);
    }
    
    protected T FindNearestFoe<T>(List<T> foes, bool activateVisionRange) where T : MonoBehaviour
    {
        if (foes.Count == 0)
            return null;

        T tempNearestFoe = null;
        
        for (int i = 0; i < foes.Count; i++)
        {
            if (activateVisionRange)
            {
                if (Math.Abs(transform.position.x - foes[i].transform.position.x) > visionRange)
                    continue;
            }

            if (tempNearestFoe == null)
            {
                tempNearestFoe = foes[i];
                continue;
            }
            
            if (Math.Abs(transform.position.x - foes[i].transform.position.x) < Math.Abs(transform.position.x - tempNearestFoe.transform.position.x))
                tempNearestFoe = foes[i];
        }
        
        return tempNearestFoe;
    }
    

    protected virtual void OnDestroy()
    {
        ObjectsInWorld.Instance.AddDeadBodiesToList(gameObject);
        GetComponent<Collider2D>().enabled = false;
        _health.enabled = false;
        _combat.enabled = false;
        _locomotion.enabled = false;     
    }
}
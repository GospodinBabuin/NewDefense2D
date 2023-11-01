using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Health))]
public class Entity : MonoBehaviour
{
    private Health _health;
    [SerializeField] protected float speed;

    private void Start()
    {
        _health = GetComponent<Health>();
    }

    protected void Move()
    {
        
    }
    
    
}

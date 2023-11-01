using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BuildingBoundaries : MonoBehaviour
{
    private CircleCollider2D _collider;
    
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    public bool IsBoundariesClear()
    {
        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(transform.position, _collider.radius);

        if (colliders == null) return true;
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;
            
            if (collider.CompareTag("BuildingBoundaries"))
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (_collider)
            Gizmos.DrawWireSphere(_collider.offset, _collider.radius);
    }
}

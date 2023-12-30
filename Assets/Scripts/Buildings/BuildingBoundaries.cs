using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BuildingBoundaries : MonoBehaviour
{
    private CircleCollider2D _collider;
    [SerializeField] private LayerMask notClearTargetsLayers;
    
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    public bool IsBoundariesClear()
    {
        Collider2D[] colliders;
        colliders = Physics2D.OverlapCircleAll(transform.position, _collider.radius, notClearTargetsLayers);

        if (colliders == null) return true;
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (_collider)
            Gizmos.DrawWireSphere(_collider.offset, _collider.radius);
    }
}

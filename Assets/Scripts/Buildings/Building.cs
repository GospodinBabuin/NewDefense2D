using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Building : MonoBehaviour
{
    private Health _health;
    protected byte BuildingLvl { get; private set; }

    private void Awake()
    {
        _health = GetComponent<Health>();
        BuildingLvl = 1;
    }

    protected virtual void OnDestroy()
    {
        if (ObjectsInWorld.Instance.Buildings.Contains(this))
            ObjectsInWorld.Instance.RemoveBuildingFromList(this, true);
    }
}

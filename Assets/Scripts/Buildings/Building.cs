using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Building : MonoBehaviour
{
    public Health Health { get; private set; }
    public byte BuildingLvl { get; private set; }

    private void Awake()
    {
        Health = GetComponent<Health>();
        BuildingLvl = 1;
    }
}

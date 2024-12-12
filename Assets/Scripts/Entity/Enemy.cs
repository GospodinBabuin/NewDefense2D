using Environment;
using GoldSystem;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Entity
{
    [SerializeField] private byte goldAfterDeath = 5;

    private void Start()
    {
        ObjectsInWorld.Instance.AddEnemyToList(this);
    }

    private void Update()
    {   
        if (!IsHost) return;

        if (nearestFoe != null)
        {
            if (!Locomotion.CloseEnough(nearestFoe.transform.position))
            {
                Locomotion.RotateAndMoveWithVelocity(nearestFoe.transform.position);
                return;
            }
            else
            {
                Combat.Attack();
                Locomotion.SetMoveAnimation(false);
                return;
            }
        }
        
        Locomotion.SetMoveAnimation(false);
    }

    private void FixedUpdate()
    {
        if (!IsHost) return;

        nearestFoe = FindNearestFoe(ObjectsInWorld.Instance.AlliedObjects, false);
    }
    
    private Vector2 GetRandomPoint(Vector2 oldPos, float radius)
    {
        Vector2 spawnPoint = oldPos;

        spawnPoint.x += Random.Range(-radius, radius);
        spawnPoint.y += Random.Range(-radius, radius);
        
        return spawnPoint;
    }
    
    public override void OnDestroy()
    {
        for (int i = 0; i < goldAfterDeath; i++)
        {
            GoldSpawner.Instance.SpawnGold(GetRandomPoint(transform.position, 0.3f), quaternion.identity);
        }

        base.OnDestroy();
        ObjectsInWorld.Instance.RemoveEnemyFromList(this);
    }
}

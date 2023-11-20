using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    protected override void Start()
    {
        ObjectsInWorld.Instance.AddEnemyToList(this);
    }
}

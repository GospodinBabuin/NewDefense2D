using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlliedSoldier : Entity
{
    private Transform _selectedBorder;
    private Vector2 _selectedBorderPosition;

    private Enemy _nearestEnemy;

    [SerializeField] private byte visionRange = 15;

    protected override void Start()
    {
        base.Start();

        ObjectsInWorld.Instance.AddSolderToList(this);
        GreenZoneBorders.Instance.OnBordersPositionChangedEvent += SelectBordersPosition;
    }

    private void Update()
    {
        if (_nearestEnemy != null)
        {
            RotateAndMove(_nearestEnemy.transform.position);
            return;
        }

        if (!IsOnBorder(_selectedBorderPosition))
        {
            RotateAndMove(_selectedBorderPosition);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!GreenZoneBorders.Instance.IsBeyondGreenZoneBorders(transform.position))
        {
            _nearestEnemy = FindNearestEnemy(ObjectsInWorld.Instance.Enemies);
        }
    }

    public void SelectBorder(Transform newBorder)
    {
        _selectedBorder = newBorder;

        SelectBordersPosition();
    }

    private void SelectBordersPosition()
    {
        _selectedBorderPosition = _selectedBorder.position;
        _selectedBorderPosition = AddRandomToTargetPosition(_selectedBorderPosition, 0f, 1.5f);
    }

    private bool IsOnBorder(Vector2 targetPosition)
    {
        return Math.Abs(transform.position.x - targetPosition.x) <= 0.1;
    }

    private Vector2 AddRandomToTargetPosition(Vector2 OldPosition, float minRandomValue, float maxRandomValue)
    {
        Vector2 NewPosition = new Vector2(OldPosition.x, OldPosition.y);

        minRandomValue = Math.Abs(minRandomValue);
        maxRandomValue = Math.Abs(maxRandomValue);

        if (NewPosition.x < 0)
        {
            NewPosition += new Vector2(Random.Range(minRandomValue, maxRandomValue), NewPosition.y);
        }
        else
        {
            NewPosition += new Vector2(Random.Range(-minRandomValue, -maxRandomValue), NewPosition.y);
        }

        return NewPosition;
    }

    private Enemy FindNearestEnemy(List<Enemy> enemies)
    {
        if (enemies.Count == 0)
            return null;

        Enemy nearestEnemy = enemies[0];

        for (int i = 1; i < enemies.Count; i++)
        {
            if (Math.Abs(transform.position.x - enemies[i].transform.position.x) > visionRange)
                continue;

            if (Math.Abs(transform.position.x - enemies[i].transform.position.x) < Math.Abs(transform.position.x - nearestEnemy.transform.position.x))
                nearestEnemy = enemies[i];
        }

        return nearestEnemy;
    }
}

using System;
using Environment;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlliedSoldier : Entity
{
    public Transform SelectedBorder { get; private set; }
    private Vector2 _selectedBorderPosition;

    private void Start()
    {
        ObjectsInWorld.Instance.AddSoldierToList(this);
        GreenZoneBorders.Instance.OnBordersPositionChangedEvent += SelectBordersPosition;
        GreenZoneBorders.Instance.SelectBorder(this);
    }

    private void Update()
    {
        if (nearestFoe != null)
        {
            if (!Locomotion.CloseEnough(nearestFoe.transform.position))
            {
                Locomotion.RotateAndMove(nearestFoe.transform.position);
                return;
            }
            else
            {
                Combat.Attack();
                Locomotion.SetMoveAnimation(false);
                return;
            }
        }

        if (!IsOnBorder(_selectedBorderPosition))
        {
            Locomotion.RotateAndMove(_selectedBorderPosition);
            return;
        }
        
        Locomotion.SetMoveAnimation(false);
    }

    private void FixedUpdate()
    {
        if (!GreenZoneBorders.Instance.IsBeyondGreenZoneBorders(transform.position) || GreenZoneBorders.Instance.IsBeyondDefaultGreenZoneBorders(transform.position))
        {
            nearestFoe = FindNearestFoe(ObjectsInWorld.Instance.Enemies, true);
        }
    }

    public void SelectBorder(Transform newBorder)
    {
        SelectedBorder = newBorder;

        SelectBordersPosition();
    }

    private void SelectBordersPosition()
    {
        _selectedBorderPosition = SelectedBorder.position;
        _selectedBorderPosition = AddRandomToTargetPosition(_selectedBorderPosition, 0f, 1.5f);
    }

    private bool IsOnBorder(Vector2 targetPosition)
    {
        return Math.Abs(transform.position.x - targetPosition.x) <= 0.1;
    }

    private static Vector2 AddRandomToTargetPosition(Vector2 oldPosition, float minRandomValue, float maxRandomValue)
    {
        Vector2 newPosition = new Vector2(oldPosition.x, oldPosition.y);

        minRandomValue = Math.Abs(minRandomValue);
        maxRandomValue = Math.Abs(maxRandomValue);

        if (newPosition.x < 0)
        {
            newPosition += new Vector2(Random.Range(minRandomValue, maxRandomValue), newPosition.y);
        }
        else
        {
            newPosition += new Vector2(Random.Range(-minRandomValue, -maxRandomValue), newPosition.y);
        }

        return newPosition;
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance.RemoveSoldierFromList(this);
        GreenZoneBorders.Instance.RemoveFromBorder(this);
    }
}

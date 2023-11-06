using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlliedSoldier : Entity
{
    private Transform _selectedBorder;
    private Vector2 _selectedBorderPosition;

    protected override void Awake()
    {
        base.Awake();

        ObjectsInWorld.Instance.AddSolderToList(this);
        GreenZoneBorders.Instance.OnBordersPositionChangedEvent += SelectBordersPosition;
    }

    private void Update()
    {
        if (!IsOnBorder(_selectedBorderPosition))
        {
            MoveToBorder(_selectedBorderPosition);
            return;
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
        _selectedBorderPosition = AddRandomToBorderPosition(_selectedBorderPosition);
    }

    private void MoveToBorder(Vector2 targetPosition)
    {
        Rotate(targetPosition);

        Move(targetPosition.x < transform.position.x ? -transform.right : transform.right);
    }

    private bool IsOnBorder(Vector2 targetPosition)
    {
        return Math.Abs(transform.position.x - targetPosition.x) <= 0.1;
    }

    private Vector2 AddRandomToBorderPosition(Vector2 borderOldPosition)
    {
        Vector2 borderNewPosition = new Vector2(borderOldPosition.x, borderOldPosition.y);

        if (borderNewPosition.x < 0)
        {
            borderNewPosition += new Vector2(Random.Range(0f, 1.5f), borderNewPosition.y);
        }
        else
        {
            borderNewPosition += new Vector2(Random.Range(-0f, -1.5f), borderNewPosition.y);
        }

        return borderNewPosition;
    }
}

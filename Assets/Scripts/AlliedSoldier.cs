
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlliedSoldier : Entity
{
    public Transform SelectedBorder { get; private set; }
    private Transform _targetTransform;

    private void Start()
    {
        GreenZoneBorders.Instance.AddSolderToList(this);
        Debug.Log(SelectedBorder);
    }
    
    private void FixedUpdate()
    {
        if (_targetTransform == null)
        {
            Vector2 newTargetPosition = FindCurrentBorderPosition();
            _targetTransform.position = new Vector2(newTargetPosition.x, newTargetPosition.y);
        }
        
        
        if (!IsOnBorder(_targetTransform.position))
        {
            MoveToBorder(_targetTransform.position);
            return;
        }
    }

    public void SelectBorder(Transform newBorder)
    {
        SelectedBorder = newBorder;
    }

    private void MoveToBorder(Vector2 targetPosition)
    {
        transform.rotation = SelectedBorder.position.x < transform.position.x ?
            Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        
        transform.position = Vector2.MoveTowards(transform.position, 
            targetPosition, speed * Time.deltaTime);
    }

    private bool IsOnBorder(Vector2 targetPosition)
    {
        return Math.Abs(transform.position.x - targetPosition.x) < 0.1;
    }

    private Vector2 FindCurrentBorderPosition()
    {
        Vector3 targetPosition = new Vector3(SelectedBorder.position.x, transform.position.y);
        
        if (SelectedBorder.position.x < 0)
        {
            targetPosition += new Vector3(Random.Range(0.25f, 0.5f), transform.position.y);
        }
        else
        {
            targetPosition += new Vector3(Random.Range(-0.25f, -0.5f), transform.position.y);
        }
        
        return targetPosition;
    }
}

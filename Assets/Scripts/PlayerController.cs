using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    private InputReader _input;
    private Animator _animator;

    private int _animIDMove;
    
    private void Awake()
    {
        _input = GetComponent<InputReader>();
        _animator = GetComponent<Animator>();
        SetAnimIDs();
    }

    private void Update()
    {
        Rotate();
        Move();
    }

    private void SetAnimIDs()
    {
        _animIDMove = Animator.StringToHash("IsMoving");
    }
    
    private void Move()
    {
        if (_input.Move == 0f)
        {
            _animator.SetBool(_animIDMove, false);
            return;
        }

        transform.position += new Vector3(_input.Move, 0, 0) * (speed * Time.deltaTime);
        
        _animator.SetBool(_animIDMove, true);
    }

    private void Rotate()
    {
        if (!Mathf.Approximately(0, _input.Move))
        {
            transform.rotation = _input.Move < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }
}

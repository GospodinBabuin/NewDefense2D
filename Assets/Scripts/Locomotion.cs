using UnityEngine;

public class Locomotion : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float stopDistance = 1f;
    private int _animIDMove;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        _animIDMove = Animator.StringToHash("IsMoving");

        AddRandomToStopDistance();
    }

    private void Move(Vector2 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
        SetMoveAnimation(true);
    }
    
    private void Rotate(Vector2 direction)
    {
        transform.rotation = direction.x > transform.position.x ?
            Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }

    public void RotateAndMove(Vector2 direction)
    {
        Rotate(direction);

        Move(direction.x < transform.position.x ? -transform.right : transform.right);
    }

    public bool CloseEnough(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) <= stopDistance;
    }

    public void SetMoveAnimation(bool moveState)
    {
        _animator.SetBool(_animIDMove, moveState);
    }

    public float GetSpeed()
    {
        return speed;
    }
    
    public float GetStopDistance()
    {
        return stopDistance;
    }

    private void AddRandomToStopDistance()
    {
        stopDistance += Random.Range(-0.1f, 0.35f);
    }
}

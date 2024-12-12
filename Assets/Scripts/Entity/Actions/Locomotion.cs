using AudioSystem;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float stopDistance = 1f;

    [SerializeField] private float minStopDistance = -0.1f;
    [SerializeField] private float maxStopDistance = 0.35f;

    [SerializeField] private SoundData soundData;
    
    public float Speed { get => speed; private set => speed = value; }
    public float StopDistance { get => stopDistance; private set => stopDistance = value; }
    
    private int _animIDMove;
    private Animator _animator;
    private Transform _spriteTransform;
    
    private Rigidbody2D _rigidbody2D;
    private Vector2 _velocity = Vector2.zero;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = 0.05f;

    private void Awake()
    {
        _spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        
        _animator = GetComponent<Animator>();
        
        _animIDMove = Animator.StringToHash("IsMoving");
        
        _rigidbody2D = GetComponent<Rigidbody2D>();

        AddRandomToStopDistance();
    }

    private void MoveWithTransform(Vector2 direction)
    {
        transform.Translate(speed * Time.deltaTime * direction);
        SetMoveAnimation(true);
    }
    
    private void MoveWithTransform(float direction)
    {
        if (direction == 0f)
        {
            SetMoveAnimation(false);
            return;
        }
        
        //Vector2 directionVector = new Vector2(direction, 0);
        //transform.Translate(speed * Time.deltaTime * directionVector);
        
        transform.position += new Vector3(direction, 0, 0) * (speed * Time.deltaTime);
        SetMoveAnimation(true);
    }

    private void MoveWithVelocity(Vector2 direction)
    {
        Vector2 targetVelocity = new Vector2(direction.x * (speed * Time.fixedDeltaTime), _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = Vector2.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);
        SetMoveAnimation(true);
    }

    private void MoveWithVelocity(float direction)
    {
        if (direction == 0f)
        {
            SetMoveAnimation(false);
            return;
        }
        
        Vector2 targetVelocity = new Vector2(direction * speed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = Vector2.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);
        
        SetMoveAnimation(true);
    }
    
    private void Rotate(Vector2 direction)
    {
        _spriteTransform.rotation = direction.x > transform.position.x ?
            Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }
    
    private void Rotate(float direction)
    {
        if (direction == 0) return;
        
        _spriteTransform.rotation = direction > 0 ? 
            Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }

    public void RotateAndMoveWithTransform(Vector2 direction)
    {
        Rotate(direction);
        MoveWithTransform(direction.x < transform.position.x ? -transform.right : transform.right);
    }
    
    public void RotateAndMoveWithTransform(float direction)
    {
        Rotate(direction);
        MoveWithTransform(direction);
    }

    public void RotateAndMoveWithVelocity(Vector2 direction)
    {
        Rotate(direction);
        MoveWithVelocity(direction.x < transform.position.x ? -transform.right : transform.right);
    }
    
    public void RotateAndMoveWithVelocity(float direction)
    {
        Rotate(direction);
        MoveWithVelocity(direction);
    }
    
    public bool CloseEnough(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) <= stopDistance;
    }

    public void SetMoveAnimation(bool moveState)
    {
        _animator.SetBool(_animIDMove, moveState);
    }

    public void IncreaseSpeed(float increaseSpeed)
    {
        speed += increaseSpeed;
    }
    
    private void AddRandomToStopDistance()
    {
        stopDistance += Random.Range(minStopDistance, maxStopDistance);
    }

    public void OnFootstepAnimEvent()
    {
        SoundManager.Instance.CreateSound()
            .WithSoundData(soundData)
            .WithRandomPitch()
            .WithPosition(transform.position)
            .Play();
    }
}

using AudioSystem;
using Unity.Netcode;
using UnityEngine;

public class Locomotion : NetworkBehaviour
{
    [SerializeField] private float stopDistance = 1f;
    
    [SerializeField] private NetworkVariable<float> speed = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<float> speedAnimationMultiplier = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [SerializeField] private float minStopDistance = -0.1f;
    [SerializeField] private float maxStopDistance = 0.35f;

    [SerializeField] private SoundData soundData;
    
    public float Speed { get => speed.Value; private set => speed.Value = value; }
    public float StopDistance { get => stopDistance; private set => stopDistance = value; }
    public float SpeedAnimationMultiplier { get => speedAnimationMultiplier.Value; private set => speedAnimationMultiplier.Value = value; }
    
    private int _animIDMove;
    private int _animIDMultiplier;
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
        _animIDMultiplier = Animator.StringToHash("Multiplier");
        
        _rigidbody2D = GetComponent<Rigidbody2D>();

        AddRandomToStopDistance();
    }

    private void MoveWithTransform(Vector2 direction)
    {
        transform.Translate(speed.Value * Time.deltaTime * direction);
        SetMoveAnimation(true);
    }
    
    public void MoveWithTransform(float direction)
    {
        if (direction == 0f)
        {
            SetMoveAnimation(false);
            return;
        }
        
        transform.position += new Vector3(direction, 0, 0) * (speed.Value * Time.deltaTime);
        SetMoveAnimation(true);
    }

    public void MoveWithVelocity(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition.x < transform.position.x ? -transform.right : transform.right;
        
        Vector2 targetVelocity = new Vector2(direction.x * (speed.Value * Time.fixedDeltaTime), _rigidbody2D.linearVelocity.y);
        _rigidbody2D.linearVelocity = Vector2.SmoothDamp(_rigidbody2D.linearVelocity, targetVelocity, ref _velocity, movementSmoothing);
        SetMoveAnimation(true);
    }

    private void MoveWithVelocity(float direction)
    {
        if (direction == 0f)
        {
            SetMoveAnimation(false);
            return;
        }
        
        Vector2 targetVelocity = new Vector2(direction * speed.Value * Time.fixedDeltaTime, _rigidbody2D.linearVelocity.y);
        _rigidbody2D.linearVelocity = Vector2.SmoothDamp(_rigidbody2D.linearVelocity, targetVelocity, ref _velocity, movementSmoothing);
        
        SetMoveAnimation(true);
    }
    
    public void Rotate(Vector2 direction)
    {
        _spriteTransform.rotation = direction.x > transform.position.x ?
            Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }
    
    public void Rotate(float direction)
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

    public void RotateAndMoveWithVelocity(Vector2 targetPosition)
    {
        Rotate(targetPosition);
        MoveWithVelocity(targetPosition);
    }
    
    public void RotateAndMoveWithVelocity(float direction)
    {
        Rotate(direction);
        MoveWithVelocity(direction);
    }
    
    public bool CloseEnough(Vector2 targetPosition, float targetHalfSize)
    {
        return Vector2.Distance(transform.position, targetPosition) <= stopDistance + targetHalfSize;
    }

    public void SetMoveAnimation(bool moveState)
    {
        _animator.SetBool(_animIDMove, moveState);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseSpeedServerRPC(float increaseSpeed)
    {
        IncreaseSpeed(increaseSpeed);
    }
    private void IncreaseSpeed(float increaseSpeed)
    {
        speed.Value += increaseSpeed;
        speedAnimationMultiplier.Value += 0.083f;
        _animator.SetFloat(_animIDMultiplier, speedAnimationMultiplier.Value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetSpeedServerRPC(float newSpeed)
    {
        SetSpeed(newSpeed);
    }
    private void SetSpeed(float newSpeed)
    {
        speed.Value = newSpeed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetSpeedAnimationMultiplierServerRPC(float newSpeedAnimMultiplier)
    {
        SetSpeedAnimationMultiplier(newSpeedAnimMultiplier);
    }
    private void SetSpeedAnimationMultiplier(float newSpeedAnimMultiplier)
    {
        speedAnimationMultiplier.Value = newSpeedAnimMultiplier;
        _animator.SetFloat(_animIDMultiplier, speedAnimationMultiplier.Value);
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

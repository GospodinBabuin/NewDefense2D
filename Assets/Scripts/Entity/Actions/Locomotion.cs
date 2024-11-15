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

    private void Awake()
    {
        _spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        
        _animator = GetComponent<Animator>();
        
        _animIDMove = Animator.StringToHash("IsMoving");

        AddRandomToStopDistance();
    }

    private void Move(Vector2 direction)
    {
        transform.Translate(speed * Time.deltaTime * direction);
        SetMoveAnimation(true);
    }
    
    public void Move(float direction)
    {
        if (direction == 0f)
        {
            SetMoveAnimation(false);
            return;
        }
        
        transform.position += new Vector3(direction, 0, 0) * (speed * Time.deltaTime);
        SetMoveAnimation(true);
    }
    
    private void Rotate(Vector2 direction)
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
            .WithParent(transform)
            .Play();
    }
}

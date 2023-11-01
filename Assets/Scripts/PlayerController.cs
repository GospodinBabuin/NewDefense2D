using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    [SerializeField] private float interactionRadius = 1.3f;
    [SerializeField] private GameObject interactionNotice;
    
    private InputReader _input;
    private Animator _animator;

    private bool _canInteract;

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
        Interact();
    }

    private void FixedUpdate()
    {
        CheckInteractionObjects();
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
            transform.rotation = _input.Move > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }

    private void CheckInteractionObjects()
    {
        Collider2D[] interactionObjects;
        interactionObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (interactionObjects == null) return;
        
        foreach (Collider2D interactionObject in interactionObjects)
        {
            if (interactionObject.GetComponent<IInteractable>() != null )
            {
                interactionNotice.SetActive(true);
                _canInteract = true;
                return;
            }
        }
        
        interactionNotice.SetActive(false);
        _canInteract = false;
    }

    private void Interact()
    {
        if (!_input.Interact || !_canInteract) return;

        Collider2D[] interactionObjects;
        interactionObjects = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        if (interactionObjects == null) return;
        
        foreach (Collider2D interactionObject in interactionObjects)
        {
            if (!interactionObject.TryGetComponent(out IInteractable interactable)) continue;
            interactable.Interact(this.gameObject);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

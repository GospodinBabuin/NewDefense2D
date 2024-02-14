using UI;
using UnityEngine;

public class PlayerController : Entity
{
    [SerializeField] private float interactionRadius = 1.3f;
    [SerializeField] private GameObject interactionNotice;
    
    private InputReader _input;

    private bool _canInteract;
    
    protected override void Start()
    {
        base.Start();

        ObjectsInWorld.Instance.AddPlayerToList(this);

        _input = GetComponent<InputReader>();
    }

    private void Update()
    {
        Rotate();
        Move();
        Attack();
        Interact();
    }

    private void FixedUpdate()
    {
        CheckInteractionObjects();
    }
    
    private void Move()
    {
        if (_input.Move == 0f)
        {
            Locomotion.SetMoveAnimation(false);
            return;
        }

        transform.position += new Vector3(_input.Move, 0, 0) * (Locomotion.GetSpeed() * Time.deltaTime);

        Locomotion.SetMoveAnimation(true);
    }

    private void Rotate()
    {
        if (_input.Move == 0) return;
        
        transform.rotation = _input.Move < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void Attack()
    {
        if (!_input.Attack) return;
        
        Combat.Attack();
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
        
        if (GameUI.Instance.IsMenuOpen())
            return;

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
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance.RemovePlayerFromList(this);
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

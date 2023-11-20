using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SortingGroup))]
public class Entity : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] protected float speed;

    protected virtual void Start()
    {
        _health = GetComponent<Health>();
        SetPositionOnGround();
    }

    protected virtual void Move(Vector2 direction)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    protected virtual void Rotate(Vector2 dierection)
    {
        transform.rotation = dierection.x < transform.position.x ?
            Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }

    protected void RotateAndMove(Vector2 dierection)
    {
        Rotate(dierection);

        Move(dierection.x < transform.position.x ? -transform.right : transform.right);
    }

    private void SetPositionOnGround()
    {
        LayerMask groundLayerMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, float.MaxValue, groundLayerMask);
        if (hit) transform.position = hit.point;
    }
}
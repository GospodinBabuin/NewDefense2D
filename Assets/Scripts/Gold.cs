using UnityEngine;

public class Gold : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactingObject)
    {
        interactingObject.GetComponentInChildren<GoldBank>().AddGold(this, 1);
        Destroy(gameObject);
    }
}

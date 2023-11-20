using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private Camera mainCamera;

    public float Paralax;
    private float _startPosX;


    private void Start()
    {
        mainCamera = Camera.main;
        _startPosX = transform.position.x;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(_startPosX + mainCamera.transform.position.x * (1 - Paralax), transform.position.y, transform.position.z);
    }
}

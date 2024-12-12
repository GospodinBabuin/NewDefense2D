using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Network
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField] private Transform player;  
        private AudioListener _audioListener;
        private Camera _camera;
        
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 clampOffset;
        [Range(0, 0.5f)] [SerializeField] private float smoothSpeed = 0.005f;
        [Range(1, 10)] [SerializeField] private float cameraDirectionXMultiplier = 5;
        [Range(1, 10)] [SerializeField] private float cameraDirectionYMultiplier = 2;

        [SerializeField] private float cameraOffset = 3f;
        [SerializeField] private float cameraMinSize = 3f;
        [SerializeField] private float cameraMaxSize = 3.5f;
        [Range(0, 3)] [SerializeField] private float cameraSizeMultiplier = 1;
        
        private Vector3 _targetPosition;
        private float _targetCameraSize;

        private void Awake()
        {
            _audioListener = GetComponentInParent<AudioListener>();
            _camera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            
            if (!IsOwner || sceneName == "Lobby")
            {
                _audioListener.enabled = false;
                _camera.gameObject.SetActive(false);
                enabled = false;
                return;
            }

            _audioListener.enabled = true;
            
            if (offset == Vector3.zero)
                offset = new Vector3(0, 2.1f, -50);
            
            if (clampOffset == Vector3.zero)
                clampOffset = new Vector3(0, 3.15f, 0);
            
            transform.position = offset;

            Cursor.lockState = CursorLockMode.Confined;
        }

        private void LateUpdate()
        {
            MoveCameraToCursor();
        }

        private void MoveCameraToCursor()
        {
            //Camera Transform
            if (Cursor.lockState != CursorLockMode.Confined) return;
            
            Vector3 cursorViewportPosition = _camera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            cursorViewportPosition.z = 0;
            
            float normalizedX = cursorViewportPosition.x * 2 - 1;
            float normalizedY = cursorViewportPosition.y * 2 - 1;
            
            Vector3 desiredPosition = new Vector3(normalizedX / 2 * cameraDirectionXMultiplier + offset.x, 
                normalizedY / 2 * cameraDirectionYMultiplier + offset.y, offset.z);
           
            _targetPosition = Vector3.Lerp(transform.localPosition, desiredPosition, smoothSpeed);

            float cameraHalfWidth = _camera.orthographicSize * _camera.aspect;

            float clampedX = Mathf.Clamp(_targetPosition.x, -clampOffset.x - cameraHalfWidth,
                clampOffset.x + cameraHalfWidth);
            float clampedY = Mathf.Clamp(_targetPosition.y, -clampOffset.y,
                clampOffset.y);
            
            transform.localPosition = new Vector3(_targetPosition.x, clampedY, transform.localPosition.z);
            
            //Camera Size
            float normalizedCameraSize = (Mathf.Abs(normalizedX) + Mathf.Abs(normalizedY)) / 2;
            
            float desiredCameraSize = normalizedCameraSize * cameraSizeMultiplier + cameraOffset;
            
            _targetCameraSize = Mathf.Lerp(_camera.orthographicSize, desiredCameraSize, smoothSpeed);
            
            float clampedCameraSize = Mathf.Clamp(_targetCameraSize, cameraMinSize,
                cameraMaxSize);
            
            _camera.orthographicSize = clampedCameraSize;
        }
    }
}

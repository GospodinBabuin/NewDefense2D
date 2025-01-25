using System;
using Environment;
using UI;
using UI.Menus;
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
        [Range(1, 5f)] [SerializeField] private float smoothSpeed = 4f;
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
            
            smoothSpeed = PlayerPrefs.GetFloat("CameraSensitivity");
            
            if (offset == Vector3.zero)
                offset = new Vector3(0, 2.1f, -50);
            
            if (clampOffset == Vector3.zero)
                clampOffset = new Vector3(0, 3.15f, 0);
            
            transform.position = offset;

            Cursor.lockState = CursorLockMode.Confined;
            
            WaterController.Instance?.Initialize(_camera);
            
            GameUI.Instance.SettingsMenu.MainSettings.OnCameraSensitivityChangedEvent += SetSmoothSpeed;
        }

        private void LateUpdate()
        {
            FindCameraTargetPosition();
        }

        private void FindCameraTargetPosition()
        {
            if (Cursor.lockState != CursorLockMode.Confined) return;

            Vector3 cursorViewportPosition = _camera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            cursorViewportPosition.z = 0;
            
            float normalizedX = cursorViewportPosition.x * 2 - 1;
            float normalizedY = cursorViewportPosition.y * 2 - 1;
            
            _targetPosition = new Vector3(normalizedX / 2 * cameraDirectionXMultiplier + offset.x, 
                normalizedY / 2 * cameraDirectionYMultiplier + offset.y, offset.z);
            
            float normalizedCameraSize = (Mathf.Abs(normalizedX) + Mathf.Abs(normalizedY)) / 2;
            
            float desiredCameraSize = normalizedCameraSize * cameraSizeMultiplier + cameraOffset;
            
            _targetCameraSize = Mathf.Clamp(desiredCameraSize, cameraMinSize,
                cameraMaxSize);
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, smoothSpeed * Time.fixedDeltaTime);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetCameraSize, smoothSpeed * Time.fixedDeltaTime);
        }

        private void SetSmoothSpeed(float newSmoothSpeed)
        {
            smoothSpeed = newSmoothSpeed;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (GameUI.Instance != null)
                GameUI.Instance.SettingsMenu.MainSettings.OnCameraSensitivityChangedEvent -= SetSmoothSpeed;
        }
    }
}

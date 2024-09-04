using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class PlayerCameraController : NetworkBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private AudioListener _audioListener;
        private Camera _camera;

        private void Awake()
        {
            _audioListener = GetComponentInChildren<AudioListener>();
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            _camera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            if (!IsOwner)
            {
                _audioListener.enabled = false;
                _virtualCamera.Priority = 0;
                _camera.gameObject.SetActive(false);
                enabled = false;
                return;
            }

            _audioListener.enabled = true;
            _virtualCamera.Priority = 100;
        }
    }
}

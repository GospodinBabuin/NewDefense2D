using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class PlayerCameraController : NetworkBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private AudioListener _audioListener;
        private Camera _camera;

        private void Awake()
        {
            _audioListener = GetComponentInParent<AudioListener>();
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            _camera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            
            if (!IsOwner || sceneName == "Lobby")
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

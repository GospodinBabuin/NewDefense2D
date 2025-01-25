using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment
{
    public class WaterController : MonoBehaviour
    {
        public static WaterController Instance;

        private Camera _camera;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void Initialize(Camera playerCamera)
        {
            if (SceneManager.GetActiveScene().name == "Lobby") return;

            _camera = playerCamera;
        }

        private void Update()
        {
            if (!_camera) return;
            
            transform.position = new Vector2(_camera.transform.position.x, transform.position.y);
        }
    }
}

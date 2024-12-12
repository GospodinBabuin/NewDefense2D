using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment
{
    public class WaterController : MonoBehaviour
    {
        public static WaterController Instance;

        private Transform _playerTransform;
        
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
        
        public void Initialize(Transform playerTransform)
        {
            if (SceneManager.GetActiveScene().name == "Lobby") return;

            _playerTransform = playerTransform;
        }

        private void Update()
        {
            if (!_playerTransform) return;
            
            transform.position = new Vector2(_playerTransform.position.x, transform.position.y);
        }
    }
}

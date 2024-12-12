using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField] private float parallaxEffect;
    
        private Camera _camera;
        private float _length;
        private float _startPosition;
        
        [SerializeField] private bool swapPlaces = true;

        public void Initialize(Camera camera)
        {
            _camera = camera;
            _startPosition = transform.position.x;
        
            if (swapPlaces && TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                _length = spriteRenderer.bounds.size.x;
            }
        }

        private void Update()
        {
            if (!_camera) return;
            
            float temp = _camera.transform.position.x * (1 - parallaxEffect);
            float distance = _camera.transform.position.x * parallaxEffect;

            transform.position = new Vector3(_startPosition + distance, transform.position.y, transform.position.z);

            if (!swapPlaces) return;
            if (temp > _startPosition + _length)
                _startPosition += _length;
            else if (temp < _startPosition - _length)
                _startPosition -= _length;
        }
    }
}

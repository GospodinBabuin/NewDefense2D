using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment
{
    public class Parallax : NetworkBehaviour
    {
        [SerializeField] private float parallaxEffect;
    
        private Camera _camera;
        private float _length;
        private float _startPosition;
        private bool _swapPlaces = false;

        public void Initialize(Camera camera)
        {
            _camera = camera;
            _startPosition = transform.position.x;
        
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                _length = spriteRenderer.bounds.size.x;
                _swapPlaces = true;
            }
            else
            {
                _swapPlaces = false;
            }
        }

        private void Update()
        {
            if (!_camera) return;
            
            float temp = _camera.transform.position.x * (1 - parallaxEffect);
            float distance = _camera.transform.position.x * parallaxEffect;

            transform.position = new Vector3(_startPosition + distance, transform.position.y, transform.position.z);

            if (!_swapPlaces) return;
            if (temp > _startPosition + _length)
                _startPosition += _length;
            else if (temp < _startPosition - _length)
                _startPosition -= _length;
        }
    }
}

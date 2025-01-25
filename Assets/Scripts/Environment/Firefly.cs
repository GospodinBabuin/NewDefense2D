using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Environment
{
    public class Firefly : MonoBehaviour
    {   
        [SerializeField] private float radius = 0.2f;
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float curveFrequency = 1f;
        [SerializeField] private float curveAmplitude = 0.5f;
        
        [SerializeField] private float randomRadius = 0.1f;
        [SerializeField] private float randomSpeed = 0.1f;
        
        private Vector2 _center;
        private Vector2 _targetPosition;
        private float _timeElapsed = 0f;
        
        private Animator _animator;
        private int _animIDLightOn;
        private int _animIDLightOff;
        private int _animIDIdle;
        private int _animIDMultiplier;
        [SerializeField] [Range(0.01f, 0.5f)] private float _animMultiplier;

        [SerializeField] private bool _isVisible = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animIDLightOn = Animator.StringToHash("LightOn");
            _animIDLightOff = Animator.StringToHash("LightOff");
            _animIDIdle = Animator.StringToHash("Idle");
            _animIDMultiplier = Animator.StringToHash("Multiplier");
        }

        private void FixedUpdate()
        {
            //MoveAlongCurve();
        }

        private void Initialize()
        {
            // _center = transform.position;
            // AddRandomToRadius();
            // AddRandomToSpeed();
            AddRandomToAnimMultiplier();
            // SetRandomTargetPosition();
        }
        
        private void MoveAlongCurve()
        {
            if (!_isVisible) return;
            
            Vector2 direction = _targetPosition - (Vector2)transform.position;
        
            if (direction.magnitude < 0.1f)  
            {
                SetRandomTargetPosition();  
                return;
            }
        
            _timeElapsed += Time.fixedDeltaTime;
            float curveOffsetX = Mathf.Sin(_timeElapsed * curveFrequency) * curveAmplitude;
            float curveOffsetY = Mathf.Cos(_timeElapsed * curveFrequency) * curveAmplitude;
        
            Vector2 movement = direction.normalized * (speed * Time.fixedDeltaTime);
        
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, movement.magnitude);
        
            transform.position += new Vector3(curveOffsetX, curveOffsetY, 0) * Time.fixedDeltaTime;
        }
        
        private void SetRandomTargetPosition()
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float distance = Random.Range(0f, radius);
            _targetPosition = _center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        }

        private void AddRandomToRadius()
        {
            radius += Random.Range(-randomRadius, randomRadius);
        }
        
        private void AddRandomToSpeed()
        {
            speed += Random.Range(-randomSpeed, randomSpeed);
        }
        
        private void AddRandomToAnimMultiplier()
        {
            _animMultiplier += Random.Range(0.05f, 0.1f);
            _animator.SetFloat(_animIDMultiplier, _animMultiplier);
        }

        private void OnEnable()
        {
            Initialize();
            _animator.SetTrigger(_animIDLightOn);
        }

        public void TurnInactive()
        {
            _animator.SetTrigger(_animIDLightOff);
        }

        public void OnLightOffAnimEventEnded()
        {
            transform.parent.gameObject.SetActive(false);
        }

        //private void OnBecameVisible()
        //{
        //    _isVisible = true;
        //}
        //
        //private void OnBecameInvisible()
        //{
        //    _isVisible = false;
        //}

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_center, radius);
        }
    }
}

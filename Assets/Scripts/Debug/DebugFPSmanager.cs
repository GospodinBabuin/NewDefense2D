using UnityEngine;

public class DebugFPSmanager : MonoBehaviour
{
    private float _deltaTime;
    private float _currentFps;
    private float _minFps = float.MaxValue;
    private float _maxFps = float.MinValue;

    [SerializeField] private bool enable = true;

    private void Update()
    {
        if (!enable) return;

        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        _currentFps = 1.0f / _deltaTime;

        if (_currentFps > _maxFps)
            _maxFps = _currentFps;

        if (_currentFps < _minFps)
            _minFps = _currentFps;
    }

    private void FixedUpdate()
    {
        if (!enable) return;

        Print();
    }

    private void Print()
    {
        Debug.Log(_currentFps);
        Debug.Log(_maxFps);
        Debug.Log(_minFps);
    }
}

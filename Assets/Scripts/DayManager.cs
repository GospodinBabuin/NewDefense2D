using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public enum DayState { DAY, EVENING, NIGHT }
    public DayState dayState { get; private set; } = DayState.DAY;

    private int DayCount;
    private float CurrentTime;
    private bool _globalLightIntensityOnPosition = false;

    [SerializeField] private Light2D GlobalLight;

    [SerializeField] private float GlobalLightIntensityChangeSpeed = 15f;

    [SerializeField] private float dayTimeGlobalLightIntensity = 1f;
    [SerializeField] private float eveningTimeGlobalLightIntensity = 0.65f;
    [SerializeField] private float nightTimeGlobalLightIntensity = 0.05f;

    [SerializeField] private int dayTime = 0;
    [SerializeField] private int eveningTime = 90;
    [SerializeField] private int nightTime = 180;
    [SerializeField] private int newDayTime = 360;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        ChangeDayTime();
    }

    private void ChangeDayTime()
    {
        CurrentTime += Time.deltaTime;

        if (CurrentTime >= eveningTime && dayState == DayState.DAY)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity -= Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity <= eveningTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = eveningTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.EVENING;
                }
            }
        }

        if (CurrentTime >= nightTime && dayState == DayState.EVENING)
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity -= Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity <= nightTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = nightTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.NIGHT;
                }
            }
        }

        if (CurrentTime >= newDayTime && dayState == DayState.NIGHT || WaveSpawner.Instance.IsMonstersDead())
        {
            _globalLightIntensityOnPosition = false;
            if (!_globalLightIntensityOnPosition)
            {
                GlobalLight.intensity += Time.deltaTime / GlobalLightIntensityChangeSpeed;

                if (GlobalLight.intensity >= dayTimeGlobalLightIntensity)
                {
                    GlobalLight.intensity = dayTimeGlobalLightIntensity;
                    _globalLightIntensityOnPosition = true;
                    dayState = DayState.DAY;
                    CurrentTime = dayTime;
                    DayCount++;
                }
            }
        }
    }
}

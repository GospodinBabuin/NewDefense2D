using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CameraTransitionHandler : MonoBehaviour
{
    public static CameraTransitionHandler Instance;
    
    private Animator _animator;
    private int _animIdFadeIn;
    private int _animIdFadeOut;

    private bool _isFading;
    
    public Camera BaseCamera => baseCamera;
    [SerializeField] private Camera baseCamera;

    
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

        _animator = GetComponent<Animator>();
        _animIdFadeIn = Animator.StringToHash("FadeIn");
        _animIdFadeOut = Animator.StringToHash("FadeOut");
        _animator.SetBool(_animIdFadeIn, false);
    }

    public IEnumerator Transit(Camera from, Camera to, float timeUntilReturn)
    {
        _animator.SetTrigger(_animIdFadeIn);

        while (!_isFading)
            yield return null;
        
        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);
        
        _animator.SetTrigger(_animIdFadeOut);
        
        if (timeUntilReturn == 0) yield break;
        
        while (!_isFading)
            yield return null;
        
        yield return new WaitForSeconds(timeUntilReturn);
        
        _animator.SetTrigger(_animIdFadeIn);

        while (!_isFading)
            yield return null;
        
        from.gameObject.SetActive(true);
        to.gameObject.SetActive(false);
        
        _animator.SetTrigger(_animIdFadeOut);
    }

    public void FadingStartedAnimEvent()
    {
        _isFading = true;
    }
    
    public void FadingEndedAnimEvent()
    {
        _isFading = false;
    }
}

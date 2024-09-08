using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : NetworkBehaviour
{
    public static SceneTransitionHandler Instance { get; private set; }
    
    private Animator _animator;
    private int _animIdFaded;


    private bool _isLoading;
    private bool _isFading;
    
    private Action _fadedInCallback;
    private Action _fadedOutCallback;

    private string _sceneToSwitchName;

    public delegate void FadeOutStarted(string sceneName);
    public event FadeOutStarted OnFadeOutStartedEvent;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _animator = GetComponent<Animator>();
        _animIdFaded = Animator.StringToHash("Faded");
        _animator.SetBool(_animIdFaded, false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += FadeOut;
    }

    private void FadeOut(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (_isFading)
            return;

        _isFading = true;
        _sceneToSwitchName = null;
        _animator.SetBool(_animIdFaded, false);
    }

    public void SwitchScene(string sceneName)
    {
        if (_isLoading) return;
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
            throw new Exception("scene already loaded");
        
        if (NetworkManager.Singleton.IsListening && sceneName != "MainMenu")
        {
            Debug.Log("NetworkManager transition");
            _sceneToSwitchName = sceneName;
            FadeInClientRPC();
        }
        else
        {
            Debug.Log("Coroutine transition");
            if (PlayerInfoHandler.Instance != null)
            {
                Destroy(PlayerInfoHandler.Instance.gameObject);
            }
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _isLoading = true;

        bool waitFading = true;
        FadeIn(() => waitFading = false);

        while (waitFading)
            yield return null;
        
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        
        waitFading = true;
        FadeOut(() => waitFading = false);

        while (waitFading)
            yield return null;

        _isLoading = false;
    }

    [ClientRpc(RequireOwnership = false)]
    private void FadeInClientRPC()
    {
        if (_isFading)
            return;
        
        _isFading = true;
        _animator.SetBool(_animIdFaded, true);
    }
    
    private void FadeIn(Action fadedInCallback)
    {
        if (_isFading)
            return;

        _isFading = true;
        _fadedInCallback = fadedInCallback;
        _animator.SetBool(_animIdFaded, true);
    }
    
    private void FadeOut(Action fadedOutCallback)
    {
        if (_isFading)
            return;

        _isFading = true;
        _fadedOutCallback = fadedOutCallback;
        _animator.SetBool(_animIdFaded, false);
    }
    
    public void CheckIfPlayersScreensAreFaded()
    {
        foreach (KeyValuePair<ulong,GameObject> player in PlayerInfoHandler.Instance.PlayerInfos)
        {
            if (!player.Value.GetComponent<PlayerInfo>().isPlayerScreenFaded)
            {
                return;
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene(_sceneToSwitchName, LoadSceneMode.Single);
    }

    public void OnFadeInAnimEnded()
    {
        _isFading = false;

        NetworkTransmission.Instance?.PlayersScreenFadedServerRPC(true, NetworkManager.Singleton.LocalClientId);
    }

    public void OnFadeOutAnimEnded()
    {
        _isFading = false;
        
        NetworkTransmission.Instance?.PlayersScreenFadedServerRPC(false, NetworkManager.Singleton.LocalClientId);
    }

    public void OnFadeInAnimEvent()
    {
        _fadedInCallback?.Invoke();
        _fadedInCallback = null;
        _isFading = false;
    }

    public void OnFadeOutAnimEvent()
    {
        _fadedOutCallback?.Invoke();
        _fadedOutCallback = null;
        _isFading = false;
    }

    public void OnFadeOutAnimEventStarted()
    {
        OnFadeOutStartedEvent?.Invoke(SceneManager.GetActiveScene().name);
    }

    public void OnInitSceneAnimEvent()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

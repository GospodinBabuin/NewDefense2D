using System.Collections.Generic;
using Environment;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParalaxManager : MonoBehaviour
{
    public static ParalaxManager Instance { get; private set; }

    [SerializeField] private List<Parallax> parallaxList;

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

    public void Initialize(Camera camera)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") return;
        
        foreach (Parallax parallax in parallaxList)
        {
            parallax.Initialize(camera);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using UnityEngine;

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
        foreach (Parallax parallax in parallaxList)
        {
            parallax.Initialize(camera);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Unit", fileName = "Unit")]
public class UnitScriptableObject : ScriptableObject
{
    public Sprite Sprite { get => sprite; private set => value = sprite; }
    public string Description { get => description; private set => value = description; }
    public int Cost { get => cost; private set => value = cost; }
    public GameObject Prefab { get => prefab; private set => value = prefab; }
    public int ID { get => id; private set => value = id; }

    [SerializeField] private Sprite sprite;
    [SerializeField] private string description;
    [SerializeField] private int cost;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int id;
}

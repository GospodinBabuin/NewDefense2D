using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {
        public List<MapInfo> Maps;
    }
}

[Serializable]
public struct MapInfo
{
    public Sprite MapSprite;
    public string MapName;
    public string SceneName;
}

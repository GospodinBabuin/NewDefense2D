using MenuSlots;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Databases/PotionDB", fileName = "PotionDB")]
public class PotionDatabaseSO : ScriptableObject
{
    public List<PotionScriptableObject> potionSO;
}

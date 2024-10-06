using System.Collections.Generic;
using MenuSlots;
using UnityEngine;

[CreateAssetMenu(menuName = "Databases/BuildingDB", fileName = "BuildingDB")]
public class BuildingDatabaseSO : ScriptableObject
{
    public List<BuildingScriptableObject> buildingSO;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Databases/UnitDB", fileName = "UnitDB")]
public class UnitDatabaseSO : ScriptableObject
{
    public List<UnitScriptableObject> unitSO;
}

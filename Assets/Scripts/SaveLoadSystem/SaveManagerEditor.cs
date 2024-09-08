using UnityEditor;
using UnityEngine;

namespace SaveLoadSystem
{
    [CustomEditor(typeof(SaveLoad))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SaveLoad saveLoad = (SaveLoad)target;
            string gameName = saveLoad.gameData.name;

            DrawDefaultInspector();

            if (GUILayout.Button("Save Game"))
            {
                saveLoad.SaveGame();
            }
            
            if (GUILayout.Button("Load Game"))
            {
                saveLoad.LoadGame(gameName);
            }
            
            if (GUILayout.Button("Delete Game"))
            {
                saveLoad.DeleteGame(gameName);
            }
            
        }
    }
}

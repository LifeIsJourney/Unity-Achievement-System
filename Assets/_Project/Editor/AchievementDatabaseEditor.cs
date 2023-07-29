using UnityEngine;
using UnityEditor;
using System.IO;

namespace AchievementSystem
{

    [CustomEditor(typeof(AchievementDatabase))]
    public class AchievementDatabaseEditor : Editor
    {

        private const string ENUM_NAME = "AchievementID";
        private const string ENUM_FILE_NAME = ENUM_NAME + ".cs";
        const string FOLDER_NAME = "Assets/";

        private AchievementDatabase database;

        private void OnEnable()
        {
            database = target as AchievementDatabase;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Enum", GUILayout.Height(30)))
            {
                GenerateEnum();
            }
            if (GUILayout.Button("ToJson", GUILayout.Height(30)))
            {
                Debug.Log(JsonUtility.ToJson(database));
            }
        }

        private void GenerateEnum()
        {
            string filePath = Path.Combine(Application.dataPath, ENUM_FILE_NAME);
            string code = "public enum " + ENUM_NAME + " {\n";
            foreach (Achievement achievement in database.achievements)
            {
                //TODO: Validate the id is proper format
                code += "\t" + achievement.id + ",\n";
            }
            code += "}\n";
            File.WriteAllText(filePath, code);
            AssetDatabase.ImportAsset(FOLDER_NAME + ENUM_FILE_NAME);
        }

    }

}
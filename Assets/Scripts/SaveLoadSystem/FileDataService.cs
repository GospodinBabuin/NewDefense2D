using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveLoadSystem
{
    public class FileDataService : IDataService
    {
        private ISerializer _serializer;
        private string _dataPath;
        private string _fileExtension;
        private string _saveName;

        public FileDataService(ISerializer serializer)
        {
            _dataPath = Application.persistentDataPath;
            _fileExtension = "json";
            _serializer = serializer;
            _saveName = "GameSave";
        }
        
        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(_saveName);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"file '{_saveName}.{_fileExtension}' already exists and cannot be overwritten");
            }
            
            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public GameData Load()
        {
            string fileLocation = GetPathToFile(_saveName);

            if (!File.Exists(fileLocation))
            {
                throw new ArgumentException($"no persisted GameData with name '{_saveName}'");
            }

            return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete()
        {
            string fileLocation = GetPathToFile(_saveName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(_dataPath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(_dataPath))
            {
                if (Path.GetExtension(path) == _fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        public bool IsSaveFileExists()
        {
            string fileLocation = GetPathToFile(_saveName);

            return File.Exists(fileLocation);
        }

        private string GetPathToFile(string fileName)
        {
            return Path.Combine(_dataPath, string.Concat(fileName, ".", _fileExtension));
        }
    }
}

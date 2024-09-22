using System.Collections.Generic;

namespace SaveLoadSystem
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load();
        void Delete();
        void DeleteAll();
        bool IsSaveFileExists();
        IEnumerable<string> ListSaves();
    }
}
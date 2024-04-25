using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Core;
using GameFramework.Manager;

namespace MainMenu
{
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        public async Task<bool> CreateGameLobby()
        {
            Dictionary<string, string> playerData = new Dictionary<string, string>()
            {
                { "GameTag", "HostPlayer" }
            };
            
            bool succeeded = await LobbyManager.Instance.CreateLobby(4, true, playerData);
            return succeeded; 
        }

        public string GetLobbyCode()
        {
            return LobbyManager.Instance.GetLobbyCode();
        }
    }
}

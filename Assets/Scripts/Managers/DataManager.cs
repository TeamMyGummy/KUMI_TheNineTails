using Util;

namespace Managers
{
    public static class DataManager
    {
        public static void Save(string key, GameState gameState)
        {
            JsonLoader.WriteDynamicData(key, gameState);
        }

        public static void Load(string key, out GameState gameState)
        {
            gameState = JsonLoader.ReadDynamicData<GameState>(key);
            if (gameState is null) 
                gameState = new();
        }
    }
}
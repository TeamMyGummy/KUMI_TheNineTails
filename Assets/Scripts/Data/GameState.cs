using Data;
using Newtonsoft.Json;
using Unity.VisualScripting;

public enum SaveKey{
    Player,
}

public class GameState
{
    [JsonProperty]
    private ASCState _ascState;

    public object Get(SaveKey key) => key switch
    {
        SaveKey.Player => _ascState,
        _ => null
    };
    
    public void Set(SaveKey key, object data) {
        switch(key) {
            case SaveKey.Player: _ascState = (ASCState)data; break;
        }
    }
}

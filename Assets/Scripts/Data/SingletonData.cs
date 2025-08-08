using Newtonsoft.Json;

public class SingletonData
{
    [JsonProperty] public LanternState LanternState = new();
    [JsonProperty] public BreakableWallState BreakableWallState = new();
}

using Newtonsoft.Json;

public class SingletonData
{
    [JsonProperty] public LanternState LanternState = new();
}

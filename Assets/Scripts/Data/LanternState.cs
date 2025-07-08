using System;
using System.Collections.Generic;

public class LanternState
{
    public HashSet<int> PassedCheckPoint = new();
    public int RecentCheckPoint = -1;
    public string RecentScene = String.Empty;
}

using System;
using System.Collections.Generic;

public class LanternState
{
    public HashSet<int> PassedCheckPoint = new();
    public int RecentCheckPoint = -1;
    public string RecentScene = String.Empty;
    
    public string RecentFloor = "층 정보";
    public string RecentSection = "구역 정보";
}

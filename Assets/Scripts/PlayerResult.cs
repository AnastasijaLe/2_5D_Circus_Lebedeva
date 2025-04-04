using System;

[Serializable]
public class PlayerResult
{
    public string playerName;
    public int points;
    public string finishTime;
    public string avatarSpriteName;

    public PlayerResult(string name, int points, string time, string avatarName)
    {
        playerName = name;
        this.points = points;
        finishTime = time;
        avatarSpriteName = avatarName;
    }
}
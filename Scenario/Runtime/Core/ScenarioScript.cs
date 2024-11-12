using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioScript
{
    public int ScriptID;
    public string SpeakerNameKey;
    public string SpeakerTitleKey;
    public string DialogKey;
    public ENextType NextType;
    public int TypeID;
    public List<ImageListInfo> ImageList;
    public string ImageBackground;
    public string BGM;
}

[System.Serializable]
public class ImageListInfo
{
    public string ImageName;
    public Vector3 Position;
    public Vector3 Scale;
    public bool IsDimming;
    public bool IsFlip;
    public EEmotionType EmotionType;
}

public enum ENextType
{
    End = -1,
    Script = 1,
    Choice = 2,
    Trigger = 3,
}

//프로토타입 데이터. 추후 변경
public enum EEmotionType
{
    Normal = -1,
    Happy = 0,
    Angry = 1,
    Suprise = 2,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncyclopediaEntry
{
    public string Title;
    public string TopicTag;
    public string MatchTag;
    public Sprite BackgroundImage;
    public ProgressPath UnlockPath;
    public List<EncyclopediaEntryText> EntryText;
}

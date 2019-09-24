using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncyclopediaSubEntry{

    public string Title;
    public string TopicTag;
    public string MatchTag;
    public Sprite BackgroundImage;
    public List<EncyclopediaEntryText> SubTopicText;
    public List<EncyclopediaEntry> SubEntries;
}

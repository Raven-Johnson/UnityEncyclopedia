using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class ListController : MonoBehaviour {

    GameObject ProgressManager;
    public GameObject ContentPanel;
    public GameObject SubTopicPanel;
    public GameObject EncyclopediaItemPrefab;
    public TextMeshProUGUI EntryTextDisplay;
    public GameObject ImagePanel;
    public List<EncyclopediaEntry> EncyclopediaEntries;
    

	// Use this for initialization
	void Start () {

        this.ProgressManager = GameObject.Find("Progress");

        ProgressObject progress = ProgressManager.GetComponent<ProgressObject>();
        EntryTextDisplay.text = String.Empty;

        foreach (EncyclopediaEntry entry in EncyclopediaEntries)
        {
            if (entry.UnlockPath.ChapterNum == 0 || progress.ProgressPaths.progressPaths.Any(i => i.ChapterNum == entry.UnlockPath.ChapterNum && i.SectionNum == entry.UnlockPath.SectionNum))
            {
                GameObject newEntry = Instantiate(EncyclopediaItemPrefab) as GameObject;
                TopicPanelProperties properties = newEntry.GetComponent<TopicPanelProperties>();
                properties.Name.text = entry.Title;
                properties.TopicTag = entry.TopicTag;
                properties.TopicText = entry.EntryText;
                properties.BackgroundImage = entry.BackgroundImage;
                properties.EntryTextDisplay = this.EntryTextDisplay;

                FillTopicList fillTopic = newEntry.GetComponent<FillTopicList>();
                fillTopic.ImagePanel = this.ImagePanel;
                fillTopic.ProgressManager = this.ProgressManager;
                fillTopic.SubTopicPanel = this.SubTopicPanel;

                newEntry.transform.SetParent(ContentPanel.transform, false);
            }
        }
    }
}


using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SubListController : MonoBehaviour {

    public GameObject EncyclopediaItemPrefab;
    public GameObject ContentPanel;
    public GameObject ImagePanel;
    public GameObject SubTopicPanel;
    public TextMeshProUGUI EntryTextDisplay;
    public List<EncyclopediaSubEntry> entries;
    GameObject ProgressManager;
    List<GameObject> panels;

    void Start()
    {
        this.ProgressManager = GameObject.Find("Progress");
        panels = new List<GameObject>();
    }

    public void UpdateContents(string topicTag)
    {
        foreach (EncyclopediaSubEntry entry in entries)
        {
            if (entry.MatchTag == topicTag)
            {
                foreach (EncyclopediaEntry subEntry in entry.SubEntries)
                {
                    GameObject newEntry = Instantiate(EncyclopediaItemPrefab) as GameObject;
                    TopicPanelProperties properties = newEntry.GetComponent<TopicPanelProperties>();
                    properties.Name.text = subEntry.Title;
                    properties.TopicTag = subEntry.TopicTag;
                    properties.MatchTag = subEntry.MatchTag;
                    properties.TopicText = subEntry.EntryText;
                    properties.EntryTextDisplay = EntryTextDisplay;
                    newEntry.transform.SetParent(ContentPanel.transform, false);

                    FillTopicList fillTopic = newEntry.GetComponent<FillTopicList>();
                    fillTopic.ImagePanel = this.ImagePanel;
                    fillTopic.ProgressManager = this.ProgressManager;

                    if (this.SubTopicPanel != null)
                    {
                        fillTopic.SubTopicPanel = this.SubTopicPanel;
                    }

                    panels.Add(newEntry);
                }
            }
        }
    }

    public void DestroyPanels()
    {
        if (SubTopicPanel != null)
        {
            SubListController subListController = SubTopicPanel.GetComponent<SubListController>();
            subListController.DestroyPanels();
        }

        foreach(GameObject panel in panels)
        {
            GameObject.Destroy(panel);
        }

        panels.Clear();
    }
}

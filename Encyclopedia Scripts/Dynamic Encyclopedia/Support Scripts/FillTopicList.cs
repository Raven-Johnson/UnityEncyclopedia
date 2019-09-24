using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;

public class FillTopicList : MonoBehaviour , IPointerClickHandler {

    public GameObject ProgressManager;
    public GameObject ImagePanel;
    public GameObject SubTopicPanel;
    TopicPanelProperties properties;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        properties = GetComponent<TopicPanelProperties>();

        #region SetSubTopics

        if (this.SubTopicPanel != null)
        {
            SubListController subList = SubTopicPanel.GetComponent<SubListController>();

            subList.DestroyPanels();
            subList.UpdateContents(properties.TopicTag);
        }

        #endregion

        #region SetImageandText
        Image entryImage = ImagePanel.GetComponent<Image>();
        entryImage.sprite = properties.BackgroundImage;

        ProgressObject progress = ProgressManager.GetComponent<ProgressObject>();
        properties.EntryTextDisplay.text = String.Empty;

        foreach (EncyclopediaEntryText entry in properties.TopicText)
        {
            if (entry.UnlockPath.ChapterNum == 0 || progress.ProgressPaths.progressPaths.Any(i => i.ChapterNum == entry.UnlockPath.ChapterNum && i.SectionNum == entry.UnlockPath.SectionNum))
            {
                try
                {
                    if (String.IsNullOrEmpty(properties.EntryTextDisplay.text))
                    {
                        properties.EntryTextDisplay.text = entry.EntryText.text;
                    }
                    else
                    {
                        properties.EntryTextDisplay.text += "\n\n" + entry.EntryText.text;
                    }
                }

                catch (NullReferenceException)
                {
                    properties.EntryTextDisplay.text = "Entry text is not set. Cannot be displayed.";
                }
            }
        }

        #endregion
    }
}

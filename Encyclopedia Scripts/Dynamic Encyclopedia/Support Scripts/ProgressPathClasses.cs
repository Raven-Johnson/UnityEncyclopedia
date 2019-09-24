using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressPath
{
    public int ChapterNum;
    public int SectionNum;

    public ProgressPath(int chapterNum, int sectionNum)
    {
        this.ChapterNum = chapterNum;
        this.SectionNum = sectionNum;
    }
}

[System.Serializable]
public class ProgressPaths
{
    public List<ProgressPath> progressPaths;
}
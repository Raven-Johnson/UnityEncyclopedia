using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fungus;

public class EncyclopediaButton : MonoBehaviour
{
    public Flowchart ButtonFlowchart;

    public void OpenEncyclopedia()
    {
        ButtonFlowchart.ExecuteBlock("Encyclopedia Button");
    }
}

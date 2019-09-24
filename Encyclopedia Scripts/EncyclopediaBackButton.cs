using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class EncyclopediaBackButton : MonoBehaviour {

    public Flowchart ButtonFlowchart;

    public void ReturntoMenu()
    {
        ButtonFlowchart.ExecuteBlock("Return to Menu Button");
    }
}

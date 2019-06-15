using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelControl : MonoBehaviour {

    public Button button;
    public GameObject panelToShow1, panelToShow2, panelActual;
    public Boolean openTwoPanels;

	// Use this for initialization
	void Start () {
        button.onClick.AddListener(openPanel);
    }
	
	void openPanel () {
        panelActual.gameObject.SetActive(false);
        panelToShow1.gameObject.SetActive(true);
        if (openTwoPanels)
            panelToShow2.gameObject.SetActive(true);
	}
}

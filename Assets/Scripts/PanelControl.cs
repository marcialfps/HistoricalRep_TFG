using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelControl : MonoBehaviour {

    public Button button;
    public GameObject panelToShow1, panelToShow2, panelActual;
    public Boolean openTwoPanels;

	/**
     * Close the actual panel and show the other.
     */
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

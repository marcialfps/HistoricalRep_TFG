using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelControl : MonoBehaviour {

    public Button button;
    public GameObject panel;
    public Boolean isOpen;

	// Use this for initialization
	void Start () {
        if (isOpen)
            button.onClick.AddListener(openPanel);
        else
            button.onClick.AddListener(closePanel);
    }
	
	void openPanel () {
        panel.gameObject.SetActive(true);
	}

    void closePanel()
    {
        panel.gameObject.SetActive(false);
    }
}

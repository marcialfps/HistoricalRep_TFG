using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigation : MonoBehaviour {

    public Button realVisitButton, virtualVisitButton, settingsButton;
    public GameObject panelMap, panelVirtualVisit, panelSettings;
    public RawImage realVisitImage, virtualVisitImage, settingsImage;

	/**
     * Change the navigation bar and active or desactive the panels depending on the
     * option. 
     */
	void Start () {
        realVisitButton.onClick.AddListener(delegate { activatePanel(realVisitButton, realVisitImage, panelMap, panelVirtualVisit, panelSettings); });
        virtualVisitButton.onClick.AddListener(delegate { activatePanel(virtualVisitButton, virtualVisitImage, panelVirtualVisit, panelMap, panelSettings); });
        settingsButton.onClick.AddListener(delegate { activatePanel(settingsButton, settingsImage, panelSettings, panelMap, panelVirtualVisit); });
    }
	
	private void activatePanel(Button b, RawImage i, GameObject panelActive, GameObject panel1, GameObject panel2)
    {
        panelActive.gameObject.SetActive(true);
        panel1.gameObject.SetActive(false);
        panel2.gameObject.SetActive(false);
        realVisitImage.color = Color.white;
        realVisitButton.GetComponentInChildren<Text>().color = Color.white;
        virtualVisitImage.color = Color.white;
        virtualVisitButton.GetComponentInChildren<Text>().color = Color.white;
        settingsImage.color = Color.white;
        settingsButton.GetComponentInChildren<Text>().color = Color.white;
        i.color = Color.yellow;
        b.GetComponentInChildren<Text>().color = Color.yellow;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageLocalData : MonoBehaviour {

    public Toggle modeARToggle, repNarrationToggle;
    public Dropdown languageDropdown;
    public Canvas canvasActual;

	/**
     * Configure all the options.
     */
	void Start () {
        configureToggleAR();
        configureDropdownLanguage();
        configureToggleNarration();
    }

    /* UI CONFIGURATION */

    private void configureToggleAR()
    {
        modeARToggle.onValueChanged.AddListener(delegate { saveARMode(); });
        if (loadARMode() == 1)
            modeARToggle.isOn = true;
        else
            modeARToggle.isOn = false;
    }

    private void configureDropdownLanguage()
    { 
        if (loadLanguage().Equals("English"))
            languageDropdown.value = 0;
        else
            languageDropdown.value = 1;

        languageDropdown.onValueChanged.AddListener(delegate { saveLanguage(); });
    }

    private void configureToggleNarration()
    {
        repNarrationToggle.onValueChanged.AddListener(delegate { saveNarration(); });
        if (loadNarration() == 1)
            repNarrationToggle.isOn = true;
        else
            repNarrationToggle.isOn = false;
    }

    /* DATA SAVE */

    // 1 is true and 0 is false
    private void saveARMode()
    {
        if(modeARToggle.isOn)
            PlayerPrefs.SetInt("AR_mode", 1);
        else
            PlayerPrefs.SetInt("AR_mode", 0);

        PlayerPrefs.Save();
    }

    private void saveNarration()
    {
        if (repNarrationToggle.isOn)
            PlayerPrefs.SetInt("Narration", 1);
        else
            PlayerPrefs.SetInt("Narration", 0);

        PlayerPrefs.Save();
    }

    /**
     * When language is changed, it reloads the scene and
     * desactive the canvas of the previous scene.
     */
    private void saveLanguage()
    {
        if (languageDropdown.value == 0)
        {
            PlayerPrefs.SetString("Language", "English");
        } else
        {
            PlayerPrefs.SetString("Language", "Spanish");
        }
        
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UnityEngine.Debug.Log(SceneManager.sceneCount);
        canvasActual.gameObject.SetActive(false);
    }

    /* DATA LOAD */

    private int loadARMode()
    {
        return PlayerPrefs.GetInt("AR_mode");
    }

    private string loadLanguage()
    {
        return PlayerPrefs.GetString("Language");
    }

    private int loadNarration()
    {
        return PlayerPrefs.GetInt("Narration");
    }
}

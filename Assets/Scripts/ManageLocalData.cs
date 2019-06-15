using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageLocalData : MonoBehaviour {

    public Toggle modeARToggle, repNarrationToggle;
    public Dropdown languageDropdown;

	// Use this for initialization
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

    private void saveLanguageEng()
    {
        PlayerPrefs.SetString("Language", "English");
        PlayerPrefs.Save();
    }

    private void saveLanguageSpa()
    {
        PlayerPrefs.SetString("Language", "Spanish");
        PlayerPrefs.Save();
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

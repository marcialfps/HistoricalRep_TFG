using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeech : MonoBehaviour {

    public AudioSource audioSource;
    private string text;

    /**
     * If the narration is activated, it calls the API.
     */
    public void launchTTS(string text, AudioSource audioSource)
    {
        if (PlayerPrefs.GetInt("Narration") == 1)
        {
            UnityEngine.Debug.Log(text);
            this.audioSource = audioSource;
            this.text = text;
            StartCoroutine(obtainAudio());
        }
    }
	
    /**
     * It calls the API with two different urls depending on the app language.
     * It obtains the audio from the response and plays it.
     */
	private IEnumerator obtainAudio()
    {
        string url;
        if (PlayerPrefs.GetString("Language").Equals("English"))
        {
            url = "http://api.voicerss.org/?key=3be5a7645c324a47bdee2ec2f717d15a&hl=en-us&src=" + text;
        } else
        {
            url = "http://api.voicerss.org/?key=3be5a7645c324a47bdee2ec2f717d15a&hl=es-es&src=" + text;
        }
        
        UnityEngine.Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;

        audioSource.clip = www.GetAudioClip(false, true, AudioType.WAV);
        audioSource.Play();
    }
}

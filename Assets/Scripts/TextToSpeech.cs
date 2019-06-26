using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeech : MonoBehaviour {

    public AudioSource audioSource;
    private string text;

    public void launchTTS(string text, AudioSource audioSource)
    {
        UnityEngine.Debug.Log(text);
        this.audioSource = audioSource;
        this.text = text;
        StartCoroutine(obtainAudio());
    }
	
	private IEnumerator obtainAudio()
    {
        string url = "http://api.voicerss.org/?key=3be5a7645c324a47bdee2ec2f717d15a&hl=es-es&src="+text;
        UnityEngine.Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;

        audioSource.clip = www.GetAudioClip(false, true, AudioType.WAV);
        audioSource.Play();
    }
}

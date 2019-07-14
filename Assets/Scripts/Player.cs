using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Player : MonoBehaviour {

    public Button playpauseBtn;
    public RawImage playpause;
    public Texture play, pause;
    public VideoPlayer videoplayer;
    public AudioSource audioSource;

	void Start() {
        playpauseBtn.onClick.AddListener(pauseVideo);
    }
	
	void playVideo() {
        videoplayer.Play();
        audioSource.Play();
        playpauseBtn.onClick.AddListener(pauseVideo);
        playpause.texture = pause;
    }

    void pauseVideo()
    {
        videoplayer.Pause();
        audioSource.Pause();
        playpauseBtn.onClick.AddListener(playVideo);
        playpause.texture = play;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Player : MonoBehaviour {

    public Button playpauseBtn, cancelBtn, stopBtn;
    public RawImage playpause, cancel, stop;
    public Texture play, pause;
    public VideoPlayer videoplayer;

	void Start() {
        playpauseBtn.onClick.AddListener(pauseVideo);
        cancelBtn.onClick.AddListener(cancelVideo);
        stopBtn.onClick.AddListener(stopVideo);
    }
	
	void playVideo() {
        videoplayer.Play();
        playpauseBtn.onClick.AddListener(pauseVideo);
        playpause.texture = pause;
    }

    void pauseVideo()
    {
        videoplayer.Pause();
        playpauseBtn.onClick.AddListener(playVideo);
        playpause.texture = play;
    }

    void cancelVideo()
    {
        //TO-DO
    }

    void stopVideo()
    {
        videoplayer.Stop();
    }
}

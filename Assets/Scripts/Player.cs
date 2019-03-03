using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Player : MonoBehaviour {

    public Button playpauseBtn, cancelBtn, stopBtn;
    public VideoPlayer videoplayer;

	void Start() {
        playpauseBtn.onClick.AddListener(playVideo);
        cancelBtn.onClick.AddListener(cancelVideo);
        stopBtn.onClick.AddListener(stopVideo);
    }
	
	void playVideo() {
        videoplayer.Play();
        playpauseBtn.onClick.AddListener(pauseVideo);
    }

    void pauseVideo()
    {
        videoplayer.Pause();
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

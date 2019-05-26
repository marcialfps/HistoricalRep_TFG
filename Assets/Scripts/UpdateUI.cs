using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UpdateUI : MonoBehaviour {

    public Text title;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;
    private ArrayList locations;

    public UpdateUI()
    {
        locations = new ArrayList();
        locations.Add(new Coordinates(43.360481, -5.842514)); //Escuela
        locations.Add(new Coordinates(43.354561, -5.852249)); //San Gregorio
        locations.Add(new Coordinates(43.353989, -5.853267)); //América
        locations.Add(new Coordinates(29.054405, -13.633832)); //Tiagua
    }

    private void Update()
    {
        var coordactual = new Coordinates(GPS.Instance.latitude, GPS.Instance.longitude);

        var distancia1 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[0], coordactual);
        var distancia2 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[1], coordactual);
        var distancia3 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[2], coordactual);

        var distancia4 = CoordinatesDistanceExtensions.DistanceTo((Coordinates)locations[3], coordactual);

        

        foreach(Coordinates c in locations)
        {
            // TO-DO
        }
        
        if (distancia1 < 20)
        {
            title.text = "Escuela de Ing. Informática";
            reproduceVideo("https://ak8.picdn.net/shutterstock/videos/13579628/preview/stock-footage-playing-jumping-little-girl-in-white-with-red-dress-enjoys-game-footage-with-alpha-channel-file.webm");
        }
        else if (distancia2 < 8)
        {
            title.text = "Colegio Mayor San Gregorio";
            reproduceVideo("https://archive.org/download/ElephantsDream/ed_1024_512kb.mp4");
        }
        else // if (distancia3 < 8)
        {
            title.text = "Colegio Mayor América";
            reproduceVideo("https://ak8.picdn.net/shutterstock/videos/13579628/preview/stock-footage-playing-jumping-little-girl-in-white-with-red-dress-enjoys-game-footage-with-alpha-channel-file.webm");
        }
        //else //if (distancia4 < 800)
        /*{
            title.text = "Tiagua";
            //reproduceVideo("file://" + Application.dataPath + "/Videos/prueba.mov");
        }*/
       /* else
        {
            title.text = "";
            videoPlayer.Stop();
            renderer.enabled = false;
        }*/
    }

    private void reproduceVideo(String url)
    {
        renderer.enabled = true;
        if (videoPlayer.url != url) videoPlayer.Stop(); //Casos en lo que se cambie de video
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.url = url;
            UnityEngine.Debug.Log("Puesta url");
            videoPlayer.Play();
        }
    }
}

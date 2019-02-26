using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UpdateGPSText : MonoBehaviour {

    public Text coordinates, title;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;
    private ArrayList locations;

    public UpdateGPSText()
    {
        locations = new ArrayList();
        locations.Add(new Coordinates(43.355078, -5.851307)); //Escuela
        locations.Add(new Coordinates(43.354561, -5.852249)); //San Gregorio
        locations.Add(new Coordinates(43.353989, -5.853267)); //América
    }

    private void Update()
    {
        var coordactual = new Coordinates(GPS.Instance.latitude, GPS.Instance.longitude);

        var distancia1 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[0], coordactual);
        var distancia2 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[1], coordactual);
        var distancia3 = CoordinatesDistanceExtensions.DistanceTo((Coordinates) locations[2], coordactual);


        coordinates.text = "Lat: " + GPS.Instance.latitude + "\nLon: " + GPS.Instance.longitude 
            + "\nDistancia escuela: " + Math.Round(distancia1,2) + " m."
            + "\nDistancia gregorio: " + Math.Round(distancia2,2) + " m."
            + "\nDistancia américa: " + Math.Round(distancia3,2) + " m.";

        foreach(Coordinates c in locations)
        {
            // TO-DO
        }
        
        if (distancia1 < 8)
        {
            title.text = "Escuela de Ing. Informática";
            reproduceVideo("https://archive.org/download/BigBuckBunny_124/Content/big_buck_bunny_720p_surround.mp4");
        }
        else if (distancia2 < 8)
        {
            title.text = "Colegio Mayor San Gregorio";
            reproduceVideo("https://archive.org/download/ElephantsDream/ed_1024_512kb.mp4");
        }
        else if (distancia3 < 8)
        {
            title.text = "Colegio Mayor América";
            reproduceVideo("https://media.w3.org/2010/05/sintel/trailer.mp4");
        }
        else
        {
            title.text = "";
            videoPlayer.Stop();
            renderer.enabled = false;
        }
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

public class Coordinates
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}
public static class CoordinatesDistanceExtensions
{
    public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates)
    {
        return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
    }

    public static double DistanceTo(this Coordinates baseCoordinates, Coordinates targetCoordinates, UnitOfLength unitOfLength)
    {
        var baseRad = Math.PI * baseCoordinates.Latitude / 180;
        var targetRad = Math.PI * targetCoordinates.Latitude / 180;
        var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
        var thetaRad = Math.PI * theta / 180;

        double dist =
            Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
            Math.Cos(targetRad) * Math.Cos(thetaRad);
        dist = Math.Acos(dist);

        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;

        return unitOfLength.ConvertFromMiles(dist) * 1000; //To meters
    }
}

public class UnitOfLength
{
    public static UnitOfLength Kilometers = new UnitOfLength(1.609344);
    public static UnitOfLength NauticalMiles = new UnitOfLength(0.8684);
    public static UnitOfLength Miles = new UnitOfLength(1);

    private readonly double _fromMilesFactor;

    private UnitOfLength(double fromMilesFactor)
    {
        _fromMilesFactor = fromMilesFactor;
    }

    public double ConvertFromMiles(double input)
    {
        return input * _fromMilesFactor;
    }
}
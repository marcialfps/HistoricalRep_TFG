using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UpdateGPSText : MonoBehaviour {

    public Text coordinates;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
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
            + "\nDistancia escuela: " + distancia1 + " m."
            + "\nDistancia gregorio: " + distancia2 + " m."
            + "\nDistancia américa: " + distancia3 + " m.";

        foreach(Coordinates c in locations)
        {
            // TO-DO
        }
        
        if (distancia1 < 920)
        {
            reproduceVideo("http://techslides.com/demos/sample-videos/small.mp4");
        }
        else if (distancia2 < 1020)
        {
            reproduceVideo("http://clips.vorwaerts-gmbh.de/VfE_html5.mp4");
        }
        else if (distancia3 < 1120)
        {
            reproduceVideo("http://mirrors.standaloneinstaller.com/video-sample/Panasonic_HDC_TM_700_P_50i.avi");
        }
        else
        {
            videoPlayer.Stop();
        }
    }

    private void reproduceVideo(String url)
    {
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
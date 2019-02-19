using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UpdateGPSText : MonoBehaviour {

    public Text coordinates;
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    private ArrayList locations;

    public UpdateGPSText()
    {
        locations = new ArrayList();
    }

    private void Update()
    {
        var coord1 = new Coordinates(43.354699, -5.851079); //Escuela
        var coord2 = new Coordinates(43.354561, -5.852249); //San Gregorio
        var coord3 = new Coordinates(43.353989, -5.853267); //America

        var coordactual = new Coordinates(GPS.Instance.latitude, GPS.Instance.longitude);

        var distancia1 = CoordinatesDistanceExtensions.DistanceTo(coord1, coordactual);
        var distancia2 = CoordinatesDistanceExtensions.DistanceTo(coord2, coordactual);
        var distancia3 = CoordinatesDistanceExtensions.DistanceTo(coord3, coordactual);


        coordinates.text = "Lat: " + GPS.Instance.latitude + "\nLon: " + GPS.Instance.longitude 
            + "\nDistancia escuela: " + distancia1 + " m."
            + "\nDistancia gregorio: " + distancia2 + " m."
            + "\nDistancia américa: " + distancia3 + " m.";

        if (distancia1 < 5)
        {
            rawImage.enabled = true;
            videoPlayer.url = "http://clips.vorwaerts-gmbh.de/VfE_html5.mp4";
            StartCoroutine(playVideo());
        }
        else if (distancia2 < 5)
        {
            rawImage.enabled = true;
            videoPlayer.url = "http://techslides.com/demos/sample-videos/small.mp4";
            StartCoroutine(playVideo());
        }
        else if (distancia3 < 5)
        {
            rawImage.enabled = true;
            videoPlayer.url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4";
            StartCoroutine(playVideo());
        }
        else
        {
            rawImage.enabled = false;
            videoPlayer.Stop();
        }
    }

    IEnumerator playVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }

        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();
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
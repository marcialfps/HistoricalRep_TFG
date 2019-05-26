using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class UpdateGPS : MonoBehaviour
{
    private ArrayList locations;
    string exampleUrl1 = "http://maps.googleapis.com/maps/api/staticmap?center=";
    string exampleUrl2 = "&zoom=13&size=600x300&maptype=roadmap&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    string key = "&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    public Text coordinates, title;
    public Button showButton, cancelButton;
    public GameObject panelShow, panelRepresentation, panelMap, arCamera, representationScreen;

    public UpdateGPS()
    {
        locations = new ArrayList();
        locations.Add(new Coordinates(43.360481, -5.842514)); //Escuela
        locations.Add(new Coordinates(43.354561, -5.852249)); //San Gregorio
        locations.Add(new Coordinates(43.353989, -5.853267)); //América
        locations.Add(new Coordinates(43.360833, -5.842222)); //Tiagua
    }

    private void Update()
    {
        var coordactual = new Coordinates(GPS.Instance.latitude, GPS.Instance.longitude);

        var distancia1 = CoordinatesDistanceExtensions.DistanceTo((Coordinates)locations[0], coordactual);
        var distancia2 = CoordinatesDistanceExtensions.DistanceTo((Coordinates)locations[1], coordactual);
        var distancia3 = CoordinatesDistanceExtensions.DistanceTo((Coordinates)locations[2], coordactual);
        var distancia4 = CoordinatesDistanceExtensions.DistanceTo((Coordinates)locations[3], coordactual);

        coordinates.text = "Lat: " + GPS.Instance.latitude + "\nLon: " + GPS.Instance.longitude;

        foreach (Coordinates c in locations)
        {
            // TO-DO
        }

        if (!panelShow.gameObject.activeInHierarchy && !panelRepresentation.gameObject.activeInHierarchy) { 
            if (distancia1 < 20)
            {
                title.text = "Escuela de Ing. Informática";
                panelShow.gameObject.SetActive(true);
                showButton.onClick.AddListener(showScene);
            }
            else if (distancia2 < 8)
            {
                title.text = "Colegio Mayor San Gregorio";
                panelShow.gameObject.SetActive(true);
                showButton.onClick.AddListener(showScene);
            }
            else if (distancia3 < 8)
            {
                title.text = "Colegio Mayor América";
                panelShow.gameObject.SetActive(true);
                showButton.onClick.AddListener(showScene);
            }
            else /*if (distancia4 < 10)*/
            {
                title.text = "Tiagua";
                panelShow.gameObject.SetActive(true);
                showButton.onClick.AddListener(showScene);
            }

        }
        /* else
         {
             title.text = "";
             videoPlayer.Stop();
             renderer.enabled = false;
         }*/
    }

    private void showScene()
    {
        panelRepresentation.gameObject.SetActive(true);
        arCamera.SetActive(true);
        representationScreen.SetActive(true);
        panelShow.gameObject.SetActive(false);
        panelMap.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(configureCancelButton);
    }

    private void configureCancelButton()
    {
        panelRepresentation.gameObject.SetActive(false);
        arCamera.SetActive(false);
        representationScreen.SetActive(false);
        panelMap.gameObject.SetActive(true);
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

/* GOOGLE MAPS */
public class MyGoogleMaps : MonoBehaviour
{

    string exampleUrl1 = "http://maps.googleapis.com/maps/api/staticmap?center=";
    string exampleUrl2 = "&zoom=13&size=600x300&maptype=roadmap&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    string key = "&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    public RawImage image;

    IEnumerator Start()
    {
        if (GPS.Instance != null)
        {
            WWW www = new WWW(exampleUrl1 + GPS.Instance.latitude + "," + GPS.Instance.longitude + exampleUrl2 + key);
            UnityEngine.Debug.Log(exampleUrl1 + GPS.Instance.latitude + "," + GPS.Instance.longitude + exampleUrl2 + key);
            yield return www;
            image.texture = www.texture;
        }
    }
}
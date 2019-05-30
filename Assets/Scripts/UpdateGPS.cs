using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class UpdateGPS : MonoBehaviour
{
    private String serverUrl = "http://192.168.1.103:8080";
    private ArrayList locations;
    private Representation actualRep;
    private Boolean isShowing;
    string exampleUrl1 = "http://maps.googleapis.com/maps/api/staticmap?center=";
    string exampleUrl2 = "&zoom=13&size=600x300&maptype=roadmap&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    string key = "&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";


    //User interface
    public Text coordinates, title, titleRep;
    public Button showButton, cancelButton, nearLocation1, nearLocation2, nearLocation3;
    public GameObject panelShow, panelRepresentation, panelMap, arCamera, representationScreen;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;

    public UpdateGPS()
    {
        locations = new ArrayList();
        // Obtain locations
        obtainLocations();
    }

    private void Update()
    {
        var coordactual = new Location(GPS.Instance.latitude, GPS.Instance.longitude, 0);
        coordinates.text = "Lat: " + GPS.Instance.latitude + "\nLon: " + GPS.Instance.longitude;

        if (!isShowing)
        {
            foreach (Location l in locations)
            {
                var distance = CoordinatesDistanceExtensions.DistanceTo(l, coordactual);
                UnityEngine.Debug.Log("Distance: " + distance);
                if (distance < 485559 && actualRep == null && !isShowing) // less 10 meters show
                {
                    UnityEngine.Debug.Log("Location detected");
                    obtainAllInfo(l.id);
                    obtainRepVideo(l.id);
                    isShowing = true;
                }
                else //if (distance < 5000000) // less 30 meters show as near location
                {
                    UnityEngine.Debug.Log("Near location detected");
                    obtainRepImage(l.id);
                }
                /*else
                {
                    actualRep = null;
                    title.text = "";
                    videoPlayer.Stop();
                    renderer.enabled = false;
                }*/
            }
        }
    }

    private void configureUI()
    {
        UnityEngine.Debug.Log("Configuring UI: "+actualRep.title);
        title.text = actualRep.title;
        panelShow.gameObject.SetActive(true);
        showButton.onClick.AddListener(showScene);
    }

    private void obtainLocations()
    {
        UnityEngine.Debug.Log("Creating request");
        WebRequest wrGET = WebRequest.Create(serverUrl+ "/location/list");

        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        parseLocation(data);
    }

    private void obtainAllInfo(long id)
    {
        UnityEngine.Debug.Log("Creating request");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/"+id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        parseRepresentationInfo(data);
    }

    private void obtainRepVideo(long id)
    {
        UnityEngine.Debug.Log("Creating request");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/video/" + id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        actualRep.videoURL = serverUrl + data;
        UnityEngine.Debug.Log(actualRep.videoURL);
        configureUI();
    }

    private void obtainRepImage(long id)
    {
        UnityEngine.Debug.Log("Creating request");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/image/" + id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        configureNearButton(serverUrl+data);
    }

    private IEnumerator configureNearButton(String image)
    {
        UnityEngine.Debug.Log(image);
        WWW www = new WWW(image);
        yield return www;
        nearLocation1.image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        nearLocation1.gameObject.SetActive(true);
    }

    private void parseLocation(String data)
    {
        // Remove [ ]
        String data2 = data.Trim(new Char[] { '[',']' });

        foreach (String l in data2.Split(','))
        {
            // Remove { }
            String l1 = l.Trim(new Char[] { '{', '}', ' '});
            UnityEngine.Debug.Log(l1);

            string[] aux = l1.Split(';');
            double lat = Convert.ToDouble(aux[0]);
            double lon = Convert.ToDouble(aux[1]);
            long id = Convert.ToInt64(aux[2]);

            this.locations.Add(new Location(lat, lon, id));
        }
    }

    private void parseRepresentationInfo(String data)
    {
        // Remove { }
        String data2 = data.Trim(new Char[] { '{', '}' });

        string[] l = data2.Split(',');
        long id = Convert.ToInt64(l[0].Split(':')[1]);
        string title = l[1].Split(':')[1];
        string description = l[2].Split(':')[1];
        string history = l[3].Split(':')[1];
        string interestInfo = l[4].Split(':')[1];
        string technicalInfo = l[5].Split(':')[1];
        double latitude = Convert.ToDouble(l[6].Split(':')[1]);
        double longitude = Convert.ToDouble(l[6].Split(':')[1]);

        this.actualRep = new Representation(id, title, description, history, interestInfo, technicalInfo, latitude, longitude);
    }

    private void showScene()
    {
        panelRepresentation.gameObject.SetActive(true);
        arCamera.SetActive(true);
        representationScreen.SetActive(true);
        panelShow.gameObject.SetActive(false);
        panelMap.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(configureCancelButton);
        titleRep.text = actualRep.title;
        reproduceVideo(actualRep.videoURL);
    }

    private void configureCancelButton()
    {
        panelRepresentation.gameObject.SetActive(false);
        arCamera.SetActive(false);
        representationScreen.SetActive(false);
        panelMap.gameObject.SetActive(true);
        isShowing = false;
        actualRep = null;
    }

    private void reproduceVideo(String url)
    {
        renderer.enabled = true;
        if (videoPlayer.url != url) videoPlayer.Stop(); //Casos en lo que se cambie de video
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.url = url;
            UnityEngine.Debug.Log("Añadida url a videoplayer: "+url);
            videoPlayer.Play();
        }
    }
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long id { get; set; }

    public Location(double latitude, double longitude, long id)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.id = id;
    }
}

public class Representation
{
    public long id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string history { get; set; }
    public string interestInfo { get; set; }
    public string technicalInfo { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string videoURL { get; set; }

    public Representation(long id, string title, string des, string hist, string interest, string techn, double lat, double longi)
    {
        this.id = id;
        this.title = title;
        this.description = des;
        this.history = hist;
        this.technicalInfo = techn;
        this.Latitude = lat;
        this.Longitude = longi;
    }
}

public static class CoordinatesDistanceExtensions
{
    public static double DistanceTo(this Location baseCoordinates, Location targetCoordinates)
    {
        return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometers);
    }

    public static double DistanceTo(this Location baseCoordinates, Location targetCoordinates, UnitOfLength unitOfLength)
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
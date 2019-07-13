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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


public class UpdateGPS : MonoBehaviour
{
    
    /**
     * URLs of the server and the maps API.
     */
    private String serverUrl = "https://serverhistrep.herokuapp.com";
    string urlMaps1 = "http://maps.googleapis.com/maps/api/staticmap?center=";
    string urlMaps2 = "&markers=color:blue%7Clabel:You%7C";
    string urlMaps3 = "&zoom=17&size=700x500&maptype=roadmap&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";

    /**
     * Private fields of the Script.
     */
    private ArrayList locations, nearLocations;
    private Representation actualRep;
    private Boolean isShowing;
    private Translator translator = new Translator();


    /**
     * UI elements.
     */
    public Text title, titleRep, titleInfo, contentInformation, textNoNearLocations;
    public Button showButton, cancelButton, descriptionButton, historyButton, interestInfo, technicalInfo;
    public GameObject panelOptions, panelShow, panelRepresentation, panelMap, arCamera, camera, representationScreen,
        maskNearLocation1, marskNearLocation2, maskActualLocation;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;
    public Image map;

    /**
     * When created, the private fields are initialized.
     */
    public UpdateGPS()
    {
        locations = new ArrayList();
        nearLocations = new ArrayList();
    }

    /**
     * When started, first we obtain all the locations from the server and launch the process
     * of refresh the location.
     */
    private void Start()
    {
        obtainLocations();
        StartCoroutine(refreshLocation());
    }

    /**
     * This process is executed each 10 seconds. First, the location of the device is updated,
     * after that, we obtain the map. Later, the go through the locations and we configure the 
     * UI depending if we have a location to show or a near location. 
     */
    private IEnumerator refreshLocation()
    {
        while (true) {
            UnityEngine.Debug.Log("Refreshing location of the device.");
            var coordactual = new Location(GPS.Instance.latitude, GPS.Instance.longitude, 0);
            StartCoroutine("obtainMap");

            foreach (Location l in locations)
            {
                var distance = CoordinatesDistanceExtensions.DistanceTo(l, coordactual);
                if (distance < 200000000 && actualRep == null) // less 20 meters show
                {
                    UnityEngine.Debug.Log("Location detected at "+distance+" meters.");
                    obtainAllInfo(l.id);
                    obtainRepVideo(l.id);
                    obtainRepImage(l.id);
                    isShowing = true;
                }
                else if (distance < 2000000) // less 200 meters show as near location
                {
                    UnityEngine.Debug.Log("Near location detected at "+distance+" meters.");
                    obtainLocationImage(l);
                    nearLocations.Add(l);
                    l.distance = distance;
                }
                else if (!isShowing)
                {
                    actualRep = null;
                }
            }
                
            configureNearLocations();
            if (actualRep == null)
            {
                panelShow.SetActive(false);
                videoPlayer.Stop();
                renderer.enabled = false;
            } else
            {
                configureUI();
            }

            yield return new WaitForSeconds(10.0f); //Wait 10 seconds
        }
    }

    private void configureUI()
    {
        UnityEngine.Debug.Log("Configuring UI: "+actualRep.title);
        title.text = translator.translate(actualRep.title);
        panelShow.gameObject.SetActive(true);
        showButton.onClick.AddListener(showScene);
    }

    private void obtainLocations()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        UnityEngine.Debug.Log("Creating request obtain locations");
        WebRequest wrGET = WebRequest.Create(serverUrl+ "/location/list");

        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        List<Location> deserializedList = JsonConvert.DeserializeObject<List<Location>>(data);
        this.locations = new ArrayList(deserializedList);
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    private void obtainAllInfo(long id)
    {
        UnityEngine.Debug.Log("Creating request obtain all info of representation "+id+".");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/"+id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        List<Representation> deserializedList = JsonConvert.DeserializeObject<List<Representation>>(data);
        this.actualRep = deserializedList[0];
    }

    private void obtainRepVideo(long id)
    {
        UnityEngine.Debug.Log("Creating request obtain video of representation "+id+".");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/video/" + id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        actualRep.videoURL = serverUrl + data;
        UnityEngine.Debug.Log(actualRep.videoURL);
    }

    private void obtainRepImage(long id)
    {
        UnityEngine.Debug.Log("Creating request obtain image of representation " + id + ".");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/image/" + id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        actualRep.imageURL = serverUrl + data;
        configureUI();
    }

    private void obtainLocationImage(Location l)
    {
        UnityEngine.Debug.Log("Creating request obtain image of near representation "+l.id+".");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/image/" + l.id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        l.image = data;
    }

    private void configureNearLocations()
    {
        UnityEngine.Debug.Log("Configuring near locations.");
        if (nearLocations.Count > 0)
        {
            nearLocations.Sort();
            StartCoroutine(configureNearImage(((Location)nearLocations[0]).image, maskNearLocation1));
            if (nearLocations.Count > 1)
                StartCoroutine(configureNearImage(((Location)nearLocations[1]).image, marskNearLocation2));
            textNoNearLocations.gameObject.SetActive(false);
        }
        else
        {
            textNoNearLocations.gameObject.SetActive(true);
        }
    }

    private IEnumerator configureNearImage(String image, GameObject i)
    {
        UnityEngine.Debug.Log(serverUrl + image);
        WWW www = new WWW(serverUrl + image);
        yield return www;
        i.GetComponentInChildren<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        i.gameObject.SetActive(true);
    }

    private IEnumerator configureImage(String image, GameObject i)
    {
        UnityEngine.Debug.Log(image);
        WWW www = new WWW(image);
        yield return www;
        i.gameObject.SetActive(true);
        i.GetComponentInChildren<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    private IEnumerator obtainMap()
    {
        UnityEngine.Debug.Log(urlMaps1 + GPS.Instance.latitude + "," + GPS.Instance.longitude + urlMaps2 + GPS.Instance.latitude + "," + GPS.Instance.longitude + urlMaps3);
        WWW www = new WWW(urlMaps1+GPS.Instance.latitude+","+GPS.Instance.longitude+urlMaps2+GPS.Instance.latitude+","+GPS.Instance.longitude+urlMaps3);
        yield return www;
        map.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    private void showScene()
    {
        panelRepresentation.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("AR_mode") == 1)
        {
            arCamera.SetActive(true);
        } else
        {
            camera.SetActive(true);
        }
        representationScreen.SetActive(true);
        panelShow.gameObject.SetActive(false);
        panelMap.gameObject.SetActive(false);
        panelOptions.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(configureCancelButton);
        titleRep.text = translator.translate(actualRep.title);
        reproduceVideo(actualRep.videoURL);
        configureInformationPanel();
        TextToSpeech tts = (new GameObject("TextToSpeechObject")).AddComponent<TextToSpeech>();
        tts.launchTTS(actualRep.description, audioSource);
    }

    private void configureInformationPanel() {
        titleInfo.text = translator.translate(actualRep.title);
        contentInformation.text = translator.translate(actualRep.description); //Default
        StartCoroutine(configureImage(actualRep.imageURL, maskActualLocation));
        descriptionButton.onClick.AddListener(delegate { showContentInfo(actualRep.description); });
        historyButton.onClick.AddListener(delegate { showContentInfo(actualRep.history); });
        interestInfo.onClick.AddListener(delegate { showContentInfo(actualRep.interestInfo); });
        technicalInfo.onClick.AddListener(delegate { showContentInfo(actualRep.technicalInfo); });
    }

    private void showContentInfo(String info)
    {
        contentInformation.text = translator.translate(info);
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

public class Location: IComparable
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long id { get; set; }

    public double distance { get; set; }
    public String image { get; set; }

    public Location(double latitude, double longitude, long id)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.id = id;
    }

    public int CompareTo(object l2)
    {
        return Convert.ToInt32(((Location)l2).distance - this.distance);
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
    public double latitude { get; set; }
    public double longitude { get; set; }

    public string videoURL { get; set; }
    public string imageURL { get; set; }

    public Representation(long id, string title, string des, string hist, string interest, string techn, double lat, double longi)
    {
        this.id = id;
        this.title = title;
        this.description = des;
        this.history = hist;
        this.technicalInfo = techn;
        this.latitude = lat;
        this.longitude = longi;
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
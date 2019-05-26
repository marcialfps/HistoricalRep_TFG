using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleMaps : MonoBehaviour {
    
    string exampleUrl1 = "http://maps.googleapis.com/maps/api/staticmap?center=";
    string exampleUrl2 = "&zoom=13&size=600x300&maptype=roadmap&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    string key = "&key=AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE";
    public RawImage image;

    IEnumerator Start()
    {
        if (GPS.Instance != null)
        {
            WWW www = new WWW(exampleUrl1 + GPS.Instance.latitude + "," + GPS.Instance.longitude + exampleUrl2 + key);
            Debug.Log(exampleUrl1 + GPS.Instance.latitude + "," + GPS.Instance.longitude + exampleUrl2 + key);
            yield return www;
            image.texture = www.texture;
        }
    }
}

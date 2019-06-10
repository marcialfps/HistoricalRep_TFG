﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;

public class VirtualVisit : MonoBehaviour
{

    private String serverUrl = "http://192.168.1.36:8080";

    public Text titleRep, titleInfo, contentInformation;
    public Button cancelButton, descriptionButton, historyButton, interestInfo, technicalInfo;
    public Button sampleButton;
    public GameObject buttonContent;
    public GameObject panelVirtualVisit, panelRepresentation, panelMenu, panelMap, arCamera, camera, representationScreen;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;

    private ArrayList representations;

    // Use this for initialization
    void Start()
    {
        obtainAllRepresentations();
        foreach (Representation r in representations)
            obtainRepVideo(r);
        configureUI();
    }

    private void obtainAllRepresentations()
    {
        UnityEngine.Debug.Log("Creating request obtain all representations");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representations");
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        UnityEngine.Debug.Log(data);
        convertToObject(data);
    }

    private void convertToObject(String data)
    {
        List<Representation> deserializedList = JsonConvert.DeserializeObject<List<Representation>>(data);
        this.representations = new ArrayList(deserializedList);
    }

    private void obtainRepVideo(Representation r)
    {
        UnityEngine.Debug.Log("Creating request obtain video");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/video/" + r.id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        r.videoURL = data;
    }

    private void configureUI()
    {
        int i = 0;
        foreach (Representation r in representations)
            if(i == 0)
            {
                sampleButton.GetComponentInChildren<Text>().text = r.title;
                sampleButton.onClick.AddListener(delegate { reproduceRep(r); });
                i = 1;
            } else
            {
                var button = (Button)GameObject.Instantiate(this.sampleButton);
                button.GetComponentInChildren<Text>().text = r.title;
                button.onClick.AddListener(delegate { reproduceRep(r); });
                button.transform.SetParent(buttonContent.transform);
            }
    }

    private void reproduceRep(Representation r)
    {
       // 
        panelRepresentation.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("AR_mode") == 1)
        {
            arCamera.SetActive(true);
        }
        else
        {
            camera.SetActive(true);
        }
        representationScreen.SetActive(true);
        panelVirtualVisit.gameObject.SetActive(false);
        panelMenu.gameObject.SetActive(false);
        panelMap.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(configureCancelButton);
        titleRep.text = r.title;
        reproduceVideo(serverUrl+r.videoURL);
        configureInformationPanel(r);
    }

    private void configureInformationPanel(Representation r)
    {
        titleInfo.text = r.title;
        contentInformation.text = r.description; //Default
        descriptionButton.onClick.AddListener(delegate { showContentInfo(r.description); });
        historyButton.onClick.AddListener(delegate { showContentInfo(r.history); });
        interestInfo.onClick.AddListener(delegate { showContentInfo(r.interestInfo); });
        technicalInfo.onClick.AddListener(delegate { showContentInfo(r.interestInfo); });
    }

    private void showContentInfo(String info)
    {
        contentInformation.text = info;
    }

    private void reproduceVideo(String url)
    {
        renderer.enabled = true;
        if (videoPlayer.url != url) videoPlayer.Stop(); //Casos en lo que se cambie de video
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.url = url;
            UnityEngine.Debug.Log("Añadida url a videoplayer: " + url);
            videoPlayer.Play();
        }
    }

    private void configureCancelButton()
    {
        panelRepresentation.gameObject.SetActive(false);
        arCamera.SetActive(false);
        representationScreen.SetActive(false);
        panelVirtualVisit.gameObject.SetActive(true);
    }
}

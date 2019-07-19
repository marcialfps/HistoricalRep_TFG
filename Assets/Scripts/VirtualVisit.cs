using Newtonsoft.Json;
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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class VirtualVisit : MonoBehaviour
{
    /**
     * URLs of the server.
     */
    private String serverUrl = "https://serverhistrep.herokuapp.com";

    /**
     * UI elements.
     */
    public Text titleRep, titleInfo, contentInformation;
    public Button cancelButton, descriptionButton, historyButton, 
        interestInfo, technicalInfo, buttonCloseRepresentation;
    public Button sampleButton;
    public GameObject buttonContent;
    public GameObject panelVirtualVisit, panelRepresentation, panelOptions,
        arCamera, camera, maskActualLocation;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Renderer renderer;

    /**
     * Private fields of the Script.
     */
    private ArrayList representations;
    private Translator translator = new Translator();

    /**
     * When started, all the information of the representation is obtained. Next, it configures the UI.
     */
    void Start()
    {
        obtainAllRepresentations();
        foreach (Representation r in representations)
        {
            obtainRepVideo(r);
            obtainRepImage(r);
        }
        configureUI();
    }

    /**
     * Auxiliar method to solve problems when trying to connect to the server.
     */
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

    private void obtainAllRepresentations()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        UnityEngine.Debug.Log("Creating request obtain all representations.");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representations");
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        List<Representation> deserializedList = JsonConvert.DeserializeObject<List<Representation>>(data);
        this.representations = new ArrayList(deserializedList);
    }

    private void obtainRepVideo(Representation r)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        UnityEngine.Debug.Log("Creating request obtain video.");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/video/" + r.id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        r.videoURL = serverUrl + data;
    }

    private void obtainRepImage(Representation r)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        UnityEngine.Debug.Log("Creating request obtain image.");
        WebRequest wrGET = WebRequest.Create(serverUrl + "/representation/image/" + r.id);
        Stream objStream = wrGET.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        String data = objReader.ReadLine();
        r.imageURL = serverUrl + data;
    }

    /**
     * It creates one button per representation, setting the image and representation video.
     */
    private void configureUI()
    {
        int i = 0;
        foreach (Representation r in representations)
            if(i == 0)
            {
                sampleButton.GetComponentInChildren<Text>().text = translator.translate(r.title);
                StartCoroutine(configureImageButton(r, sampleButton));
                sampleButton.onClick.AddListener(delegate { reproduceRep(r); });
                i = 1;
            } else
            {
                var button = (Button)GameObject.Instantiate(this.sampleButton);
                button.GetComponentInChildren<Text>().text = translator.translate(r.title);
                StartCoroutine(configureImageButton(r, button));
                button.onClick.AddListener(delegate { reproduceRep(r); });
                button.transform.SetParent(buttonContent.transform);
            }
    }

    private IEnumerator configureImageButton(Representation r, Button b)
    {
        UnityEngine.Debug.Log(r.imageURL);
        WWW www = new WWW(r.imageURL);
        yield return www;
        b.GetComponentInChildren<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    private IEnumerator configureImage(String imageurl)
    {
        UnityEngine.Debug.Log(imageurl);
        WWW www = new WWW(imageurl);
        yield return www;
        maskActualLocation.GetComponentInChildren<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    /**
     * It actives all the elements needed and configure the UI. Finally, reproduce the video and launch the TTS.
     */
    private void reproduceRep(Representation r)
    {
        panelRepresentation.gameObject.SetActive(true);
        PanelControl pc = buttonCloseRepresentation.gameObject.GetComponent<PanelControl>();
        pc.panelToShow1 = panelVirtualVisit;
        if (PlayerPrefs.GetInt("AR_mode") == 1)
        {
            arCamera.SetActive(true);
        }
        else
        {
            camera.SetActive(true);
        }
        panelVirtualVisit.gameObject.SetActive(false);
        panelOptions.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(configureCancelButton);
        titleRep.text = translator.translate(r.title);
        reproduceVideo(r.videoURL);
        configureInformationPanel(r);
        TextToSpeech tts = (new GameObject("TextToSpeechObject")).AddComponent<TextToSpeech>();
        tts.launchTTS(translator.translate(r.description), audioSource);
    }

    /**
     * Set the content that will be shown.
     */
    private void configureInformationPanel(Representation r)
    {
        titleInfo.text = translator.translate(r.title);
        contentInformation.text = translator.translate(r.description); //Default
        StartCoroutine(configureImage(r.imageURL));
        descriptionButton.onClick.AddListener(delegate { showContentInfo(r.description); });
        historyButton.onClick.AddListener(delegate { showContentInfo(r.history); });
        interestInfo.onClick.AddListener(delegate { showContentInfo(r.interestInfo); });
        technicalInfo.onClick.AddListener(delegate { showContentInfo(r.technicalInfo); });
    }

    private void showContentInfo(String info)
    {
        contentInformation.text = translator.translate(info);
    }

    private void reproduceVideo(String url)
    {
        renderer.enabled = true;
        videoPlayer.Stop(); //In case the video is changed.
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
        panelVirtualVisit.gameObject.SetActive(true);
        videoPlayer.Stop();
        audioSource.Stop();
    }
}

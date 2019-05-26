using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class LoadScene : MonoBehaviour {

    public string sceneName;
    public Button button;

    // Use this for initialization
    void Start () {
        button.onClick.AddListener(loadScene);
    }
	
	// Update is called once per frame
	void loadScene () {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}

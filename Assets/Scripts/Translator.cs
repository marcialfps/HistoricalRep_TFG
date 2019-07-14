using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Runtime;

public class Translator {

    /**
     * If the language is english, it calls the API in order to translate the spanish text.
     */
    public string translate(string inputText)
    {
        UnityEngine.Debug.Log("Translating text: "+ inputText);
        if (PlayerPrefs.GetString("Language").Equals("English")) {
            WebRequest wrGET = WebRequest.Create("https://translation.googleapis.com/language/translate/v2?q="+inputText+"&target=en&key="
                +"AIzaSyDIhY8U0bDAtyYyJw-iuIBI2a1KPWbYMJE");
            Stream objStream = wrGET.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            String json = objReader.ReadToEnd();
            Result data = JsonConvert.DeserializeObject<Result>(json);

            return data.data.translations[0].translatedText;
        } else
        {
            return inputText;
        }
    }
}

public class Result
{
    public Data data { get; set; }
}

public class Data
{
    public List<Translation> translations { get; set; }
}

public class Translation
{
    public string translatedText { get; set; }
    public string detectedSourceLanguage { get; set; }
}
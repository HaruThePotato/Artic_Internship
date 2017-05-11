using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class LoadManager : MonoBehaviour {

    AWSscript aws;

    public GameObject localButtonPrefab;
    public GameObject remoteButtonPrefab;

    public Canvas UILoad;
    public Canvas UIDownload;

    string levelSelected;
    string downloadSelected;

    public List<string> listOfDownloadables;

    private static LoadManager instance = null;

    public static LoadManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        aws = AWSscript.GetInstance();
        LoadLevelScreenButton();
        //loadDownloads();
    }

    public void LoadLevelScreenButton()
    {
        levelSelected = null;
        if (Directory.Exists(Application.dataPath + "/Serialization/XML"))
        {
            string[] loadedXMLs = Directory.GetFiles(Application.dataPath + "/Serialization/XML/", "*.xml");
            Transform t = UILoad.transform.FindChild("BackgroundPanel").FindChild("LocalExteriorPanel").FindChild("LocalPanel").FindChild("Image");
            if (t.childCount > 0)
            {
                foreach (Transform child in t.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (string f in loadedXMLs)
            {
                GameObject levelButton = Instantiate(localButtonPrefab, t) as GameObject;
                levelButton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileName(f).Substring(0, Path.GetFileName(f).Length - 4);
                levelButton.name = Path.GetFileName(f);
                levelButton.GetComponent<Button>().onClick.AddListener(() => { SelectLevel(levelButton.name); });
            }
        }
    }

    public void SelectLevel(string n)
    {
        levelSelected = n;
    }

    public void loadDownloads()
    {
        //uim.Status.text = "Connecting...";
        listOfDownloadables = aws.getListOfBucketObjects("LoadManager");
        Debug.Log("Connecting to AWS S3 Database.");
    }

    public void callDownload()
    {
        if (downloadSelected == null)
        {
            //UIManager.GetInstance().Status.text = "No Download Selected.";
            Debug.Log("No downloadable object has been selected.");
        }
        else
        {
            //uim.Status.text = "Downloading...";
            aws.GetObject(downloadSelected + ".xml");
        }
    }

    public void loadDownloadsInContainer()
    {
        levelSelected = null;
        Transform t = UILoad.transform.FindChild("BackgroundPanel").FindChild("RemoteExteriorPanel").FindChild("RemotePanel").FindChild("Image");
        if (t.childCount > 0)
        {
            foreach (Transform child in t.transform)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (string objects in listOfDownloadables)
        {
            GameObject downloadButton = Instantiate(remoteButtonPrefab, t) as GameObject;
            downloadButton.transform.GetChild(0).GetComponent<Text>().text = objects.Substring(0, objects.Length - 5).Replace("level/", "");/*Path.GetFileName(objects).Substring(0, Path.GetFileName(objects).Length - 4);*/
            downloadButton.name = objects.Substring(0, objects.Length - 5).Replace("level/", "");
            downloadButton.GetComponent<Button>().onClick.AddListener(() => { SelectDownload(downloadButton.name); });
        }
    }

    public void SelectDownload(string n)
    {
        downloadSelected = n;
    }
}

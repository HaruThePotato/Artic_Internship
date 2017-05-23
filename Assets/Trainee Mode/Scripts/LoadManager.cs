using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class LoadManager : MonoBehaviour
{
    AWSscript aws;
    XMLManager xmlm;
    GridManager gm;
    ObjectManager objm;
    UIManager uim;

    public GameObject floorPrefab;
    Dictionary<string, float> floorPosition;

    public GameObject localButtonPrefab;
    public GameObject remoteButtonPrefab;

    public Canvas UILoad;
    public Canvas UIDownload;
    public Canvas UILoadLevel;
    public GameObject loadingUI;

    string levelSelected;
    string downloadSelected;

    List<string> listOfDownloadables;

    public string BundleURL;
    public int version;
    public string assetName;


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
        floorPosition = new Dictionary<string, float>();
        aws = AWSscript.GetInstance();
        xmlm = XMLManager.GetInstance();
        gm = GridManager.GetInstance();
        objm = ObjectManager.GetInstance();
        uim = UIManager.GetInstance();

        floorPosition.Add("Lesser X", 0);
        floorPosition.Add("More X", 0);
        floorPosition.Add("Lesser Z", 0);
        floorPosition.Add("More Z", 0);

        LoadLevelScreenButton();
        Invoke("loadDownloads", 0.1f);

        assetName = "CompleteTank";
        BundleURL = "https://s3-us-west-2.amazonaws.com/articsp/AssetBundle/texture";
        version = 1;
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
            aws.GetObject(downloadSelected + ".xml", "LoadManager");
        }
        loadDownloadsInContainer();
    }

    public void loadDownloadsInContainer()
    {
        levelSelected = null;
        Transform t = UILoad.transform.FindChild("BackgroundPanel").FindChild("DownloadExteriorPanel").FindChild("DownloadPanel").FindChild("Image");
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

    public void callLoadSelectedLevel()
    {
        loadingUI.SetActive(true);
        StartCoroutine(LoadSelectedLevel());
    }

    public IEnumerator LoadSelectedLevel()
    {
        bool firstRun = true;
        if (levelSelected != null)
        {
            LevelDatabase levelData = xmlm.LoadLevel(levelSelected);

            // Wait for the Caching system to be ready
            while (!Caching.ready)
                yield return null;

            using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
            {
                yield return www;
                if (www.error != null)
                    throw new Exception("WWW download had an error:" + www.error);
                AssetBundle bundle = www.assetBundle;
                foreach (LevelNode lNode in levelData.dbList)
                {
                    for (int i = 0; i < lNode.objectIDs.Count; i++)
                    {
                        LevelObject lObj = new LevelObject();
                        if (assetName == "")
                        {
                            Instantiate(bundle.mainAsset);
                            Debug.Log("Asset has no name.");
                        }
                        else
                        {

                            //Debug.Log(objm.GetLevelObject(lNode.objectIDs[i]).LObject);
                            lObj.LObject = Instantiate(bundle.LoadAsset(lNode.objectIDs[i]), lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;                            //lObj.LObject = Instantiate(bundle.LoadAsset(lNode.objectIDs[i]), lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
                            //lObj.LObject = Instantiate(objm.GetLevelObject(lNode.objectIDs[i]).LObject, lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
                            lObj.LObject.transform.GetChild(0).transform.eulerAngles = lNode.objectRotations[i];
                            lObj.LObject.transform.localScale = levelData.objectScale;
                            lObj.LObject.name = lNode.objectIDs[i];

                            if (firstRun)
                            {
                                firstRun = false;
                                floorPosition["More X"] = 0;
                                floorPosition["More Z"] = 0;
                                floorPosition["Lesser X"] = lObj.LObject.transform.position.x;
                                floorPosition["Lesser Z"] = lObj.LObject.transform.position.z;

                            }

                            if (lObj.LObject.transform.position.x < floorPosition["Lesser X"]) // lesser of x
                            {
                                floorPosition["Lesser X"] = (lObj.LObject.transform.position.x);
                            }
                            if (lObj.LObject.transform.position.x > floorPosition["More X"]) // higher of x
                            {
                                floorPosition["More X"] = (lObj.LObject.transform.position.x);
                            }
                            if (lObj.LObject.transform.position.z < floorPosition["Lesser Z"]) // lesser of z
                            {
                                floorPosition["Lesser Z"] = (lObj.LObject.transform.position.z);
                            }
                            if (lObj.LObject.transform.position.z > floorPosition["More Z"]) // higher of z
                            {
                                floorPosition["More Z"] = (lObj.LObject.transform.position.z);
                            }
                            //if (lObj.LObject.name == "RW_RunwayNumber")
                            //{
                            //    for (int j = 0; j < lNode.objectIDs.Count; j++)
                            //    {
                            //        if (lNode.numberStrings != null) //if there is/are runway number(s) being inputted previously before saving. THIS ALLOWS THE LEVEL TO LOAD EVEN IF THERE IS NO NUMBER.
                            //        {
                            //            lObj.LObject.transform.Find("UICanvas").transform.Find("lane_text").gameObject.GetComponent<Text>().text = lNode.numberStrings[j];
                            //        }
                            //    }
                            //}
                            lObj.LObjectType = lNode.objectTypes[i];
                        }
                    }
                }
                // Unload the AssetBundles compressed contents to conserve memory
                bundle.Unload(false);
                CancelLoadScreen();
                Debug.Log(floorPosition["Lesser X"]);
                Debug.Log(floorPosition["More X"]);
                Debug.Log(floorPosition["Lesser Z"]);
                Debug.Log(floorPosition["More Z"]);
            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
        }
        createFloor();
    }

    public void CleanCache()
    {
        if (Caching.CleanCache())
        {
            Debug.Log("Successfully cleaned the cache.");
        }
        else
        {
            Debug.Log("Cache is being used.");
        }
    }

    public void CancelLoadScreen()
    {
        StartCoroutine(FadeCanvas(UILoadLevel, false));
        UILoadLevel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //cc.cAllowCameraControls = true;
    }

    IEnumerator FadeCanvas(Canvas c, bool r)
    {
        if (!r)
        {
            while (c.GetComponent<CanvasGroup>().alpha > 0)
            {
                c.GetComponent<CanvasGroup>().alpha -= Time.deltaTime * 2;
                yield return null;
            }
        }
        else
        {
            while (c.GetComponent<CanvasGroup>().alpha < 1)
            {
                c.GetComponent<CanvasGroup>().alpha += Time.deltaTime * 2;
                yield return null;
            }
        }
        yield return null;
    }

    public void createFloor()
    {
        Dictionary<string, float> lengthOfFloor = new Dictionary<string, float>();
        lengthOfFloor.Add("X", floorPosition["More X"] - floorPosition["Lesser X"] + 1);
        lengthOfFloor.Add("Z", floorPosition["More Z"] - floorPosition["Lesser Z"] + 1);

        Dictionary<string, float> CenterOfFloor = new Dictionary<string, float>();
        CenterOfFloor.Add("XCenter", floorPosition["More X"] - (lengthOfFloor["X"] / 2));
        CenterOfFloor.Add("ZCenter", floorPosition["More Z"] - (lengthOfFloor["Z"] / 2));

        Debug.Log(CenterOfFloor["XCenter"] + "XCenter");
        Debug.Log(CenterOfFloor["ZCenter"] + "ZCenter");

        floorPrefab.GetComponent<Transform>().position = new Vector3(CenterOfFloor["XCenter"], 0, CenterOfFloor["ZCenter"]);
        floorPrefab.GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 1);
        floorPrefab.GetComponent<BoxCollider>().size = new Vector3(lengthOfFloor["X"], 1, lengthOfFloor["Z"]);
    }

    //public void LoadSelectedLevel()
    //{
    //    if (levelSelected != null)
    //    {
    //        LevelDatabase levelData = xmlm.LoadLevel(levelSelected);

    //        foreach (LevelNode lNode in levelData.dbList)
    //        {
    //            for (int i = 0; i < lNode.objectIDs.Count; i++)
    //            {
    //                LevelObject lObj = new LevelObject();
    //                Debug.Log(objm.GetLevelObject(lNode.objectIDs[i]).LObject);
    //                lObj.LObject = Instantiate(objm.GetLevelObject(lNode.objectIDs[i]).LObject, lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
    //                lObj.LObject.transform.transform.eulerAngles = lNode.objectRotations[i];
    //                lObj.LObject.transform.localScale = levelData.objectScale;
    //                lObj.LObject.name = lNode.objectIDs[i];
    //                lObj.LObjectType = lNode.objectTypes[i];
    //            }
    //        }
    //    }
    //}

    //public IEnumerator DownloadAndCache()
    //{
    //    // Wait for the Caching system to be ready
    //    while (!Caching.ready)
    //        yield return null;

    //    // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
    //    using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
    //    {
    //        Debug.Log("DownloadAndCache");
    //        yield return www;
    //        if (www.error != null)
    //            throw new Exception("WWW download had an error:" + www.error);
    //        AssetBundle bundle = www.assetBundle;
    //        if (assetName == "")
    //            Instantiate(bundle.mainAsset);
    //        else
    //            Instantiate(bundle.LoadAsset(assetName));
    //        // Unload the AssetBundles compressed contents to conserve memory
    //        bundle.Unload(false);

    //    } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    //}
}

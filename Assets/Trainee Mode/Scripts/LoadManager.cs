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

    public GameObject levelButtonPrefab;

    public Canvas UILoad;
    public Canvas UIDownload;

    string levelSelected;
    string downloadSelected;

    void Start()
    {
        LoadLevelScreenButton();
    }

    public void SelectLevel(string n)
    {
        levelSelected = n;
    }

    public void LoadLevelScreenButton()
    {
        levelSelected = null;
        if (Directory.Exists(Application.dataPath + "/Serialization/XML"))
        {
            string[] loadedXMLs = Directory.GetFiles(Application.dataPath + "/Serialization/XML/", "*.xml");
            //Transform t = UILoad.transform.GetChild(0).FindChild("LoadingPanel").GetChild(0);
            Transform t = UILoad.transform.FindChild("BackgroundPanel").FindChild("LocalPanel").FindChild("Scroll View");
            if (t.childCount > 0)
            {
                foreach (Transform child in t.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            foreach (string f in loadedXMLs)
            {
                GameObject levelButton = Instantiate(levelButtonPrefab, t) as GameObject;
                levelButton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileName(f).Substring(0, Path.GetFileName(f).Length - 4);
                levelButton.name = Path.GetFileName(f);
                levelButton.GetComponent<Button>().onClick.AddListener(() => { SelectLevel(levelButton.name); });
            }

        }
    }

}

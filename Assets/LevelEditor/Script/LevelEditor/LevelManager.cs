// DONE BY \\
//  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    List<string> listOfDownloadables;

    GridManager gm;
	UIManager uim;
	ObjectManager objm;
	XMLManager xmlm;
	CheckManager cm;
    AWSscript aws;

    public LevelObject hObject;

	[Header("LevelManager References")]
	[Tooltip("The prefab for each \"LEVELBUTTON\"")]
	public GameObject levelButtonPrefab;
	[Header("LevelManager Values")]
	[Tooltip("The rotation angle in which the object will rotate")]
	public int rAngle = 90;

	public LevelObject selectedObj = new LevelObject();
	[System.NonSerialized]
	public bool bHoldingObject = false;

	public string levelSelected;
    string downloadSelected;

	bool adjacentCheck = false;
	bool runwayCheck = false;
	bool rangeCheck = false;

	private static LevelManager instance = null;

	public static LevelManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		gm = GridManager.GetInstance();
		uim = UIManager.GetInstance();
		objm = ObjectManager.GetInstance();
		xmlm = XMLManager.GetInstance();
		cm = CheckManager.GetInstance();
        aws = AWSscript.GetInstance();
	}

	void Update()
	{
		SpawnAndHoverObject();
		InputHandler();
	}

	void SpawnAndHoverObject()
	{
		if (selectedObj.LObject != null)
		{
			if (gm.isHovering && !uim.mouseOverUI && gm.currentNode.bFree)
			{
				Vector3 pos = new Vector3(gm.currentNode.nPosX, 0, gm.currentNode.nPosZ);
				if (gm.currentNode.nObjects.Count > 0)
				{
					foreach (LevelObject go in gm.currentNode.nObjects)
					{
						pos.y += go.LObject.transform.GetComponentInChildren<MeshRenderer>().bounds.size.y;

					}
				}

				if (!bHoldingObject)
				{
					//Spawn object if not yet in scene
					hObject = new LevelObject();
					hObject.LObject = Instantiate(selectedObj.LObject, pos, Quaternion.identity) as GameObject;
					hObject.LObject.transform.parent = transform.FindChild("LevelObjects").transform;
					hObject.LObject.transform.localScale = gm.currentNode.nVisualGrid.transform.localScale;
					hObject.LObject.name = selectedObj.LObject.name;
					hObject.LObjectType = selectedObj.LObjectType;
					hObject.bundleName = selectedObj.bundleName;
					bHoldingObject = true;
					uim.holdingObject = true;
				}
				else
				{
					//Update position of object after spawning
					if (gm.currentNode != gm.prevNode)
					{
						hObject.LObject.transform.position = pos;
					}
				}
			}
		}
	}

	void InputHandler()
	{
		//Clones objects on button down
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && bHoldingObject)
		{
			PlaceObject(true);
		}
		//Delete objects on button down
		else if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0) && !bHoldingObject)
		{
			DeleteObject();
		}
		//Rotate object on button down
		else if (Input.GetMouseButtonDown(1) && bHoldingObject)
		{
			RotateObject();
		}
		//Get object that is placed on click
		else if (Input.GetMouseButtonDown(1) && !bHoldingObject && !Input.GetKey(KeyCode.LeftShift))
		{
			GetObject();
		}
		//Places object once on click
		else if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && bHoldingObject)
		{
			PlaceObject(false);
		}
		//Cancels selected object on key R
		else if ((Input.GetKey(KeyCode.R) && bHoldingObject))
		{
			CancelSelect();
		}
	}


	public void CloneSucceed()
	{
		gm.currentNode.nObjects.Add(hObject);
		bHoldingObject = false;
		uim.holdingObject = false;
	}

	public void PlaceSucceed()
	{
		gm.currentNode.nObjects.Add(hObject);
		bHoldingObject = false;
		uim.holdingObject = false;
		hObject = new LevelObject();
		selectedObj = new LevelObject();

	}

	public void CancelSelect() //destroys hovering object and resets object selection
	{
		Destroy(hObject.LObject);
		selectedObj.LObject = null;
		selectedObj.LObjectType = 0;
		bHoldingObject = false;
		uim.holdingObject = false;
		hObject = new LevelObject();
		selectedObj = new LevelObject();
	}

	void PlaceObject(bool cloneOrPlace) //This access CheckManager for checking conditions before placing an object. ACCESS CHECKMANAGER.
	{
		if (cloneOrPlace) //if PlaceObject is true
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
                cm.PlaceObjectClone(); 
            }
		}
		else if (!cloneOrPlace) //if not cloning object
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
                cm.PlaceObjectSingle(); 
            }
        }
	}

	/*void PlaceObject(bool cloneOrPlace) //This only checks if the object is stackable/non-stackable before placing the object. DOES NOT ACCESS CHECKMANAGER.
	{
		if (cloneOrPlace) //if PlaceObject is true
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
				if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
				{
					if (gm.currentNode.nObjects.Last().LObjectType != 1&& gm.currentNode.nObjects.Last().LObjectType != 4) //if the top object is stackable
					{
						CloneSucceed(); //add the selected object on the selected grid
					}
					else
					{
						CancelSelect();
					}
				}
				else
				{

					CloneSucceed(); //add the selected object on the selected grid
				}
			}
		}
		else if (!cloneOrPlace) //if PlaceObject is false;
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
				if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
				{
					if (gm.currentNode.nObjects.Last().LObjectType != 1 && gm.currentNode.nObjects.Last().LObjectType != 4) //if the top object is stackable
					{
						PlaceSucceed();////add the selected object on the selected grid
					}
					else
					{
						CancelSelect();
					}
				}
				else
				{
					PlaceSucceed(); //add the selected object on the selected grid
				}
			}
		}
	}*/

	void DeleteObject()
	{
		if (gm.isHovering && !uim.mouseOverUI)
		{
			if (gm.currentNode.nObjects.Count > 0)
			{
				Destroy(gm.currentNode.nObjects.Last().LObject);
				gm.currentNode.nObjects.RemoveAt(gm.currentNode.nObjects.Count - 1);
			}
		}
	}

	void RotateObject()
	{
		if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null)
		{
			hObject.LObject.transform.GetChild(0).transform.eulerAngles += new Vector3(0, rAngle, 0);
		}
	}

	void GetObject()
	{
		if (gm.isHovering && !uim.mouseOverUI && gm.selectedNode != null)
		{
			if (gm.selectedNode == gm.currentNode && gm.selectedNode.nObjects.Count > 0)
			{
				hObject = new LevelObject();
				hObject = gm.selectedNode.nObjects.Last();
				gm.selectedNode.nObjects.RemoveAt(gm.selectedNode.nObjects.Count - 1);
				bHoldingObject = true;
				uim.holdingObject = true;
				selectedObj = hObject;
			}
		}
	}

	//Button Functions 
	public void NewLevelButton()
	{
		if (uim.mouseOverUI && !bHoldingObject)
		{
			gm.ResetCameraObject();
			foreach (Node n in gm.myGrid)
			{
				if (n.nObjects.Count > 0)
				{
					foreach (LevelObject lo in n.nObjects)
					{
						Destroy(lo.LObject);
					}
					n.nObjects.Clear();					
				}
			}
			uim.levelName.text = "";
			uim.Status.text = "New Level";
		}
	}
	
	public void SaveLevelButton()
	{
		LevelDatabase levelDB = new LevelDatabase();
		string input = uim.UIsave.transform.GetChild(0).GetChild(0).FindChild("SaveInput").GetComponent<InputField>().text;
		if (input != "")
		{
			uim.levelName.text = input;
			levelDB.cameraPosition = gm.cameraPlacementObject.transform.position;
			levelDB.objectScale = gm.myGrid[0, 0].nVisualGrid.transform.localScale;
			List<string> myBundle = new List<string>();
			foreach (Node n in gm.myGrid)
			{
				if (n.nObjects.Count > 0)
				{
					LevelNode levelNode = new LevelNode();
					levelNode.nodePositionX = n.nPosX;
					levelNode.nodePositionZ = n.nPosZ;
					for (int i = 0; i < n.nObjects.Count; i++)
					{
						levelNode.objectPositions.Add(n.nObjects[i].LObject.transform.position);
						levelNode.objectRotations.Add(n.nObjects[i].LObject.transform.GetChild(0).transform.eulerAngles);
						levelNode.objectTypes.Add(n.nObjects[i].LObjectType);
						levelNode.objectIDs.Add(n.nObjects[i].LObject.name);
						//levelDB.objectBundleNames.Add(n.nObjects[i].bundleName);
						myBundle.Add(n.nObjects[i].bundleName);
					}
					levelDB.dbList.Add(levelNode);
				}
			}
			levelDB.objectBundleNames = myBundle.Distinct().ToList();
			xmlm.SaveLevel(input, levelDB);
			uim.Status.text = "Saved Level";

			if (uim.UIsave.transform.GetChild(0).GetChild(0).FindChild("UploadToggle").GetComponent<Toggle>().isOn)
			{
				LevelDatabase levelAWSDB = new LevelDatabase();
                levelAWSDB.cameraPosition = gm.cameraPlacementObject.transform.position;
                levelAWSDB.objectScale = gm.myGrid[0, 0].nVisualGrid.transform.localScale;
				List<string> myBundleA = new List<string>();
				foreach (Node n in gm.myGrid)
				{
					if (n.nObjects.Count > 0)
					{
                        LevelNode levelNode = new LevelNode();
                        levelNode.nodePositionX = n.nPosX;
                        levelNode.nodePositionZ = n.nPosZ;
                        for (int i = 0; i < n.nObjects.Count; i++)
						{
                            levelNode.objectPositions.Add(n.nObjects[i].LObject.transform.position);
                            levelNode.objectRotations.Add(n.nObjects[i].LObject.transform.GetChild(0).transform.eulerAngles);
                            levelNode.objectTypes.Add(n.nObjects[i].LObjectType);
                            levelNode.objectIDs.Add(n.nObjects[i].LObject.name);
							myBundleA.Add(n.nObjects[i].bundleName);
						}
                        levelAWSDB.dbList.Add(levelNode);
					}
				}
                levelAWSDB.objectBundleNames = myBundle.Distinct().ToList();
				xmlm.UploadLevel(input, levelAWSDB);
				uim.Status.text = "Saved Level\nUploading ..";
			}
			uim.CancelSaveScreen();
		}

	}

	public void LoadLevelScreenButton()
	{
		levelSelected = null;
		if (Directory.Exists(Application.dataPath + "/Serialization/XML"))
		{
			string[] loadedXMLs = Directory.GetFiles(Application.dataPath + "/Serialization/XML/", "*.xml");
			//Debug.Log("Loaded Length : " + loadedXMLs.Length);
			Transform t = uim.UIload.transform.GetChild(0).GetChild(0).FindChild("LevelPanel").GetChild(0);
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

	public void SelectLevel(string n)
	{
		levelSelected = n;
		uim.selectedLevelName.text = "Level Selected: " + n;
		uim.selectedLevelName.text = uim.selectedLevelName.text.Replace(uim.xml, uim.blank);
	}

	public void LoadSelectedLevel()
	{
		if (levelSelected != null)
		{
			uim.selectedLevelName.text = null;
			uim.levelName.text = levelSelected;
			if (uim.mouseOverUI && !bHoldingObject)
			{
				gm.ResetCameraObject();
				foreach (Node n in gm.myGrid)
				{
					if (n.nObjects.Count > 0)
					{
						foreach (LevelObject lo in n.nObjects)
						{
							Destroy(lo.LObject);
						}
						n.nObjects.Clear();
					}
				}
			}
			LevelDatabase levelData = xmlm.LoadLevel(levelSelected);
			gm.cameraPlacementObject.transform.position = levelData.cameraPosition;
			gm.pCam = gm.FindNodeFromPos(levelData.cameraPosition.x, levelData.cameraPosition.z);
			gm.pCam.bFree = false;

			foreach (LevelNode lNode in levelData.dbList)
			{
				for (int i = 0; i < lNode.objectIDs.Count; i++)
				{
					LevelObject lObj = new LevelObject();
					lObj.LObject = Instantiate(objm.GetLevelObject(lNode.objectIDs[i]).LObject, lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
					lObj.LObject.transform.GetChild(0).transform.eulerAngles = lNode.objectRotations[i];
					lObj.LObject.transform.localScale = levelData.objectScale;
					lObj.LObject.name = lNode.objectIDs[i];
					lObj.LObjectType = lNode.objectTypes[i];
					gm.FindNodeFromPos(lNode.nodePositionX, lNode.nodePositionZ).nObjects.Add(lObj);
				}
			}
			uim.CancelLoadScreen();
			uim.Status.text = "Loaded level";
		}
	}

	public void DeleteSelectedLevel()
	{
		if (levelSelected != null)
		{
			File.Delete(Application.dataPath + "/Serialization/XML/" + levelSelected);
			Destroy(uim.UIload.transform.GetChild(0).GetChild(0).FindChild("LevelPanel").GetChild(0).FindChild(levelSelected).gameObject);
			levelSelected = null;
			uim.Status.text = "Deleted level";
		}
	}

    public void loadDownloads()
    {
        uim.Status.text = "Connecting...";
        Debug.Log("Connecting to AWS S3 Database.");
        listOfDownloadables = aws.getListOfBucketObjects("LevelManager");
    }

    public void callDownload()
    {
        if (downloadSelected == null)
        {
            UIManager.GetInstance().Status.text = "No Download Found.";
            Debug.Log("No downloadable object has been selected.");
        }
        else
        {
            uim.Status.text = "Downloading...";
            aws.GetObject(downloadSelected + ".xml", "LevelManager");
        }
    }

    public void loadDownloadsInContainer()
    {
        levelSelected = null;
        Transform t = uim.UIDownload.transform.GetChild(0).GetChild(0).FindChild("DownloadPanel").GetChild(0);
        if (t.childCount > 0)
        {
            foreach (Transform child in t.transform)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (string objects in listOfDownloadables)
        {
            GameObject downloadButton = Instantiate(levelButtonPrefab, t) as GameObject;
            downloadButton.transform.GetChild(0).GetComponent<Text>().text = objects.Substring(0, objects.Length - 5).Replace("level/", "");/*Path.GetFileName(objects).Substring(0, Path.GetFileName(objects).Length - 4);*/
            downloadButton.name = objects.Substring(0, objects.Length - 5).Replace("level/", "");
            downloadButton.GetComponent<Button>().onClick.AddListener(() => { SelectDownload(downloadButton.name); });
            Debug.Log(objects);
        }
    }

    public void SelectDownload(string n)
    {
        downloadSelected = n;
    }

    void AddBundleName(List<string> myBundle, string b)
	{
		if (myBundle.Count == 0)
		{
			myBundle.Add(b);
		}
		else
		{
			foreach (string s in myBundle)
			{
				if (s != b)
				{
					myBundle.Add(b);
				}
			}
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CheckManager : MonoBehaviour
{

	GridManager gm;
	UIManager uim;
	ObjectManager objm;
	XMLManager xmlm;
	LevelManager lvlm;

	//bool runwayCheck = false;
	//bool rangeCheck = false;

	GameObject[] vehicle;

	//Insert 2 objects' names in selectedObject and hitObject if they cannot be placed adjacent to each other. Enter new line per 10 objecs for easier management
	string[] selectedObject = new string[] { "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad",
		"Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_BlastPad", "Runway_DisplacedThreshold", "Runway_DisplacedThreshold2", "Runway_Threshold", "Runway_TouchDownZone", "Runway_AimingPoint", "Radio_Tower",
		"Radio_Tower", "Radio_Tower", "Radio_Tower", "Radio_Tower", "Radio_Tower", "Radio_Tower", "Taxi_Cross", "Taxi_Cross", "Taxi_Curve", "Taxi_Line",
	    "Plane_A320", "Plane_A350", "Plane_A380", "Plane_B777-300er", "Pushback_Vehicle"};
	string[] hitObject = new string[] { "Taxi_Cross", "Taxi_Curve", "Taxi_Diagonal", "Taxi_Diagonal2", "Taxi_Diagonalflip", "Taxi_Line", "Taxi_Merge", "Taxi_MergeLine", "Taxi_StraightDia", "Taxi_StraightDiaFlip",
		"Taxi_Tline", "Taxi_X", "Taxi_XFlip", "Runway_DisplacedThreshold", "Runway_DisplacedThreshold", "Runway_DisplacedThreshold2", "Runway_Threshold", "Runway_TouchDownZone", "Runway_AimingPoint", "Radio_Tower",
		"Terminal_Corner1", "Terminal_Corner2", "Terminal_End", "Terminal_Middle", "Terminal_MiddleEnd", "Apron_GateBridgeLight", "Taxi_Diagonal", "Taxi_RunwayBorderDia", "Taxi_RunwayBorderDia", "Taxi_RunwayBorderDia",
	    "Hangar_Roof", "Hangar_Roof", "Hangar_Roof", "Hangar_Roof", "Hangar_Roof"};

	//Insert 2 object tags in selectedObjectTag and hitObjectTag if they cannot be placed adjacent to each other. Enter new line per 10 objecs for easier madnagement
	string[] selectedObjectTag = new string[] { "runway", "runway", "runwayNumber", "runwayNumber", "runwayNumber", "hangar"};
	string[] hitObjectTag = new string[] { "roadway", "apron", "roadway", "apron", "runwayNumber", "apronOnly"};

	//Insert 2 runway objects' names in selectedRunway and hitRunway if they can be placed next to each other. Enter new line per 10 objecs for easier management
	string[] selectedRunway = new string[] { "Runway_BlastPad", "Runway_DisplacedThreshold2", "Runway_Threshold", "Runway_Threshold", "Runway_Line", "Runway_Line", "Runway_AimingPoint" };
	string[] hitRunway = new string[] { "Runway_BlastPad", "Runway_DisplacedThreshold", "Runway_DisplacedThreshold2", "Runway_BlastPad", "Runway_TouchDownZone", "Runway_Line", "Runway_Line" };

	//Insert 2 runway numbers's names in selectedNumber and hitNumber if they can be placed next to each other.
	string[] selectedNumber = new string[] { "Runway_9", "Runway_9C", "Runway_9L", "Runway_9R", "Runway_18", "Runway_18C", "Runway_18L", "Runway_18R" };
	string[] hitNumber = new string[] { "Runway_27", "Runway_27C", "Runway_27R", "Runway_27L", "Runway_36", "Runway_36C", "Runway_36R", "Runway_36L" };

	//Insert 2 objects' names in TObjectOne and TObjectTwo if they can be placed diagonally but not next to each other. Enter new line per 10 objecs for easier management
	string[] TObjectOne = new string[] { "Hangar_Front", "Hangar_Roof",  "Runway_BlastPad", "Runway_BlastPad", "Terminal_Middle", "Terminal_MiddleEnd", "Taxi_Cross", "Taxi_Diagonal", "Taxi_Diagonal2", "Taxi_Diagonalflip",
	    "Taxi_Line", "Taxi_RunwayBorder", "Taxi_RunwayBorderDia", "Taxi_StraightDia", "Taxi_StraightDiaFlip", "Taxi_Tline", "Taxi_X","Taxi_XFlip", "Taxi_Curve", "Taxi_Merge",
		 "Plane_A320", "Plane_A350", "Plane_A380", "Plane_B777-300er", "Pushback_Vehicle"};
	string[] TObjectTwo = new string[] { "Hangar_Side", "Hangar_Corner",  "Taxi_RunwayBorder", "Taxi_RunwayBorderDia", "Apron_GateBridgeLight", "Apron_GateBridgeLight", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline",
	    "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline", "Vehicle_Towline",
	    "Terminal_Middle", "Terminal_Middle", "Terminal_Middle", "Terminal_Middle", "Terminal_Middle"};

	private static CheckManager instance = null;

	public static CheckManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start()
	{
		gm = GridManager.GetInstance();
		uim = UIManager.GetInstance();
		objm = ObjectManager.GetInstance();
		xmlm = XMLManager.GetInstance();
		lvlm = LevelManager.GetInstance();
	}

	// Update is called once per frame
	void Update()
	{
	}

	bool CheckRunway() //check the runway lane for correct order. This is super long because of all the double checking and in opposite order.
	{
		bool runwayCheck = true;
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f); //Extra 0.5f for correction of 0.5 in Rot and Collider
		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, 1, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, 1, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left or back in layers runwayMarkings and runwayNumber
		{
			for (int i = 0; i < selectedRunway.Length; i++)
			{
				if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
				{//check if the selectedObj is runwayNumber tagged and the collidedObj is Runway_Threshold
					RaycastHit hitTwo;
					if (Physics.Raycast(lastPos, -transform.right, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber")) 
						|| Physics.Raycast(lastPos, -transform.forward, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))) //raycast left and back in layer runwayNumber
					{
						for (int j = 0; j < selectedRunway.Length; j++)
						{
							if ((lvlm.selectedObj.LObject.name == selectedNumber[j] && hitTwo.collider.gameObject.name == hitNumber[j])
								|| (lvlm.selectedObj.LObject.name == hitNumber[j] && hitTwo.collider.gameObject.name == selectedNumber[j])) //check selectedNumber and hitNumber strings
							{
								runwayCheck = true;
								break;
							}
							else
							{
								runwayCheck = false;
							}
						}
					}
					else if (Physics.Raycast(lastPos, transform.right, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
						|| Physics.Raycast(lastPos, transform.forward, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))) //raycasts right and forward in layer runwayNumber
					{
						for (int j = 0; j < selectedRunway.Length; j++)
						{
							if ((lvlm.selectedObj.LObject.name == selectedNumber[j] && hitTwo.collider.gameObject.name == hitNumber[j])
								|| (lvlm.selectedObj.LObject.name == hitNumber[j] && hitTwo.collider.gameObject.name == selectedNumber[j])) //check selectedNumber and hitNumber strings
							{
								runwayCheck = true;
								break;
							}
							else
							{
								runwayCheck = false;
							}
						}
					}
				}
				else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
				{//check if selectedObj is Runway_TouchDownZOne and collidedObj is runwayNumber tagged
					runwayCheck = true;
					break;
				}
				else if ((lvlm.selectedObj.LObject.name == selectedRunway[i] && hit.collider.gameObject.name == hitRunway[i] )//check selectedNumber and hitNumber strings
					|| (lvlm.selectedObj.LObject.name == hitRunway[i] && hit.collider.gameObject.name == selectedRunway[i]))
				{
					runwayCheck = true;
					break;
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else if (Physics.Raycast(lastPos, transform.right, out hit, 1, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
				|| Physics.Raycast(lastPos, transform.forward, out hit, 1, LayerMask.GetMask("runwayMarkings", "runwayNumber")))
		{//raycasts right and forward in layers runwayMarkings and runwayNumber
			for (int i = 0; i < selectedRunway.Length; i++)
			{
				if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
				{//check if the selectedObj is runwayNumber tagged and the collidedObj is Runway_Threshold
					RaycastHit hitTwo;
					if (Physics.Raycast(lastPos, -transform.right, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
						|| Physics.Raycast(lastPos, -transform.forward, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))) //raycasts left and back in layer runwayNumber
					{
						for (int j = 0; j < selectedRunway.Length; j++)
						{
							if ((lvlm.selectedObj.LObject.name == selectedNumber[j] && hitTwo.collider.gameObject.name == hitNumber[j])
								|| (lvlm.selectedObj.LObject.name == hitNumber[j] && hitTwo.collider.gameObject.name == selectedNumber[j])) //check selectedNumber and hitNumber strings
							{
								runwayCheck = true;
								break;
							}
							else
							{
								runwayCheck = false;
							}
						}
					}
					else if (Physics.Raycast(lastPos, transform.right, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
						|| Physics.Raycast(lastPos, transform.forward, out hitTwo, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))) //raycasts right and forward in layer runwayNumber
					{
						for (int j = 0; j < selectedRunway.Length; j++)
						{
							if ((lvlm.selectedObj.LObject.name == selectedNumber[j] && hitTwo.collider.gameObject.name == hitNumber[j])
								|| (lvlm.selectedObj.LObject.name == hitNumber[j] && hitTwo.collider.gameObject.name == selectedNumber[j])) //check selectedNumber and hitNumber strings
							{
								runwayCheck = true;
								break;
							}
							else
							{
								runwayCheck = false;
							}
						}
					}
				}
				else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
				{//check if selectedObj is Runway_TouchDownZOne and collidedObj is runwayNumber tagged
					runwayCheck = true;
					break;
				}
				else if ((lvlm.selectedObj.LObject.name == selectedRunway[i] && hit.collider.gameObject.name == hitRunway[i]) //check selectedRunway and hitRunway strings
					|| (lvlm.selectedObj.LObject.name == hitRunway[i] && hit.collider.gameObject.name == selectedRunway[i]))
				{
					runwayCheck = true;
					break;
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else if (lvlm.selectedObj.LObject.name == "Runway_BlastPad" || lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold")
		{//check if selectedObj is Runway_Blastpad/DisplacedThreshold if no raycast was raycasted due to no object
			runwayCheck = true;
		}
		else
		{
			runwayCheck = false;
		}
		return runwayCheck;
	}

	bool CheckAdjacent() //check the adjacent objects when trying to place a specified object 
	{
		bool adjacentCheck = true;
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f);
		if (gm.currentNode.nObjects.Count > 0)
		{
			foreach (LevelObject go in gm.currentNode.nObjects) //adds 0.9 to y coordinate of lastPos for each object present on the grid
			{
				lastPos.y += 0.9f;
			}
		}
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.transform.position.y == lastPos.y) //ensures that only the objects on the same y level can be detected by OverlapSphere
			{
				print(collided.gameObject.name);
				if (!(collided.gameObject.transform.position.x == lastPos.x && collided.gameObject.transform.position.z == lastPos.z - 0.5f)) //ignores selectedObj itself
				{		
					for (int i = 0; i < selectedObjectTag.Length; i++)
					{
						if ((lvlm.selectedObj.LObject.tag == selectedObjectTag[i] && collided.gameObject.tag == hitObjectTag[i]) 
							|| (lvlm.selectedObj.LObject.tag == hitObjectTag[i] && collided.gameObject.tag == selectedObjectTag[i])) //checks selectedObjectTag and hitObjectTag strings
						{
							adjacentCheck = false;
							break;
						}
						else //passed the first checking of tags (adjacent)
						{					
							for (int j = 0; j < selectedObject.Length; j++)
							{
								if ((lvlm.selectedObj.LObject.name == selectedObject[j] && collided.gameObject.name == hitObject[j])
									|| (lvlm.selectedObj.LObject.name == hitObject[j] && collided.gameObject.name == selectedObject[j])) //checks selectedObject and hitObject strings
								{
									adjacentCheck = false;
									break;
								}
								else //passed the second checking of names (adjacent)
								{
									if (collided.gameObject.transform.position.x == lastPos.x || collided.gameObject.transform.position.z == lastPos.z-0.5f)
									{
										for (int m = 0; m < TObjectOne.Length; m++)
										{
											if ((lvlm.selectedObj.LObject.name == TObjectOne[m] && collided.gameObject.name == TObjectTwo[m])
												|| (lvlm.selectedObj.LObject.name == TObjectTwo[m] && collided.gameObject.name == TObjectOne[m])) //checks TObjectOneand TObjectTwo strings
											{
												adjacentCheck = false;
												break; 
											}
											else //passed the third checking of names (ignores diagonal)
											{
												adjacentCheck = true;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return adjacentCheck;
	}




	public void PlaceObjectClone() //place object more than once
	{
		bool adjacentResult = true;
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1 && gm.currentNode.nObjects.Last().LObjectType != 4) //if the bottom object allows stacking
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if ((lvlm.selectedObj.LObject.tag == "apronOnly" || lvlm.selectedObj.LObject.tag == "hangar") && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects and building on apron
					{
						print("This can only be placed on an Apron tile.");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "vehicle")
					{
						vehicle = GameObject.FindGameObjectsWithTag("vehicle");
						if (GameObject.FindGameObjectsWithTag("vehicle").Length > 1)
						{
							print("You can only place 1 PushBack Vehicle.");
							lvlm.CancelSelect();						
						}
						else
						{
							lvlm.CloneSucceed();
						}
					}
					else
					{
						adjacentResult = CheckAdjacent();
						if (adjacentResult == false)
						{
							print("You cannot place the selected object here.");
							lvlm.CancelSelect();	
						}
						else
						{
							lvlm.CloneSucceed();
						}
					}
				}
				else if (lvlm.selectedObj.LObjectType == 2) //selected object is stackable
				{
					lvlm.CloneSucceed();
				}
				else if (lvlm.selectedObj.LObjectType == 3 || lvlm.selectedObj.LObjectType == 4) //selected object is base-only or base-only-no-stack
				{
					print("This can only be used at ground level");
					lvlm.CancelSelect();
				}
			}
			else
			{
				print("You cannot place anything on this tile.");
				lvlm.CancelSelect();
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2) //if object type 1, 2, or 3 is selected 
			{
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else //if object type 3 or 4 is selected
			{
				adjacentResult = CheckAdjacent();
				if (adjacentResult == false)
				{
					print("You cannot place the selected object here.");
					lvlm.CancelSelect();
				}
				else if (adjacentResult == true && (lvlm.selectedObj.LObject.tag == "runway" || lvlm.selectedObj.LObject.tag == "runwayNumber")) //if CheckAdjacent passed and selectedObj is runway object, run CheckRunway
				{
					bool runwayResult = true;
					runwayResult = CheckRunway();
					if (runwayResult == false)
					{
						print("Check your Runway objects' placement and the Runway Number.");
						lvlm.CancelSelect();
					}
					else
					{
						lvlm.CloneSucceed();
					}
				}
				else //if CheckAjacent passed and selectedObj is not runway object, place object
				{
					lvlm.CloneSucceed();
				}
			}
		}
	}


	public void PlaceObjectSingle() //place ONE object and no clone
	{
		bool adjacentResult = true;
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1 && gm.currentNode.nObjects.Last().LObjectType != 4) //if the bottom object allows stacking
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if ((lvlm.selectedObj.LObject.tag == "apronOnly" || lvlm.selectedObj.LObject.tag == "hangar") && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects and building on apron
					{
						print("This can only be placed on an Apron tile.");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "vehicle")
					{
						vehicle = GameObject.FindGameObjectsWithTag("vehicle");
						if (GameObject.FindGameObjectsWithTag("vehicle").Length > 1)
						{
							print("You can only place 1 PushBack Vehicle.");
							lvlm.CancelSelect();
						}
						else
						{
							adjacentResult = CheckAdjacent();
							if (adjacentResult == false)
							{
								print("You cannot place the selected object here.");
								lvlm.CancelSelect();
							}
							else
							{
								lvlm.PlaceSucceed();
							}
						}
					}
					else
					{
						adjacentResult = CheckAdjacent();
						if (adjacentResult == false)
						{
							print("You cannot place the selected object here.");
							lvlm.CancelSelect();	
						}
						else
						{
							lvlm.PlaceSucceed();
						}
					}
				}
				else if (lvlm.selectedObj.LObjectType == 2) //selected object is stackable
				{
					lvlm.PlaceSucceed();
				}
				else if (lvlm.selectedObj.LObjectType == 3 || lvlm.selectedObj.LObjectType == 4) //selected object is base-only or base-only-no-stack
				{
					print("This can only be used at ground level");
					lvlm.CancelSelect();
				}
			}
			else
			{
				print("You cannot place anything on this tile.");
				lvlm.CancelSelect();
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2) //if object type 1, 2, or 3 is selected 
			{
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else //if object type 3 or 4 is selected
			{
				adjacentResult = CheckAdjacent();
				if (adjacentResult == false)
				{
					print("You cannot place the selected object here.");
					lvlm.CancelSelect();
				}
				else if (adjacentResult == true && (lvlm.selectedObj.LObject.tag == "runway" || lvlm.selectedObj.LObject.tag == "runwayNumber")) //if CheckAdjacent passed and selectedObj is runway object, run CheckRunway
				{
					bool runwayResult = true;
					runwayResult = CheckRunway();
					if (runwayResult == false)
					{
						print("Check your Runway objects' placement and the Runway Number.");
						lvlm.CancelSelect();
					}
					else
					{
						lvlm.PlaceSucceed();
					}
				}
				else //if CheckAjacent passed and selectedObj is not runway object, place object
				{
					lvlm.PlaceSucceed();
				}
			}
		}
	}
}

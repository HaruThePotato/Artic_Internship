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

	string[] selectedObject = new string[] { "Taxi_Line" };
	string[] hitObject = new string[] { "Runway_BlastPad" };

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




	/*void CheckRunway() //check the runway lane for correct order. This is super long because of all the double checking and in opposite order.
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f); //Extra 0.5f for correction of 0.5 in Rot and Collider
		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
		{
			if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
			{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				CheckRunwayNumber();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
					|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast right and forward
				{
					if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
					{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						CheckRunwayNumber();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
					{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
				|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast right and forward
		{
			if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
			{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				CheckRunwayNumber();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
					|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
				{
					if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
					{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						CheckRunwayNumber();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
					{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else
		{
			runwayCheck = false;
		}
	}

	void CheckRunwayNumber()
	{
		print("checkrunwaynumber");
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f); //Extra 0.5f for correction of 0.5 in Rot and Collider

		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
		{
			if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
			{
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
				{
					if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
					{
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
		{
			if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
			{
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
				{
					if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
					{
						runwayCheck = true; ;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
					{
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else
		{
			runwayCheck = true;
		}
	}*/

	bool CheckAdjacent() //check the adjacent objects when trying to place a specified object 
	{
		bool adjacentCheck = true;
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ);
		if (gm.currentNode.nObjects.Count > 0)
		{
			foreach (LevelObject go in gm.currentNode.nObjects)
			{
				lastPos.y += go.LObject.transform.GetComponentInChildren<MeshRenderer>().bounds.size.y;
			}
		}
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.name != "GridCollider" && collided.gameObject.name != "Object" && collided.gameObject.transform.position != lastPos)
			{
				print(collided.gameObject.name);
				for (int i = 0; i < selectedObject.Length; i++)
				{
					if (lvlm.selectedObj.LObject.name == selectedObject[i] && collided.gameObject.name == hitObject[i])
					{
						print("false");
						adjacentCheck = false;
						break;
					}
					else
					{
						print("true");
						adjacentCheck = true;
					}
				}

			}
		}
		return adjacentCheck;
	}




	
	/*public void PlaceObjectClone() //place object more than once
	{
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
					else
					{
						CheckAdjacent();
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
				CheckAdjacent();
			}
		}
	}*/


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
					else
					{
						adjacentResult = CheckAdjacent();
						if (adjacentResult == false)
						{
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
					lvlm.CancelSelect();
				}
				else
				{
					lvlm.PlaceSucceed();
				}
			}
		}
	}

}

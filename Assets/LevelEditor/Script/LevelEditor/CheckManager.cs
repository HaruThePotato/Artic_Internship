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

	bool adjacentCheck = false;
	bool runwayCheck = false;
	bool rangeCheck = false;

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

	void CheckAdjacent() //check the adjacent objects when trying to place a specified object
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ);
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid.
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.transform.position.x == lastPos.x || collided.gameObject.transform.position.z == lastPos.z) //check for same row or column, ignoring diagonal
			{
				if (lvlm.selectedObj.LObject.name == "Runway_BlastPad" && collided.gameObject.tag == "taxiway") //if blastpad was selected and taxiway is around
				{
					adjacentCheck = false;
					print("You cannot place that near a taxiway");
					break;
				}
				else if (lvlm.selectedObj.LObject.tag == "taxiway" && collided.gameObject.name == "Runway_BlastPad") //if taxyiway is selected and blastpad is around
				{
					adjacentCheck = false;
					print("You cannot place that near a blastpad");
					break;
				}
				else if (lvlm.selectedObj.LObject.tag == "runway" && (collided.gameObject.tag == "roadway" || collided.gameObject.tag == "apron"))
				{//if runway is selected and roadway/apron is around
					adjacentCheck = false;
					print("You cannot place that near a plane roadway");
					break;
				}
				else if ((lvlm.selectedObj.LObject.tag == "roadway" || lvlm.selectedObj.LObject.tag == "apron") && collided.gameObject.tag == "runway") //if roadway is selected and runway is around
				{
					adjacentCheck = false;
					print("You cannot place that near a runway");
					break;
				}
				else if (lvlm.selectedObj.LObject.name == "HoldingShortLine") //if holding short line is selected and check for taxiway
				{
					if (collided.gameObject.tag == "taxiway")
					{
						adjacentCheck = true;
						break;
					}
					else
					{
						adjacentCheck = false;
					}
				}
				else
				{
					adjacentCheck = true;
				}
			}
		}
	}

	void CheckRunway() //check the runway lane for correct order. This is super long because of all the double checking and in opposite order.
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ);
		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))) //raycast left and back
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
			else if (lvlm.selectedObj.LObject.name == "Runway_RunwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.name == "Runway_RunwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchdownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))
					|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))) //raycast right and forward
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
					else if (lvlm.selectedObj.LObject.name == "Runway_RunwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.name == "Runway_RunwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchdownZone")
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
		else if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))
				|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))) //raycast right and forward
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
			else if (lvlm.selectedObj.LObject.name == "Runway_RunwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.name == "Runway_RunwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchdownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))
					|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings"))) //raycast left and back
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
					else if (lvlm.selectedObj.LObject.name == "Runway_RunwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.name == "Runway_RunwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchdownZone")
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

	void CheckAdjacentTrue() //build if CheckAdjacent passes
	{
		if (adjacentCheck == true)
		{
			lvlm.PlaceSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckAdjacentTrueClone()
	{
		if (adjacentCheck == true)
		{
			lvlm.CloneSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckAdjacentFalse() //nothing built and cancel selection if CheckAdjacent fails
	{
		adjacentCheck = false;
		lvlm.CancelSelect();
	}

	void CheckRunwayTrue() //place runway marking if runwayCheck returns true, cancel selection if returns false
	{
		if (runwayCheck == true)
		{
			lvlm.PlaceSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckRunwayTrueClone() //clone and place runway marking if runwayCheck returns true, cancel selection if returns false
	{
		if (runwayCheck == true)
		{
			lvlm.CloneSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	public void PlaceObjectClone() //clone and place object
	{
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1) //if the bottom object allows stacking
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if (gm.currentNode.nObjects.Last().LObjectType == 4) //bottom object is BaseOnlyNonStackable
					{
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
						{
							print("This can only be built in apron area");
							lvlm.CancelSelect();
						}
						else
						{
							lvlm.CloneSucceed();
						}
					}
					else //bottom object type is 2 or 3
					{
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
						{
							print("This can only be built in apron area");
							lvlm.CancelSelect();
						}
						else if (lvlm.selectedObj.LObject.tag == "plane" && (gm.currentNode.nObjects.Last().LObject.tag == "grass" || gm.currentNode.nObjects.Last().LObject.tag == "roadway"))
						{ //selected plane but try to build on grass plain or plane roadway
							lvlm.CancelSelect();
						}
						else if (lvlm.selectedObj.LObject.tag == "vehicle" && gm.currentNode.nObjects.Last().LObject.tag == "grass")
						{ //selected vehicle but try to build on grass plain
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
				else if (lvlm.selectedObj.LObjectType == 3) //selected object is base-only
				{
					print("That can only be used at ground level");
					lvlm.CancelSelect();
				}
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2)
			{ //if object type 1, 2, or 3 is selected 
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else if (lvlm.selectedObj.LObjectType == 3 || lvlm.selectedObj.LObjectType == 4) //if object type 3 or 4 is selected
			{
				if (lvlm.selectedObj.LObject.tag == "runway") //specifically runway
				{
					if (lvlm.selectedObj.LObject.name == "Runway_BlastPad" || lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold" || lvlm.selectedObj.LObject.name == "Runway_Line")
					{//BlastPad, DisplacedThreshold or Line selected, no need to CheckRunway
						CheckAdjacent();
						CheckAdjacentTrueClone();
					}
					else
					{
						CheckAdjacent();
						if (adjacentCheck == true)
						{
							CheckRunway();
							CheckRunwayTrueClone();
						}
					}
				}
				else if (lvlm.selectedObj.LObject.tag == "roadway") //specifically roadway
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}
				else
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}
			}
		}
	}

	public void PlaceObjectSingle() //place ONE object and no clone
	{
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1) //if the bottom object allows stacking
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if (gm.currentNode.nObjects.Last().LObjectType == 4) //bottom object is BaseOnlyNonStackable
					{
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
						{
							print("This can only be built in apron area");
							lvlm.CancelSelect();
						}
						if (lvlm.selectedObj.LObject.tag == "plane")
						{
							lvlm.CancelSelect();
						}
						else
						{
							lvlm.PlaceSucceed();
						}
					}
					else //bottom object type is 2 or 3
					{
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
						{
							print("This can only be built in apron area");
							lvlm.CancelSelect();
						}
						else if (lvlm.selectedObj.LObject.tag == "plane" && (gm.currentNode.nObjects.Last().LObject.tag == "grass" || gm.currentNode.nObjects.Last().LObject.tag == "roadway"))
						{ //selected plane but try to build on grass plain or plane roadway
							lvlm.CancelSelect();
						}
						else if (lvlm.selectedObj.LObject.tag == "vehicle" && gm.currentNode.nObjects.Last().LObject.tag == "grass")
						{ //selected vehicle but try to build on grass plain
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
				else if (lvlm.selectedObj.LObjectType == 3) //selected object is base-only
				{
					print("That can only be used at ground level");
					lvlm.CancelSelect();
				}
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2)
			{ //if object type 1, 2, or 3 is selected 
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else if (lvlm.selectedObj.LObjectType == 3 || lvlm.selectedObj.LObjectType == 4) //if object type 3 or 4 is selected
			{
				if (lvlm.selectedObj.LObject.tag == "runway") //specifically runway
				{
					if (lvlm.selectedObj.LObject.name == "Runway_BlastPad" || lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold" ||  lvlm.selectedObj.LObject.name == "Runway_Line")
					{//BlastPad, DisplacedThreshold or Line selected, no need to CheckRunway
						CheckAdjacent();
						CheckAdjacentTrue();
					}
					else
					{
						CheckAdjacent();
						if (adjacentCheck == true)
						{
							CheckRunway();
							CheckRunwayTrue();
						}
					}	
				}
				else if (lvlm.selectedObj.LObject.tag == "roadway") //specifically roadway
				{
					CheckAdjacent();
					CheckAdjacentTrue();
				}
				else
				{
					CheckAdjacent();
					CheckAdjacentTrue();
				}
			}
		}
	}

}

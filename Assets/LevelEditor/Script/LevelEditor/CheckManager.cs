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




	void CheckRunway() //check the runway lane for correct order. This is super long because of all the double checking and in opposite order.
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f); //Extra 0.5f for correction of 0.5 in Rot and Collider
		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
		{
			if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
			{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				CheckRunwayNumber();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				CheckRot();
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
					|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast right and forward
				{
					if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
					{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						CheckRunwayNumber();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
					{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
						CheckRot();
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
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				CheckRunwayNumber();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
			{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
				CheckRot();
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
					|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
				{
					if (lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold2" && hit.collider.gameObject.name == "Runway_DisplacedThreshold")
					{//selected object is DisplacedThreshold2 and raycast hits DisplacedThreshold
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_Threshold" && (hit.collider.gameObject.name == "Runway_BlastPad" || hit.collider.gameObject.name == "Runway_DisplacedThreshold2"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.tag == "runwayNumber" && hit.collider.gameObject.name == "Runway_Threshold")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						CheckRunwayNumber();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_TouchDownZone" && hit.collider.gameObject.tag == "runwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_AimingPoint" && hit.collider.gameObject.name == "Runway_TouchDownZone")
					{ //selected object is AimingPointMarker and raycast hits TouchdownZoneMarker
						CheckRot();
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
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
			{
				CheckRot();
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
				{
					if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
					{
						CheckRot();
					}
				}
				else
				{
					CheckRot();
				}
			}
		}
		else if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
		{
			if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
			{
				CheckRot();
			}
			else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
			{
				CheckRot();
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
				{
					if (lvlm.selectedObj.LObject.name == "Runway_9" && hit.collider.gameObject.name == "Runway_27")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9C" && hit.collider.gameObject.name == "Runway_27C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9L" && hit.collider.gameObject.name == "Runway_27R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_9R" && hit.collider.gameObject.name == "Runway_27L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18" && hit.collider.gameObject.name == "Runway_36")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18C" && hit.collider.gameObject.name == "Runway_36C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18L" && hit.collider.gameObject.name == "Runway_36R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_18R" && hit.collider.gameObject.name == "Runway_36L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27" && hit.collider.gameObject.name == "Runway_9")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27C" && hit.collider.gameObject.name == "Runway_9C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27L" && hit.collider.gameObject.name == "Runway_9R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_27R" && hit.collider.gameObject.name == "Runway_9L")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36" && hit.collider.gameObject.name == "Runway_18")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36C" && hit.collider.gameObject.name == "Runway_18C")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36L" && hit.collider.gameObject.name == "Runway_18R")
					{
						CheckRot();
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_36R" && hit.collider.gameObject.name == "Runway_18L")
					{
						CheckRot();
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
			CheckRot();
		}
	}

	void CheckAdjacent() //check the adjacent objects when trying to place a specified object
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f);
		if (gm.currentNode.nObjects.Count > 0)
		{
			lastPos.y += lvlm.selectedObj.LObject.transform.GetComponentInChildren<MeshRenderer>().bounds.size.y;
		}
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.name != "GridCollider")
			{
				if (collided.gameObject.transform.position.x == lastPos.x || collided.gameObject.transform.position.z == lastPos.z - 0.5f)//check for same row or column, ignoring diagonal
				{
					if (lvlm.selectedObj.LObject.tag == "runway" && (collided.gameObject.tag == "roadway" || collided.gameObject.tag == "apron"))
					{//if runway is selected and roadway/apron is around
						adjacentCheck = false;
						print("You cannot place that near a roadway");
						break;
					}
					else if (lvlm.selectedObj.LObject.name == "Runway_BlastPad") //if blastpad was selected and taxiway is around
					{
						if (collided.gameObject.tag == "taxiway")
						{
							adjacentCheck = false;
							print("You cannot place that near a taxiway");
							break;
						}
						else if (collided.gameObject.name == "Runway_BlastPad")
						{
							CheckRot();
							break;
						}
						else
						{
							adjacentCheck = true;
						}
					}
					else if (lvlm.selectedObj.LObject.gameObject.name == "Runway_DisplacedThreshold")
					{
						if (!(collided.gameObject.transform.position.x == lastPos.x && collided.gameObject.transform.position.z == lastPos.z - 0.5f))
						{
							if (collided.gameObject.name == "Runway_DisplacedThreshold")
							{
								print("YATA");
								adjacentCheck = false;
								break;
							}
							else
							{
								print("good");
								CheckRot();
							}
						}
						else
						{
							print("YES");
							adjacentCheck = true;
						}
					}
					else if (lvlm.selectedObj.LObject.tag == "taxiway" && collided.gameObject.name == "Runway_BlastPad") //if taxyiway is selected and blastpad is around
					{
						adjacentCheck = false;
						print("You cannot place that near a blastpad");
						break;
					}
					
					else if ((lvlm.selectedObj.LObject.tag == "roadway" || lvlm.selectedObj.LObject.tag == "apron") && collided.gameObject.tag == "runway") //if roadway is selected and runway is around
					{
						adjacentCheck = false;
						print("You cannot place that near a runway");
						break;
					}
					else if (lvlm.selectedObj.LObject.tag == "apronOnly")
					{
						if (lvlm.selectedObj.LObject.name == "Apron_GateBridgeLight" && (collided.gameObject.name == "Terminal_Corner1" || collided.gameObject.name == "Terminal_Corner2" || collided.gameObject.name == "Terminal_End"))
						{
							CheckRot();
							break;
						}
						else if (lvlm.selectedObj.LObject.name == "Radio_Tower")
						{
							if (!(collided.gameObject.transform.position.x == lastPos.x && collided.gameObject.transform.position.z == lastPos.z - 0.5f))
							{
								if (collided.gameObject.tag == "apronOnly" || collided.gameObject.tag == "hangar")
								{
									adjacentCheck = false;
									break;
								}
								else
								{
									adjacentCheck = true;
								}
							}		
						}
						else if (lvlm.selectedObj.LObject.name != "Terminal_Middle" && lvlm.selectedObj.LObject.name != "Apron_GateBridgeLight" && lvlm.selectedObj.LObject.name != "Radio_Tower")
						{
							if (collided.gameObject.name != "Radio_Tower")
							{
								if (collided.gameObject.name != "Apron_Main")
								{
									if ((collided.gameObject.transform.position.x != lastPos.x) || (collided.gameObject.transform.position.z != lastPos.z - 0.5f))
									{
										CheckRot();
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
							else
							{
								adjacentCheck = false;
								break;
							}
						}
						else if (lvlm.selectedObj.LObject.name == "Terminal_Middle")
						{
							if (collided.gameObject.name != "Radio_Tower")
							{
								if (collided.gameObject.name == "Terminal_Corner1" || collided.gameObject.name == "Terminal_Corner2")
								{
									adjacentCheck = false;
									break;
								}
								else if (collided.gameObject.name == "Terminal_End")
								{
									CheckRot();
									break;
								}
								else if (collided.gameObject.name == "Terminal_Middle" && !(collided.gameObject.transform.position.x == lastPos.x && collided.gameObject.transform.position.z == lastPos.z - 0.5f))
								{
									adjacentCheck = true;
									break;
								}
								else
								{
									adjacentCheck = false;
									print("Start building with another Terminal block first");
								}
							}
							else
							{
								adjacentCheck = false;
								break;
							}
						}
						else if (lvlm.selectedObj.LObject.name == "Terminal_MiddleEnd")
						{
							if (collided.gameObject.name == "Terminal_Corner1" || collided.gameObject.name == "Terminal_Corner2")
							{
								CheckRot();
								break;
							}
							else
							{
								adjacentCheck = false;
							}
						}
						else if ((lvlm.selectedObj.LObject.name == "Terminal_Corner1" || lvlm.selectedObj.LObject.name == "Terminal_Corner2") && (collided.gameObject.name == "Terminal_Middle" || collided.gameObject.name != "Radio_Tower"))
						{
							adjacentCheck = false;
						}
						else
						{
							adjacentCheck = false;
						}
					}
					else if (lvlm.selectedObj.LObject.name == "Apron_Taxi")
					{
						if ((collided.gameObject.name == "Apron_Main" || collided.gameObject.tag == "taxiway")
							|| (collided.gameObject.name == "Apron_Taxi" && collided.gameObject.transform.position != lastPos))
						{
							adjacentCheck = true;
							break;
						}
						else
						{
							adjacentCheck = false;
						}
					}
					else if (lvlm.selectedObj.LObject.name == "Grass_Taxi")
					{
						if ((collided.gameObject.name == "Grass_Main" || collided.gameObject.tag == "taxiway")
							|| (collided.gameObject.name == "Grass_Taxi" && collided.gameObject.transform.position != lastPos))
						{
							adjacentCheck = true;
							break;
						}
						else
						{
							adjacentCheck = false;
						}
					}
					else if (lvlm.selectedObj.LObject.name == "Grass_Runway")
					{
						if ((collided.gameObject.name == "Grass_Main" || collided.gameObject.tag == "runway")
							|| (collided.gameObject.name == "Grass_Runway" && collided.gameObject.transform.position != lastPos))
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
						print("no1");
						adjacentCheck = true;
					}
				}
			}
			else
			{
				adjacentCheck = false;
			}
		}
	}

	void CheckRot()
	{
		print("CheckRot");
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ + 0.5f);
		if (gm.currentNode.nObjects.Count > 0)
		{
			lastPos.y += lvlm.selectedObj.LObject.transform.GetComponentInChildren<MeshRenderer>().bounds.size.y;
		}
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid.
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.name != "GridCollider")
			{
				if (collided.gameObject.transform.position.x == lastPos.x && collided.gameObject.transform.position.z != lastPos.z - 0.5f)
				{//check for same row or column, ignoring diagonal
					float difference = collided.gameObject.transform.position.z - (lastPos.z - 0.5f);
					int selectedObjectAngle = (int)lvlm.hObject.LObject.transform.GetChild(0).transform.eulerAngles.y;
					int collidedObjectAngle = (int)collided.gameObject.transform.GetChild(0).transform.eulerAngles.y;
					if (lvlm.selectedObj.LObject.tag == "runway" || lvlm.selectedObj.LObject.tag == "runwayNumber")
					{
						if (collided.gameObject.tag == "runway" || collided.gameObject.tag == "runwayNumber")
						{
							if (difference < 0)
							{
								if ((selectedObjectAngle == 90 && collidedObjectAngle == 90) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
								{
									runwayCheck = true;
									break;
								}
								else
								{
									runwayCheck = false;
								}
							}
							else
							{
								if ((selectedObjectAngle == 270 && collidedObjectAngle == 270) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
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
					else if (lvlm.selectedObj.LObject.tag == "apronOnly")
					{
						if (collided.gameObject.tag == "apronOnly" && collided.gameObject.name != "Apron_Main")
						{
							if (lvlm.selectedObj.LObject.name == "Terminal_Middle")
							{
								if (difference < 0)
								{
									if (collidedObjectAngle == 0 && lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1)
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
								else if (difference > 0)
								{
									if (collidedObjectAngle == 180 && lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1)
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Terminal_MiddleEnd")
							{
								if (collided.gameObject.name == "Terminal_Corner1")
								{
									if (difference < 0)
									{
										if ((collidedObjectAngle == 0 && selectedObjectAngle == 270) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((collidedObjectAngle == 180 && selectedObjectAngle == 90) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
								else if (collided.gameObject.name == "Terminal_Corner2")
								{
									if (difference < 0)
									{
										if ((collidedObjectAngle == 0 && selectedObjectAngle == 90) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((collidedObjectAngle == 180 && selectedObjectAngle == 270) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Terminal_End")
							{
								if (collided.gameObject.name == "Terminal_Middle")
								{
									if (difference < 0)
									{
										if (selectedObjectAngle == 180 && lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1)
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if (selectedObjectAngle == 0 && lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1)
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
								else
								{
									if (difference < 0)
									{
										if (((selectedObjectAngle == 90 && collidedObjectAngle == 90) || (selectedObjectAngle == 270 && collidedObjectAngle == 270))
											&& (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if (((selectedObjectAngle == 90 && collidedObjectAngle == 90) || (selectedObjectAngle == 270 && collidedObjectAngle == 270))
											&& (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Apron_GateBridgeLight")
							{
								if (collided.gameObject.name == "Terminal_End" || collided.gameObject.name == "Terminal_Corner1" || collided.gameObject.name == "Terminal_Corner2")
								{

									if (difference < 0)
									{
										if ((selectedObjectAngle == 180 && collidedObjectAngle == 180) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((selectedObjectAngle == 0 && collidedObjectAngle == 0) && (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else
							{
								if (difference < 0)
								{
									if (((selectedObjectAngle == 90 && collidedObjectAngle == 90) || (selectedObjectAngle == 270 && collidedObjectAngle == 270))
										&& (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z + 1))
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
								else if (difference > 0)
								{
									if (((selectedObjectAngle == 90 && collidedObjectAngle == 90) || (selectedObjectAngle == 270 && collidedObjectAngle == 270))
										&& (lvlm.hObject.LObject.transform.position.z == collided.gameObject.transform.position.z - 1))
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
							}
						}
					}
				}
				else if (collided.gameObject.transform.position.z == lastPos.z - 0.5f && collided.gameObject.transform.position.x != lastPos.x)
				{//check for same row or column, ignoring diagonal
					float difference = collided.gameObject.transform.position.x - lastPos.x;
					int selectedObjectAngle = (int)lvlm.hObject.LObject.transform.GetChild(0).transform.eulerAngles.y;
					int collidedObjectAngle = (int)collided.gameObject.transform.GetChild(0).transform.eulerAngles.y;
					if (lvlm.selectedObj.LObject.tag == "runway" || lvlm.selectedObj.LObject.tag == "runwayNumber")
					{
						if (collided.gameObject.tag == "runway" || collided.gameObject.tag == "runwayNumber")
						{
							if (difference < 0)
							{
								if ((selectedObjectAngle == 180 && collidedObjectAngle == 180) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
								{
									runwayCheck = true;
									break;
								}
								else
								{
									runwayCheck = false;
								}
							}
							else if (difference > 0)
							{
								if ((selectedObjectAngle == 0 && collidedObjectAngle == 0) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
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
					else if (lvlm.selectedObj.LObject.tag == "apronOnly")
					{
						if (collided.gameObject.tag == "apronOnly" && collided.gameObject.name != "Apron_Main")
						{
							if (lvlm.selectedObj.LObject.name == "Terminal_Middle")
							{
								if (difference < 0)
								{
									if (collidedObjectAngle == 90 && lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1)
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
								else if (difference > 0)
								{
									if (collidedObjectAngle == 270 && lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1)
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Terminal_MiddleEnd")
							{
								if (collided.gameObject.name == "Terminal_Corner1")
								{
									if (difference < 0)
									{
										if ((collidedObjectAngle == 90 && selectedObjectAngle == 0) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((collidedObjectAngle == 270 && selectedObjectAngle == 180) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
								else if (collided.gameObject.name == "Terminal_Corner2")
								{
									if (difference < 0)
									{
										if ((collidedObjectAngle == 90 && selectedObjectAngle == 180) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((collidedObjectAngle == 270 && selectedObjectAngle == 0) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Terminal_End")
							{
								if (collided.gameObject.name == "Terminal_Middle")
								{
									if (difference < 0)
									{
										if (selectedObjectAngle == 270 && lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1)
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if (selectedObjectAngle == 90 && lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1)
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
								else
								{
									if (difference < 0)
									{
										if (((selectedObjectAngle == 0 && collidedObjectAngle == 0) || (selectedObjectAngle == 180 && collidedObjectAngle == 180))
											&& (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if (((selectedObjectAngle == 0 && collidedObjectAngle == 0) || (selectedObjectAngle == 180 && collidedObjectAngle == 180))
											&& (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else if (lvlm.selectedObj.LObject.name == "Apron_GateBridgeLight")
							{
								if (collided.gameObject.name == "Terminal_End" || collided.gameObject.name == "Terminal_Corner1" || collided.gameObject.name == "Terminal_Corner2")
								{
									if (difference < 0)
									{
										if ((selectedObjectAngle == 270 && collidedObjectAngle == 270) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
									else if (difference > 0)
									{
										if ((selectedObjectAngle == 90 && collidedObjectAngle == 90) && (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
										{
											adjacentCheck = true;
											break;
										}
										else
										{
											adjacentCheck = false;
										}
									}
								}
							}
							else
							{
								if (difference < 0)
								{
									if (((selectedObjectAngle == 0 && collidedObjectAngle == 0) || (selectedObjectAngle == 180 && collidedObjectAngle == 180))
										&& (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x + 1))
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
								else if (difference > 0)
								{
									if (((selectedObjectAngle == 0 && collidedObjectAngle == 0) || (selectedObjectAngle == 180 && collidedObjectAngle == 180))
										&& (lvlm.hObject.LObject.transform.position.x == collided.gameObject.transform.position.x - 1))
									{
										adjacentCheck = true;
										break;
									}
									else
									{
										adjacentCheck = false;
									}
								}
							}
						}
					}
				}
			}
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

	public void PlaceObjectClone() //place ONE object and no clone
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
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag == "apron") //selected apronOnly objects and building on apron
						{
							if (lvlm.selectedObj.LObject.name == "Apron_GateBridgeLight") //selected GateBridgeLight
							{
								CheckAdjacent();
								if (adjacentCheck == false)
								{
									print("You need to start building with a Terminal block first.");
								}
								CheckAdjacentTrueClone();
							}
							else
							{
								lvlm.CloneSucceed();
							}
						}
						else if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
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
							CheckRunwayTrue();
						}
					}
				}
				else if (lvlm.selectedObj.LObject.tag == "roadway") //specifically roadway
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}
				else if (lvlm.selectedObj.LObject.name == "Apron_Taxi") //specifically ApronTaxi (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Apron or Taxiway block beside it.");
					}
					CheckAdjacentTrueClone();
				}
				else if (lvlm.selectedObj.LObject.name == "Grass_Taxi") //specifically GrassTaxi (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Grass or Taxiway block beside it.");
					}
					CheckAdjacentTrueClone();
				}
				else if (lvlm.selectedObj.LObject.name == "Grass_Runway")//specifically GrassRunway (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Grass or Runway block beside it.");
					}
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
						if ((lvlm.selectedObj.LObject.tag == "apronOnly" || lvlm.selectedObj.LObject.tag == "hangar") && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
						{
							print("This can only be built in apron area");
							lvlm.CancelSelect();
						}
						else
						{
							CheckAdjacent();
							CheckAdjacentTrue();
						}
					}
					else //bottom object type is 2 or 3
					{
						if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag == "apron") //selected apronOnly objects and building on apron
						{
							if (lvlm.selectedObj.LObject.name == "Apron_GateBridgeLight") //selected GateBridgeLight
							{
								CheckAdjacent();
								if (adjacentCheck == false)
								{
									print("You need to start building with a Terminal block first and make sure it is connected to the terminal properly.");
								}
								CheckAdjacentTrue();
							}
							else
							{
								CheckAdjacent();
								CheckAdjacentTrue();
							}
						}
						else if (lvlm.selectedObj.LObject.tag == "hangar" && gm.currentNode.nObjects.Last().LObject.tag == "apron")
						{
							lvlm.PlaceSucceed();
						}
						else if ((lvlm.selectedObj.LObject.tag == "apronOnly" || lvlm.selectedObj.LObject.tag == "hangar") && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
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
				if (lvlm.selectedObj.LObject.tag == "runway" || lvlm.selectedObj.LObject.tag == "runwayNumber") //specifically runway
				{
					if (lvlm.selectedObj.LObject.name == "Runway_BlastPad" || lvlm.selectedObj.LObject.name == "Runway_DisplacedThreshold" || lvlm.selectedObj.LObject.name == "Runway_Line")
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
				else if (lvlm.selectedObj.LObject.name == "Apron_Taxi") //specifically ApronTaxi (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Apron or Taxiway block beside it.");
					}
					CheckAdjacentTrue();
				}
				else if (lvlm.selectedObj.LObject.name == "Grass_Taxi") //specifically GrassTaxi (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Grass or Taxiway block beside it.");
					}
					CheckAdjacentTrue();
				}
				else if (lvlm.selectedObj.LObject.name == "Grass_Runway")//specifically GrassRunway (hybrid block)
				{
					CheckAdjacent();
					if (adjacentCheck == false)
					{
						print("You can only place this block if there is an Grass or Runway block beside it.");
					}
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

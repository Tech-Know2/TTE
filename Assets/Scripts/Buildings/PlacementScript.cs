using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlacementScript : MonoBehaviour
{
    // Materials Indicating Placement Capabilities
    private Material originalMaterial;
    public Camera mainCamera;

    //Access Scripts
    public PlayerScript playerScript;

    // Access the Information About the Building
    public BuildingPopUp buildingPopUp;
    public Settlements settlementData;
    public Buildings buildingData;

    // Location and Building Vars
    private GameObject buildingInstance;
    public List<string> acceptableTileTags;
    private GameObject hoveredObject;
    private bool isPlacing = false;
    private Vector3 hoveringLocation;

    //Settlement that the Building is being placed under the control of
    private Settlements buildingsSettlement;

    private string buildingType;

    public void PlaceBuilding(string buildType, GameObject building, BuildingSlotDisplay buildingSlotDisplay)
    {
        if (buildType == "Settlement")
        {
            settlementData = buildingSlotDisplay.settlementData;
            buildingInstance = Instantiate(building);
            buildingType = buildType;
            isPlacing = true;
        }
        else
        {
            buildingData = buildingSlotDisplay.buildingData;
            buildingInstance = Instantiate(building);
            buildingType = buildType;
            isPlacing = true;
        }
    }

    private void Update()
    {
        if (isPlacing)
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any objects
            if (Physics.Raycast(ray, out hit))
            {
                if (acceptableTileTags.Contains(hit.transform.tag))
                {
                    // Store the new hovered object
                    hoveredObject = hit.transform.gameObject;

                    // Update the preview position to follow the mouse
                    Vector3 mousePosition = hit.point;
                    hoveringLocation = mousePosition;
                }
            }
            else
            {
                // No object is being hovered
                hoveredObject = null;
            }

            if(buildingType != null)
            {
                Preview(buildingType);
            }
        }
    }


    private void Preview(string buildingType)
    {
        if (isPlacing)
        {
            bool solidPlacement = false;

            if (buildingType == "Settlement")
            {
                if (settlementData != null)
                {
                    if (settlementData.acceptableBuildTiles.Contains(hoveredObject.tag) && !playerScript.playerSettlementDataList.Any(settlement => settlement.tilesUnderCityControl.Contains(hoveredObject)))
                    {
                        solidPlacement = true;
                        buildingInstance.transform.position = new Vector3(hoveredObject.transform.position.x, hoveredObject.transform.position.y + 5f, hoveredObject.transform.position.z);
                    }
                    else
                    {
                        solidPlacement = false;
                    }
                }
            }
            else
            {
                if (buildingData != null)
                {
                    if (buildingData.acceptableBuildTiles.Contains(hoveredObject.tag))
                    {
                        if (buildingData.requiresASettlement == true && playerScript.playerSettlementDataList.Any(settlement => settlement.tilesUnderCityControl.Contains(hoveredObject)))
                        {
                            solidPlacement = true;
                            buildingInstance.transform.position = new Vector3(hoveringLocation.x, hoveringLocation.y + 10f, hoveringLocation.z);

                            //Assign the settlement, and pass it to the building to be
                            buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(hoveredObject));
                        }
                        else if (buildingData.requiresASettlement == false && buildingData.acceptableBuildTiles.Contains(hoveredObject.tag))
                        {
                            solidPlacement = true;
                            buildingInstance.transform.position = new Vector3(hoveringLocation.x, hoveringLocation.y + 10f, hoveringLocation.z);
                        }
                        else
                        {
                            solidPlacement = false;

                            NotificationController notificationController = playerScript.notificationController;
                            notificationController.NewNotification("Building", "Can't Build There", "That tile is not under the control of a near by city", false);
                        }
                    }
                    else
                    {
                        solidPlacement = false;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (solidPlacement == true)
                {
                    Vector3 spawnLocation;

                    // Place the building
                    if(buildingType == "Settlement")
                    {
                        spawnLocation = new Vector3(hoveredObject.transform.position.x, hoveredObject.transform.position.y + 5f, hoveredObject.transform.position.z);
                    }else 
                    {
                        spawnLocation = new Vector3(hoveringLocation.x, hoveringLocation.y + 10f, hoveringLocation.z);
                    }
                    GameObject placedBuilding = Instantiate(buildingInstance, spawnLocation, Quaternion.identity);

                    // Assign all of the data to the newly created building
                    BuildingDataController buildingDataController = placedBuilding.GetComponent<BuildingDataController>();

                    if (buildingType == "Settlement")
                    {
                        buildingDataController.originalSettlementData = settlementData;
                        buildingDataController.SettlementSetup();
                    }
                    else
                    {
                        buildingDataController.originalBuildingData = buildingData;
                        buildingDataController.buildingsSettlement = buildingsSettlement;
                        buildingDataController.BuildingSetUp();

                        buildingPopUp.clickedBuildingPopUpCount += 1;
                    }

                    // Reset placement variables
                    isPlacing = false;

                    // Hide the Pop-up and show the Cards
                    if(buildingType == "Settlement")
                    {
                        buildingPopUp.closeBuildingDisplay();
                    } else
                    {
                        if(buildingPopUp.clickedBuildingPopUpCount == buildingPopUp.multiBuildingPopUpCount)
                        {
                            buildingPopUp.closeBuildingDisplay();
                        }
                    }

                    // Destroy the previewed buildingInstance after placement
                    Destroy(buildingInstance);
                }
            }
        }
    }
}
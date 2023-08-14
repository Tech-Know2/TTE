using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityController : MonoBehaviour
{
    // Accessing Data and Scripts
    public BuildingDataController buildingDataController;
    public BuildingEffectController buildingEffectController;
    private Settlements settlementData;

    // List of tiles under the city's control
    public List<GameObject> collidedTiles = new List<GameObject>();

    // Information About the City (Obtained and Stored from the Scriptable Object). Don't edit these values, edit the scriptable objects' values
    public string cityName;
    public int cityLevel;
    public int cityPopulation;
    public float calculatedLoyalty;
    public int cityFood;
    public List<Object> settlementReligiousFollowers = new List<Object>();
    public List<Buildings> settlementBuildings = new List<Buildings>();
    public List<GameObject> tilesUnderCityControl = new List<GameObject>();
    public List<Unit> settlementUnits = new List<Unit>();

    //Notification System
    private NotificationController notificationController;
    private PlayerScript playerScript;

    public void SettlementStart()
    {
        buildingDataController.controlSphere.SetActive(true);
        
        print(buildingDataController.controlSphere);
        settlementData = buildingDataController.clonedSettlementData;
        settlementData.cityPopulation = 10;
        settlementData.cityFood = 50;

        //Assign the Variables to be public so they can be seen from the editor for debugging
        cityName = settlementData.cityName;
        cityLevel = settlementData.cityLevel;
        cityPopulation = settlementData.cityPopulation;
        calculatedLoyalty = settlementData.calculatedLoyalty;

        CheckCollision();

        //Notification System
        Transform playerAndCameraRig = GameObject.Find("Player and Camera Rig")?.transform;

        if (playerAndCameraRig != null)
        {
            playerScript = playerAndCameraRig.GetComponent<PlayerScript>();
            notificationController = playerScript.notificationController;
        }
        else
        {
            Debug.LogError("Could not find game object named 'Player and Camera Rig'.");
            return;
        }
            
        notificationController.NewNotification("Settlement", "New Settlement Established", "Great work establishing a new settlement. The people of " + cityName + "are thrilled to be joining your empire", true);
    }

    public void UpdateCityDisplayValues() //Update all of the variables to the new and updated data
    {
        cityName = settlementData.cityName;
        cityLevel = settlementData.cityLevel;
        cityFood = settlementData.cityFood;
        cityPopulation = settlementData.cityPopulation;
        calculatedLoyalty = settlementData.calculatedLoyalty;
        tilesUnderCityControl = settlementData.tilesUnderCityControl;
        settlementUnits = settlementData.settlementUnits;
        settlementBuildings = settlementData.settlementBuildings;
        settlementReligiousFollowers = settlementData.settlementReligiousFollowers;
    }

    private void CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, buildingDataController.controlSphere.transform.localScale.x / 2f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Deep Sea") || collider.CompareTag("Shallow Sea") || collider.CompareTag("Sand") || collider.CompareTag("Plains") || collider.CompareTag("Mountains"))
            {
                GameObject tile = collider.gameObject;

                if (!collidedTiles.Contains(tile))
                {
                    print("Tile Under Control: " + tile.name);
                    settlementData.tilesUnderCityControl.Add(tile);
                    collidedTiles.Add(tile);
                }
            }
        }

        buildingDataController.controlSphere.SetActive(false);
    }


    public void CityUpgraded() //Most City Upgrade Impacts are between the city level of 1-4
    {
        if(cityLevel == 2)
        {
            //Update city
            settlementData.cityLevel = cityLevel;
        } 
        else if (cityLevel == 3)
        {
            //Update city
            settlementData.cityLevel = cityLevel;
        } 
        else if (cityLevel == 4)
        {
            settlementData.cityLevel = cityLevel;
        } 
        else 
        {
            //Do Something
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPopUp : MonoBehaviour
{
    //Connecting to Other Scripts
    private BuildingDataController buildingDataController;
    public CardEffectManager cardEffectManager;
    public PlacementScript placementScript;

    //Enabling and Disabling the Pop-ups
    public GameObject buildingPopUp;
    public bool isBuildingPopUpActive = false;
    public List<GameObject> cardSlots = new List<GameObject>();
    public bool isCardsShowing = true;

    public int multiBuildingPopUpCount = 0;
    public int clickedBuildingPopUpCount = 0;

    //Prefab for Dispays
    public GameObject buildingSlotPrefab;
    public List<GameObject> buildingSlots = new List<GameObject>(); //Max buildings per card is 4
    private List<GameObject> usedBuildingSlots = new List<GameObject>();

    //Data for the Buildings
    public List<Buildings> buildingsData = new List<Buildings>();
    private Buildings sendingBuildingData;
    public Settlements settlementData;

    //Currently Selected Card
    public Card clickedCard;

    public void SlotClickFunctionality(GameObject clickedObject)
    {
        print("Building Slot Clicked");

        BuildingSlotDisplay buildingSlotDisplay = clickedObject.GetComponent<BuildingSlotDisplay>();
        sendingBuildingData = buildingSlotDisplay.buildingData;

        if (buildingSlotDisplay != null)
        {
            if (buildingSlotDisplay.settlementData != null)
            {
                print(buildingSlotDisplay.settlementData);
                placementScript.PlaceBuilding("Settlement", buildingSlotDisplay.building, buildingSlotDisplay);
                Destroy(clickedObject);
            }
            else
            {
                print(buildingSlotDisplay.buildingData);
                placementScript.PlaceBuilding("Building", buildingSlotDisplay.building, buildingSlotDisplay);

                Destroy(clickedObject);

                if(clickedBuildingPopUpCount == multiBuildingPopUpCount)
                {
                    closeBuildingDisplay();
                }
            }

        }
    }


    public void BuildingDisplay(string passedBuildType)
    {
        clickedCard = cardEffectManager.card;

        isBuildingPopUpActive = true;
        isCardsShowing = false;

        buildingPopUp.SetActive(isBuildingPopUpActive);
        PropogateBuildingDisplays(passedBuildType);

        foreach (GameObject card in cardSlots)
        {
            card.SetActive(isCardsShowing);
        }
    }

    public void closeBuildingDisplay()
    {
        isBuildingPopUpActive = false;
        isCardsShowing = true;

        buildingPopUp.SetActive(isBuildingPopUpActive);

        foreach (GameObject card in cardSlots)
        {
            card.SetActive(isCardsShowing);
        }

        ClearBuildingDisplays();
    }


    public void PropogateBuildingDisplays(string passedBuildType)
    {
        print("Propogate Displays");

        if (passedBuildType == "Settlement" && buildingSlots.Count > 0)
        {
            GameObject newBuildingSlot = Instantiate(buildingSlotPrefab);
            newBuildingSlot.transform.SetParent(buildingSlots[0].transform, false);
            newBuildingSlot.transform.position = buildingSlots[0].transform.position;

            BuildingSlotDisplay buildingSlotDisplay = newBuildingSlot.GetComponent<BuildingSlotDisplay>();
            buildingSlotDisplay.settlementData = settlementData;
            buildingSlotDisplay.building = clickedCard.buildingGameObjects[0];
            buildingSlotDisplay.setDisplayVariables("Settlement");
        }
        else
        {
            if (buildingSlots.Count >= clickedCard.buildingGameObjects.Count)
            {
                for (int i = 0; i < buildingsData.Count; i++)
                {
                    GameObject newBuildingSlot = Instantiate(buildingSlotPrefab);
                    newBuildingSlot.transform.SetParent(buildingSlots[i].transform, false);
                    newBuildingSlot.transform.position = buildingSlots[i].transform.position;

                    print("Building a building");

                    BuildingSlotDisplay buildingSlotDisplay = newBuildingSlot.GetComponent<BuildingSlotDisplay>();
                    buildingSlotDisplay.buildingData = buildingsData[i];
                    buildingSlotDisplay.building = clickedCard.buildingGameObjects[i];
                    buildingSlotDisplay.setDisplayVariables("Building");
                }
            }
            else
            {
                // Handle the case when there are not enough building slots available
                Debug.LogError("Not enough building slots available for the buildings.");
            }

            multiBuildingPopUpCount = buildingsData.Count;
        }
    }

    public void SetSettlementData(Settlements data)
    {
        settlementData = data;
    }

    public void ClearBuildingDisplays()
    {
        //Reset All Vars back to the starting value
        buildingsData.Clear();
        clickedBuildingPopUpCount = 0;
    }
}

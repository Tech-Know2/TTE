using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardEffectManager : MonoBehaviour
{
    //Access and Retrieve the Scriptable Object Data From the Current Card
    public Card originalCard;
    public Card card;
    private Government government;
    public PlayerScript playerScript;
    private NotificationController notificationController;
    public EconomyManager economyManager;
    public BuildingPopUp buildingPopUp;

    //Building and Creation Card Variables
    public GameObject selectedTileLocation;

    //Manage Weather or not The Cards Are Being Currently in Use, or First Time Use
    public bool firstTime = true;
    private Card currentHighLevelCard;

    //Setup The Varibales to Be Accessed and Assigned By The Scriptable Objects
    private string effectCostType;
    private string cardEffectRegion;
    private int turnEffectLength;
    private int turnEffectCost;
    
    //Government and Political Based Data Sets
    private string governmentName;
    private bool assignsGovernment = false;

    //Current Active Cards
    public List<Card> activeCards = new List<Card>();

    //Filter What Type Of Effect The Card is Going to Have On The Board
    public void EffectFilter()
    {
        if (firstTime == true)
        {
            originalCard = playerScript.cardData;
            card = Instantiate(originalCard);
        } else if (firstTime == false)
        {
            card = currentHighLevelCard;
        }

        if(selectedTileLocation != null)
        {
            print("Tile Selected");
        }

        if(card.isGovernmentCard == true)
        {
            government = card.government;
            governmentName = government.governmentName;
            assignsGovernment = card.assignsGovernment;
                
            if(card.isGovernmentWarSupportBoostCard == true || card.isAllianceCard == true || card.isDeclareWarCard == true || card.isPeaceTreatyCard == true || card.assignsGovernment == true)
            {
                GovernmentEffectFilter();
            }
        }

        //Check to see if the card creates buildings or settlements
        if (card.createsBuildings == true || card.createsSettlement == true)
        {
            ConstructBuilding();
        }

        //Check to see if the card creates units
        if(card.createsUnits == true)
        {
            CreateUnits();
        }

        for(int i = 0; i < card.effectManagerList.Count; i++)
        {
            //Check to see if the card Requires multiple Turns
            if (card.effectManagerList[i].requiresMultipleTurns == true && firstTime == true)
            {
                activeCards.Add(card);
            }
            
            effectCostType = card.effectManagerList[i].effectCostType;
            turnEffectLength = card.effectManagerList[i].turnEffectLength;
            turnEffectCost = card.effectManagerList[i].turnEffectCost;
            cardEffectRegion = card.effectManagerList[i].religionName;

            if(effectCostType == "Gold Cost")
            {
                GoldCost(i);
            }
            else if (effectCostType == "Silver Cost")
            {
                SilverCost(i);
            }
            else if (effectCostType == "Loyalty Cost")
            {
                LoyaltyCost(i);
            }
            else if (effectCostType == "Food Cost")
            {
                FoodCost(i);
            }
            else if (effectCostType == "People Cost")
            {
                PeopleCost(i);
            }
            else if (effectCostType == "War Support Cost")
            {
                WarSupportCost(i);
            }
            else if (effectCostType == "Religion Cost")
            {
                ReligionCost(i);
            }
            else if (effectCostType == "Tech Cost")
            {
                TechCost(i);
            }
            else if (effectCostType == "Weariness Cost")
            {
                WearinessCost(i);
            } else
            {
                if(government != null)
                {
                    print(card + "Does Not Have An Effect, Probably and Error");
                }
            }
        }
    }

    //Government Related Effects
    public void GovernmentEffectFilter()
    {
        for (int i = 0; i < card.effectManagerList.Count; i++)
        {
            if (card.isGovernmentWarSupportBoostCard == true)
            {
                GovernmentWarSupportBoost(i);
            }
            else if (card.isAllianceCard == true == true)
            {
                AllianceCard(i);
            }
            else if (card.isPeaceTreatyCard == true)
            {
                PeaceTreatyCard(i);
            }
            else if (card.isDeclareWarCard == true)
            {
                DeclareWarCard(i);
            }
            else if (card.assignsGovernment == true)
            {
                AssignGovernment(i);
            }
        }
    }    

    //Different Types of Economic and Social Effects
    public void GoldCost(int i)
    {
        print("Gold Cost");
        economyManager.currentGold = economyManager.currentGold + card.effectManagerList[i].turnEffectCost;
    }

    public void SilverCost(int i)
    {
        print("Silver Cost");
        economyManager.currentSilver = economyManager.currentSilver + card.effectManagerList[i].turnEffectCost;
    }

    public void LoyaltyCost(int i)
    {
        print("Loyalty Cost");
        
        //Effect the loyalty of the city that the card has been applied to
        Settlements cardsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

        if (cardsSettlement != null)
        {
            cardsSettlement.calculatedLoyalty += card.effectManagerList[i].turnEffectCost;
        }
    }

    public void FoodCost(int i)
    {
        print("Food Cost");
        if(card.requiresASettlement == true)
        {
            //keep Count of the Tiles Being checked
            int acceptableTileCount = 0;

            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));
            
            if(card.effectManagerList[i].requiresTiles == true)
            {
                for (int x = 0; x < buildingsSettlement.tilesUnderCityControl.Count; x++)
                {
                    GameObject tile =  buildingsSettlement.tilesUnderCityControl[x];
                    if(card.desierdTileString.Contains(tile.tag))
                    {
                        acceptableTileCount += 1;
                    }
                }

                print("previous "+ buildingsSettlement.cityFood);
                print("Tiles " + acceptableTileCount);
                buildingsSettlement.cityFood += (card.effectManagerList[i].turnEffectCost * acceptableTileCount);
                print("current " + buildingsSettlement.cityFood);
                
            } else 
            {
                print("previous "+ buildingsSettlement.cityFood);
                buildingsSettlement.cityFood += card.effectManagerList[i].turnEffectCost;
                print("current " + buildingsSettlement.cityFood);
            }
            
            
        }
    }

    public void PeopleCost(int i)
    {
        print("People Cost");
        Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));
        buildingsSettlement.cityFood += card.effectManagerList[i].turnEffectCost;
    }

    public void WarSupportCost(int i)
    {
        print("War Support Cost");
        //Update the War Support value of the government the card effects
        //Modular so it does not matter if the government is not listed, it will be added and then have the effect applied in a modular and scalable way

        //Bool to keep track of the governments and if they exist or not
        bool governmentExists = true;


        for (int x = 0; x < economyManager.governmentEffectManagerList.Count; x++)
        {
            if (economyManager.governmentEffectManagerList[x].governmentName == card.effectManagerList[i].governmentName)
            {
                economyManager.governmentEffectManagerList[x].currentGovernmentWarSupport += card.effectManagerList[i].turnEffectCost;
                governmentExists = true;
            }

            if (governmentExists && x == economyManager.governmentEffectManagerList.Count)
            {
                governmentExists = false;
            }
        }

        if (governmentExists == false)
        {
            print("Government Does Not Exist, creating it now");

            //Create the government and add it to the list. Making the system modular, and easy to change and update. Shouldn't need just redundant
            GovernmentEffectManager newGovernment = new GovernmentEffectManager
            {
                governmentName = card.effectManagerList[i].governmentName, // Set the name as desired
                currentGovernmentWarSupport = 50 + card.effectManagerList[i].turnEffectCost // Set the initial war support value as desired
            };
        }
    }

    public void ReligionCost(int i)
    {
        print("Religion Cost");
        //Add religious producable objects to the list in the settlement to be stored and mofified for later.
        //Religious Units are created and added based on the empire's/player's color that added them 

        Settlements religiousSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));
        Object clonedReligiousIndividual = Instantiate(card.effectManagerList[i].relgiousIndividual);

        //Add the Religious Individual(s) to the settlement list
        religiousSettlement.settlementReligiousFollowers.Add(clonedReligiousIndividual);
        religiousSettlement.cityPopulation += 1;
    }

    public void WearinessCost(int i)
    {
        print("Weariness Cost");

        Settlements wearySettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

        wearySettlement.calculatedWeariness += card.effectManagerList[i].turnEffectCost;
    }

    public void TechCost(int i)
    {
        print("Tech Point Cost");

        economyManager.currentTechPoints += card.effectManagerList[i].turnEffectCost;
    }

    //Different Types of Political Effects    
    public void GovernmentWarSupportBoost(int i)
    {
        print("Government War Support Boost");
    }

    public void AllianceCard(int i)
    {
        print("Alliance Card");
    }

    public void DeclareWarCard(int i)
    {
        print("Declare War Card");
    }

    public void PeaceTreatyCard(int i)
    {
        print("Peace Treaty Card");
    }

    public void AssignGovernment(int i)
    {
        notificationController.NewNotification("Government Assigned", "Government Selected", "Congrats on choosing the " + government.name, true);
        playerScript.government = government;
    }

    //Placement and Creation Based Card Effects
    public void ConstructBuilding()
    {
        Settlements settlementData;
        Buildings buildingData;

        if(card.createsSettlement == true)
        {
            //Settlement Placement logic
            if (card.settlementScriptableObject != null)
            {
                settlementData = card.settlementScriptableObject;

                buildingPopUp.SetSettlementData(settlementData);
                buildingPopUp.BuildingDisplay("Settlement");
                buildingPopUp.settlementData = settlementData;

                print("Also Called");
            }
        }

        if(card.createsBuildings == true)
        {
            // Building Placement logic
            if (card.buildingGameObjects != null)
            {
                for (int i = 0; i < card.buildingGameObjects.Count; i++)
                {
                    GameObject building = card.buildingGameObjects[i];
                    BuildingDataController buildingDataController = building.GetComponent<BuildingDataController>();
                    buildingData = buildingDataController.originalBuildingData;
                    buildingPopUp.buildingsData.Add(buildingData);
                }
                buildingPopUp.BuildingDisplay("Building");
            }
        }
    }

    public void CreateUnits()
    {
        //Create Units
        //Get the informationabout the city that is in control of the tile
        //Determine if the city has a building capable of producing units
        //If so the card can be placed and used

        //Logic for Global Troop Creation
        //Determine what cities have the ability to produce these troops, then record them, and then produce them there
        // Logic for Global Troop Creation
        if (card.createUnitGlobally == true)
        {
            List<Settlements> acceptableCitySpawns = new List<Settlements>();
            bool hasRequirement = false;

            for (int i = 0; i < playerScript.playerSettlementDataList.Count; i++)
            {
                if (card.requiresBarrack == true)
                {
                    hasRequirement = playerScript.playerSettlementDataList[i].settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                    if (hasRequirement)
                    {
                        acceptableCitySpawns.Add(playerScript.playerSettlementDataList[i]);
                    }
                }
                else if (card.requiresWeaponSmith == true)
                {
                    // Add logic for Weapon Smith requirement
                }
                else if (card.requiresDiplomatTrainingHall == true)
                {
                    // Add logic for Diplomat Training Hall requirement
                }
                else if (card.requiresMachineWorkshop == true)
                {
                    // Add logic for Machine Workshop requirement
                }
                else if (card.requiresTrainingHall == true)
                {
                    // Add logic for Training Hall requirement
                }
            }

            foreach (Settlements citySpawn in acceptableCitySpawns)
            {
                // Create the unit at each city, and at the building that the unit can be made at
            }
        }

        //Logic for Single Troop Creation
        if (card.requiresBarrack == true)
        {
            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

            if (buildingsSettlement != null)
            {
                bool hasBarrack = buildingsSettlement.settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                if (hasBarrack)
                {
                    // The settlement has a building with the name "Barrack," so the card can be placed and used
                    // Implement the logic for using the card here
                }
                else
                {
                    print("Can't train units here. The settlement does not have a Barrack.");
                }
            }
        }
        else if (card.requiresWeaponSmith == true)
        {
            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

            if (buildingsSettlement != null)
            {
                bool hasWeaponSmith = buildingsSettlement.settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                if (hasWeaponSmith)
                {
                    // The settlement has a building with the name "Barrack," so the card can be placed and used
                    // Implement the logic for using the card here
                }
                else
                {
                    print("Can't train units here. The settlement does not have a Weapon Smith.");
                }
            }
        }
        else if (card.requiresDiplomatTrainingHall == true)
        {
            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

            if (buildingsSettlement != null)
            {
                bool hasDiplomatHall = buildingsSettlement.settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                if (hasDiplomatHall)
                {
                    // The settlement has a building with the name "Barrack," so the card can be placed and used
                    // Implement the logic for using the card here
                }
                else
                {
                    print("Can't train units here. The settlement does not have a Diplomat Training Hall.");
                }
            }
        }
        else if (card.requiresMachineWorkshop == true)
        {
            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

            if (buildingsSettlement != null)
            {
                bool hasMachineWorkshop = buildingsSettlement.settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                if (hasMachineWorkshop)
                {
                    // The settlement has a building with the name "Barrack," so the card can be placed and used
                    // Implement the logic for using the card here
                }
                else
                {
                    print("Can't train units here. The settlement does not have a Machine Workshop.");
                }
            }
        }
        else if (card.requiresTrainingHall == true)
        {
            Settlements buildingsSettlement = playerScript.playerSettlementDataList.Find(settlement => settlement.tilesUnderCityControl.Contains(selectedTileLocation));

            if (buildingsSettlement != null)
            {
                bool hasTrainingHall = buildingsSettlement.settlementBuildings.Any(buildingData => buildingData.buildingName.Contains("Barrack"));

                if (hasTrainingHall)
                {
                    // The settlement has a building with the name "Barrack," so the card can be placed and used
                    // Implement the logic for using the card here
                }
                else
                {
                    print("Can't train units here. The settlement does not have a Training Hall.");
                }
            }
        }
        else {
            print("Can't train this unit here");
        }
    }

    //Manage the Currently Active Cards and their Effects
    public void CurrentlyActiveCards()
    {
        List<Card> cardsToRemove = new List<Card>();

        foreach (Card currentCard in activeCards.ToArray()) // Iterate over a copy of activeCards
        {
            // Apply the card effect
            card = currentCard;

            // Store the current card as the high-level card
            currentHighLevelCard = card;
            firstTime = false;
            EffectFilter();

            //Go through the effects and check if they have been running for the appropiate amount of time
            for (int i = 0; i < card.effectManagerList.Count; i++)
            {
                currentCard.effectManagerList[i].turnsActive++;

                if (currentCard.effectManagerList[i].turnsActive >= currentCard.effectManagerList[i].turnEffectLength)
                {
                    currentCard.effectManagerList.RemoveAt(i);
                    i--; // Adjust the index after removing an element
                }

                if (currentCard.effectManagerList.Count <= 0)
                {
                    cardsToRemove.Add(currentCard);
                    break; // No need to continue iterating if all effects have been removed
                }
            }
        }

        // Remove the cards outside of the loop
        foreach (Card cardToRemove in cardsToRemove)
        {
            activeCards.Remove(cardToRemove);
        }
    }
}
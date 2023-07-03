using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechDisplay : MonoBehaviour
{
    public Tech originalTech;
    private Tech tech;
    public Dealer dealer;
    public PlayerScript playerScript;
    public TechTreeController techTreeController;
    private ClonedTechData clonedTechData;
    private int updatedtechPointCount;

    private List<string> researchedTechNames = new List<string>();

    public Button currentTechButton;
    public Button previousTechButton;
    public Tech originalPreviousTech;
    private Tech previousTech;
    public EconomyManager economyManager;

    void Start()
    {
        //Fine the Cloned Tech Script
        Transform playerAndCameraRig = GameObject.Find("Player and Camera Rig")?.transform;
        clonedTechData = playerAndCameraRig.GetComponentInChildren<ClonedTechData>();

        originalTech.ResetTechState();

        currentTechButton = GetComponent<Button>();

        tech = Instantiate(originalTech);

        if (originalPreviousTech != null)
        {
            previousTech = Instantiate(originalPreviousTech);
        }

        Image buttonImage = currentTechButton.image;
        string techColor = tech.techColor;

        print("tech Color Code" + techColor);

        Color newColor;

        if (ColorUtility.TryParseHtmlString(techColor, out newColor))
        {
            buttonImage.color = newColor;
        }
        else
        {
            Debug.LogError("Invalid color code: " + techColor);
        }
    }

    public void CheckSettlmentStatus()
    {
        if (tech.techName == "Settlements")
        {
            techTreeController.isSettlementTechResearched = true;
        }
    }

    public void techResearch()
    {
        // Get the Current Amount of Tech Points from Player
        updatedtechPointCount = economyManager.currentTechPoints;

        if (previousTechButton != null)
        {
            TechDisplay techDisplay = previousTechButton.GetComponent<TechDisplay>();
            if (techDisplay != null)
            {
                previousTech = techDisplay.tech;

                print(tech.techName + " has already been researched");
            }
        }

        if (playerScript.currentEra >= tech.techEra && updatedtechPointCount >= 1 && !researchedTechNames.Contains(tech.techName))
        {
            updatedtechPointCount = updatedtechPointCount - 1;

            // Set the Economy Manager tech Points to the New Updated Values
            economyManager.currentTechPoints = updatedtechPointCount;

            clonedTechData.clonedResearchedTechs.Add(tech);

            if (tech.techCards.Count != 0)
            {
                foreach (Card card in tech.techCards)
                {
                    dealer.actionCardArray.Add(card);
                }
                dealer.filterCards();
            }

            // Add the name of the researched tech to the list
            researchedTechNames.Add(tech.techName);

            print(tech.techName + " Researched");
        }
        else if (previousTech != null && researchedTechNames.Contains(previousTech.techName) && playerScript.currentEra >= tech.techEra && updatedtechPointCount >= 1)
        {
            updatedtechPointCount = updatedtechPointCount - 1;

            // Set the Economy Manager tech Points to the New Updated Values
            economyManager.currentTechPoints = updatedtechPointCount;

            if (tech.techCards.Count != 0)
            {
                foreach (Card card in tech.techCards)
                {
                    dealer.actionCardArray.Add(card);
                }
                dealer.filterCards();
            }

            // Add the name of the researched tech to the list
            researchedTechNames.Add(tech.techName);

            print(tech.techName + " Researched");
        }

        CheckSettlmentStatus();
    }
}

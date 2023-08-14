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
    private float updatedtechPointCount;

    private List<string> researchedTechNames = new List<string>();

    public Button currentTechButton;
    public Button previousTechButton;
    private Tech originalPreviousTech;
    private Tech previousTech;
    public EconomyManager economyManager;

    //Notification System
    private NotificationController notificationController;

    void Start()
    {
        // Find the Cloned Tech Script
        Transform playerAndCameraRig = GameObject.Find("Player and Camera Rig")?.transform;
        clonedTechData = playerAndCameraRig.GetComponentInChildren<ClonedTechData>();

        //Connect to the Notification Tech System
        playerScript = playerAndCameraRig.GetComponent<PlayerScript>();
        notificationController = playerScript.notificationController;

        currentTechButton = GetComponent<Button>();
        tech = Instantiate(originalTech);

        Image buttonImage = currentTechButton.image;
        string techColor = tech.techColor;

        Color newColor;

        if (ColorUtility.TryParseHtmlString(techColor, out newColor))
        {
            buttonImage.color = newColor;
        }
        else
        {
            //Debug.LogError("Invalid color code: " + techColor);
        }

        if (previousTechButton != null)
        {
            TechDisplay techDisplay = previousTechButton.GetComponent<TechDisplay>();
            previousTech = techDisplay.originalTech;
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

        for (int i = 0; i < clonedTechData.clonedResearchedTechs.Count; i++)
        {
            researchedTechNames.Add(clonedTechData.clonedResearchedTechs[i].techName);
        }

        bool canResearch = false;

        if (previousTechButton != null)
        {
            TechDisplay techDisplay = previousTechButton.GetComponent<TechDisplay>();
            previousTech = techDisplay.originalTech;

            print(previousTech.techName + " previous tech name");
        }

        if (previousTech == null || researchedTechNames.Contains(previousTech.techName))
        {
            canResearch = true;
        }

        if (canResearch && playerScript.currentEra >= tech.techEra && updatedtechPointCount >= 1 && !researchedTechNames.Contains(tech.techName))
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

            notificationController.NewNotification("Tech Research", "New Tech Researched", "You have researched " + tech.techName, true);
        }
        else
        {
            notificationController.NewNotification("Tech Research", "Unable to research this Tech", "You're unable to research " + tech.techName, false);
        }

        CheckSettlmentStatus();
    }
}

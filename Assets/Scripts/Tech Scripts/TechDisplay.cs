using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechDisplay : MonoBehaviour
{
    public Tech tech;
    public Dealer dealer;
    public PlayerScript playerScript;

    public Button currentTechButton;
    public Button previousTechButton;
    private Tech previousTech;
    public EconomyManager economyManager;

    void Start()
    {
        tech.isResearched = false;

        currentTechButton = GetComponent<Button>();

        Image buttonImage = currentTechButton.image;
        string techColor = tech.techColor;

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

    public void techResearch()
    {
        int playerGoldCount = economyManager.currentGold;
        int goldCost = tech.goldCost;

        if (previousTechButton != null)
        {
            TechDisplay techDisplay = previousTechButton.GetComponent<TechDisplay>();
            if (techDisplay != null)
            {
                previousTech = techDisplay.tech;

                print(tech.techName + " has already been researched");
            }
        }

        if (previousTech == null && playerScript.currentEra >= tech.techEra && playerGoldCount >= tech.goldCost)
        {
            tech.isResearched = true;
            economyManager.currentGold -= goldCost;

            if(tech.techCards.Count != 0)
            {
                dealer.actionCardArray.AddRange(tech.techCards);
                dealer.filterCards();
            }

            print(tech.techName + " Researched");
        }
        else if (previousTech != null && tech.isResearched == false && previousTech.isResearched == true && playerScript.currentEra >= tech.techEra  && playerGoldCount >= tech.goldCost)
        {
            tech.isResearched = true;
            economyManager.currentGold -= goldCost;
            
            if(tech.techCards.Count != 0)
            {
                dealer.actionCardArray.AddRange(tech.techCards);
                dealer.filterCards();
            }

            print(tech.techName + " Researched");
        }
    }
}

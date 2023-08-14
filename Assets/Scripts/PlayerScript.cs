using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Players Visible Information
    public string playerName;
    public string playerColor;
    public List<string> avaliableColors = new List<string>();

    //Vars for Game Management
    public int currentEra;
    public int currentTurn;

    //Era Turn Deciding Variables
    public int eraOneLength;
    public int eraTwoLength;

    // Associate the script with our code so it can be called
    public CardSelector cardSelector;
    public Dealer dealer;
    public PlacementScript placementScript;
    public EconomyManager economyManager;
    public Government government;
    public NameGenerator nameGenerator;
    public BuildingEffectController buildingEffectController;
    
    //Player's Empire Data and Information
    public List<Settlements> playerSettlementDataList = new List<Settlements>();
    public List<GameObject> playerSettlementObjectList = new List<GameObject>();
    public List<Buildings> playerBuildingsDataList = new List<Buildings>();
    public List<GameObject> playerBuildingObjectList = new List<GameObject>();

    // Selected Card Game Object
    private GameObject selectedCard;
    public GameObject selectedTileLocation;
    public Card cardData;

    //Amount of Cards that Can Be Played Per Turn
    public int maxCardsPerTurn = 3;
    public int cardsPlayed = 0;

    //Controlling the Interactions with the Cards In Game
    public CardEffectManager cardEffectManager;
    public NotificationController notificationController;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the other script to this variable
        dealer = GetComponent<Dealer>();
        placementScript = GetComponent<PlacementScript>();

        currentEra = 1;
        currentTurn = 0;

        dealer.filterCards();

        //Determine Player Name and Player Empire Color
        int randColorInt = Random.Range(0, avaliableColors.Count);
        playerColor = avaliableColors[randColorInt];

        //Generate Player Name
        nameGenerator.PlayerNameGeneration();
    }

    public void Name()
    {
        nameGenerator.GenerateName();
    }

    public void TurnEffects()
    {
        //buildingEffectController.EffectController();
        for (int i = 0; i < playerSettlementDataList.Count; i++)
        {
            Settlements settlement = playerSettlementDataList[i];
            buildingEffectController.SettlementEffectManager(playerSettlementDataList[i]);
            for (int x = 0; x < settlement.settlementBuildings.Count; x++)
            {
                Buildings building = settlement.settlementBuildings[x];
                buildingEffectController.BuildingEffectManager(building, settlement);
            }
        }

        for(int i = 0; i < playerBuildingsDataList.Count; i++)
        {
            Buildings building = playerBuildingsDataList[i];

            buildingEffectController.BuildingEffectManager(building, null);
        }

        for (int i = 0; i < playerSettlementObjectList.Count; i++)
        {
            GameObject settlementObject = playerSettlementObjectList[i];
            CityController cityController = settlementObject.GetComponent<CityController>();

            cityController.UpdateCityDisplayValues();
        }
    }

    void checkEra()
    {
        if(currentTurn < eraOneLength)
        {
            currentEra = 1;
        } else if (currentTurn < eraTwoLength)
        {
            currentEra = 2;
        } else 
        {
            currentEra = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkEra();

        economyManager.passedTurnCount = currentTurn;
        economyManager.passedEraCount = currentEra;        
    }

    public void CardSelected()
    {
        if (cardsPlayed < maxCardsPerTurn)
        {
            cardsPlayed = cardsPlayed + 1;
            PlayCard();
        } else 
        {
            print("You Have Reached Max Cards Played Per Turn");
        }
    }

    public void PlayCard()
    {
        cardEffectManager.originalCard = cardData;
        cardEffectManager.selectedTileLocation = selectedTileLocation;
        cardEffectManager.EffectFilter();
    }
}
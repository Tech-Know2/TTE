using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeController : MonoBehaviour
{
    public BuildingPopUp buildingPopUp;

    public Button techTreeButton;
    public GameObject techTree;
    public RawImage nonagon;
    public CameraController cameraController;

    //Notification System Connections
    private NotificationController notificationController;
    private PlayerScript playerScript;

    public bool isSettlementTechResearched;
    private bool scrollEnabled;

    public List<GameObject> techBranches = new List<GameObject>();
    public GameObject techBranchHolder;
    private int currentBranchNumber = 0;

    private float rotationValue;
    private float currentRotation = 0f;
    
    public bool treeEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        //Allow Movement at the start of the game
        cameraController.cameraPanningAllowed = true;
        cameraController.cameraScrollAllowed = true;
        cameraController.cameraMovementAllowed = true;

        techTree.SetActive(false);
        treeEnabled = false;

        isSettlementTechResearched = false;

        rotationValue = 360f / techBranches.Count;

        techBranchHolder.SetActive(true);

        for (int x = 0; x < techBranches.Count; x++)
        {
            if (x == 0)
            {
                techBranches[x].SetActive(true);
            }
            else
            {
                techBranches[x].SetActive(false);
            }
        }
    }

    public void Movement()
    {
        if(treeEnabled == true)
        {
            cameraController.cameraPanningAllowed = false;
            cameraController.cameraScrollAllowed = false;
            cameraController.cameraMovementAllowed = false;
        } else 
        {
            cameraController.cameraPanningAllowed = true;
            cameraController.cameraScrollAllowed = true;
            cameraController.cameraMovementAllowed = true;
        }
    }

    public void checkRequiredResearch()
    {
        if(isSettlementTechResearched == true)
        {
            scrollEnabled = true;
        } else 
        {
            scrollEnabled = false;

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

            
            notificationController.NewNotification("Tech Tree", "Unable to Scroll", "If you wish to scroll through the tech branches, research the required tech (Settlements).", false);
        }
    }

    public void ActivateTree()
    {
        if (treeEnabled == false)
        {
            treeEnabled = true;
            Movement();
            techTree.SetActive(true);

            foreach (GameObject card in buildingPopUp.cardSlots)
            {
                card.SetActive(false);
            }

            currentBranchNumber = 0;

            // Check if settlementTech is not null before accessing its properties
            if (isSettlementTechResearched == true)
            {
                scrollEnabled = true;
            }
            else
            {
                scrollEnabled = false;
            }

            currentRotation = currentBranchNumber * rotationValue; // Calculate the initial rotation based on the current branch number
            nonagon.rectTransform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            displayCurrentBranch();
        }
        else
        {
            treeEnabled = false;
            techTree.SetActive(false);
            Movement();

            foreach (GameObject card in buildingPopUp.cardSlots)
            {
                card.SetActive(true);
            }
        }
    }



    public void rightSpin()
    {
        checkRequiredResearch();
        
        if (scrollEnabled == true)
        {
            if (currentBranchNumber >= techBranches.Count - 1)
            {
                currentBranchNumber = 0;
            }
            else
            {
                currentBranchNumber++;
            }

            currentRotation -= rotationValue;
            nonagon.rectTransform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            displayCurrentBranch();
        }
    }

    public void leftSpin()
    {
        checkRequiredResearch();

        if (scrollEnabled == true)
        {
            if (currentBranchNumber > 0)
            {
                currentBranchNumber--;
            }
            else
            {
                currentBranchNumber = techBranches.Count - 1;
            }

            currentRotation += rotationValue;
            nonagon.rectTransform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            displayCurrentBranch();
        }
    }

    public int displayCurrentBranch()
    {
        print(currentBranchNumber);
        for (int i = 0; i < techBranches.Count; i++)
        {
            if (i == currentBranchNumber)
            {
                techBranches[i].SetActive(true);
            }
            else
            {
                techBranches[i].SetActive(false);
            }
        }
        return currentBranchNumber;
    }
}

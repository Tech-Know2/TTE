using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Notifications
{
    public string notificationType; //Notification type examples include (building, card, tech, government, etc). Pretty much a summary of the notification type
    public string notificationHeader; //The name of the notification
    public string notificationMessage; //The message of the notification
    public bool isPositive; //Is it positive or not
    public int turn; //Turn the notification occured on
}

public class NotificationController : MonoBehaviour
{
    public List<Notifications> notificationList = new List<Notifications>();
    public PlayerScript playerScript;

    public void DisplayNotifications()
    {
        
    }

    public void NewNotification(string type, string header, string message, bool positive)
    {
        Notifications newNotification = new Notifications
        {
            notificationType = type,
            notificationHeader = header,
            notificationMessage = message,
            isPositive = positive,
            turn = playerScript.currentTurn
        };

        notificationList.Add(newNotification);
    }
}

//Standard Connection to this system
/*

private NotificationController notificationController;
private PlayerScript playerScript;

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
    
notificationController.NewNotification("h", "e", "l", false);

*/

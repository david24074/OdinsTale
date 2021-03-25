using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageLog : MonoBehaviour
{
    [SerializeField] private int maxMessages = 30;
    [SerializeField] private GameObject messageObject;
    [SerializeField] private Transform messageContent;
    private static MessageLog messageLogger;
    private List<GameObject> currentMessageObjects = new List<GameObject>();

    private void Awake()
    {
        messageLogger = this;
    }

    public void LoadMessages(List<string> newMessages)
    {
        for(int i = 0; i < newMessages.Count; i++)
        {
            AddNewMessage(newMessages[i]);
        }
    }

    public  List<string> GetAllMessages()
    {
        List<string> allMessages = new List<string>();

        for(int i = 0; i < currentMessageObjects.Count; i++)
        {
            allMessages.Add(currentMessageObjects[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        }

        return allMessages;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddNewMessage("New test message number: " + Random.Range(0, 300));
        }
    }

    public static void AddNewMessage(string newText)
    {
        GameObject newObject = Instantiate(messageLogger.messageObject, messageLogger.messageContent);
        newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newText;
        messageLogger.currentMessageObjects.Add(newObject);

        if(messageLogger.currentMessageObjects.Count >= messageLogger.maxMessages)
        {
            GameObject objectToRemove = messageLogger.currentMessageObjects[0];
            Destroy(objectToRemove);
            messageLogger.currentMessageObjects.Remove(objectToRemove);
        }
    }
}

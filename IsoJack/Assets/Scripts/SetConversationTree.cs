﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


///*
public class SetConversationTree : MonoBehaviour
{
    
    public GameObject ConversationView;
    private int textPosition = 0;
    public List<int> dialogueType;
     //0 = Normal
     //1 = 4 - way Choice
     //2 = Exit on next click
     
    public List<Sprite> NPCSprite;
    public List<Sprite> PlayerSprite;
    public List<string> NPCName;
    public List<string> dialogueText;
    public List<string> ChoiceWarps; //Only for Dialogue Options Warp1;Warp2;Warp3;Warp4

    private int tp; //Text Position
    private GameObject DialogueTextObject;
    private GameObject NPCPortrait;
    private GameObject PlayerPortrait;
    private GameObject NPCNameObject;
    public GameObject ChoicesCanvas;
    private GameObject Dia1;
    private GameObject Dia2;
    private GameObject Dia3;
    private GameObject Dia4;
    private GameObject AdvLog;

    private GameObject hp;
    private GameObject inventory;

    private bool dialogueActive;
    // Start is called before the first frame update
    void Start()
    {
        //bool ConvIsInactive = ConversationView.activeSelf;
        //if (ConvIsInactive) ConversationView.SetActive(true);
        DialogueTextObject = GameObject.Find("ConversationView/DialogueText");
        NPCPortrait = GameObject.Find("ConversationView/npcPortrait");
        PlayerPortrait = GameObject.Find("ConversationView/playerPortrait");
        NPCNameObject = GameObject.Find("ConversationView/NPCNameTag");
        //ChoicesCanvas = GameObject.Find("ConversationView/choicesCanvas");
        Dia1 = GameObject.Find("ConversationView/choicesCanvas/DialogueOptionOne");
        Dia2 = GameObject.Find("ConversationView/choicesCanvas/DialogueOptionTwo");
        Dia3 = GameObject.Find("ConversationView/choicesCanvas/DialogueOptionThree");
        Dia4 = GameObject.Find("ConversationView/choicesCanvas/DialogueOptionFour");
        AdvLog = GameObject.Find("AdventureLogPanel/AdventureLogBox");
        //if (ConvIsInactive) ConversationView.SetActive(false);
        hp = GameObject.Find("HUDCanvas/HPIndicator");
        inventory = GameObject.Find("HUDCanvas/Inventory");
    }

    void OnEnable()
    {
        
    }

    public void StartConversation()
    {
        Debug.Log("Started Conversation");
        GameManager.instance.inConversation = true;
        hp.SetActive(false);
        inventory.SetActive(false);
        ConversationView.SetActive(true);
        if (Adventureog.advLogInstance.isOpen == true)
            Adventureog.advLogInstance.closeLog();
        tp = textPosition;
        loadDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialogueActive)
        {
            if(ConversationView.activeInHierarchy == false)
                StartConversation();

            if (dialogueType[tp] == 0)
            {
                tp += 1;
                loadDialogue();
            }
            if ((dialogueType[tp] == 2 || tp + 1 >= dialogueText.Count))
            {
                Debug.Log("Ended Conversation");
                ConversationView.SetActive(false);
                GameManager.instance.inConversation = false;
                hp.SetActive(true);
                inventory.SetActive(true);
                ChoicesCanvas.SetActive(true);
                Dia1.SetActive(true);
                Dia2.SetActive(true);
                Dia3.SetActive(true);
                Dia4.SetActive(true);


                if (NPCSprite[tp + 1] != null)
                {
                    Debug.Log("Augmenting tp!");
                    textPosition = tp+1;
                }
                else
                    Debug.Log("Did not augment tp!");
                //CharTalk.charInstance.exitDialougeView();
            }
        }
        if (dialogueType[tp] == 1)
        {
            int choice = DialougeView.converstationInstance.getChoicePressed();
            if (choice != 0)
            {
                tp = int.Parse(GetSection(ChoiceWarps[tp], choice - 1));
                loadDialogue();
            }
        }

    }

    void loadDialogue()
    {
        
        Debug.Log("Conversation Section: " + tp.ToString());
        addQuestHandler(tp, NPCName[tp]);
        if (dialogueType[tp] == 0 || dialogueType[tp] == 2)
        { 
            ChoicesCanvas.SetActive(false);
            DialogueTextObject.GetComponent<UnityEngine.UI.Text>().text = dialogueText[tp];
        }
        else if (dialogueType[tp] == 1)
        {
            ChoicesCanvas.SetActive(true);
            DialogueTextObject.GetComponent<UnityEngine.UI.Text>().text = GetSection(dialogueText[tp], 0);
            DialougeView.converstationInstance.showDialougeChoices();
            ChoicesCanvas.SetActive(true);
            int c = CountSections(dialogueText[tp]);
            Dia1.SetActive(false);
            Dia2.SetActive(false);
            Dia3.SetActive(false);
            Dia4.SetActive(false);

            if (c > 1)
            {
                Dia1.SetActive(true);
                Dia1.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetSection(dialogueText[tp], 1);
            }
            if (c > 2)
            {
                Dia2.SetActive(true);
                Dia2.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetSection(dialogueText[tp], 2);
            }
            if (c > 3)
            {
                Dia3.SetActive(true);
                Dia3.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetSection(dialogueText[tp], 3);
            }
            if (c > 4)
            {
                Dia4.SetActive(true);
                Dia4.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetSection(dialogueText[tp], 4);
            }
        }
        //Update Name
        if (NPCName[tp] != "") NPCNameObject.GetComponent<UnityEngine.UI.Text>().text = NPCName[tp];
        //Update NPC Sprite
        if (NPCSprite[tp] != null) NPCPortrait.GetComponent<UnityEngine.UI.Image>().sprite = NPCSprite[tp];
        //Update Player Sprite
        if (PlayerSprite[tp] != null) PlayerPortrait.GetComponent<UnityEngine.UI.Image>().sprite = PlayerSprite[tp];
    }

    string GetSection(string text, int section)
    {
        //Section starts at 0. Section is defined as a string of text seperated by ';'

        if (CountSections(text) <= section || section < 0) return "";

        return text.Split(';')[section];
    }

    int CountSections(string text)
    {
        if (text == "") return 0;
        int count = 1;
        foreach (char c in text)
        {
            if (c == ';') count++;
        }
        return count;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Can talk!!");
            dialogueActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueActive = false;
        }
    }

    public void addQuestHandler(int num, string name)
    {
        if(num == 1 && name == "Tavern Queen")
        {
            if (Adventureog.advLogInstance.questOne.qButton.activeInHierarchy == true)
            {
                textPosition = 4;
                tp = 4;
            }
        }
        if(num == 4 && name == "Tavern Queen")
        {
            if (Adventureog.advLogInstance.questOne.qButton.activeInHierarchy == false)
            {
                textPosition = 0;
                tp = 1;
            }
        }

        if(num == 7 && name == "Chief")
        {
            Adventureog.advLogInstance.questOne.qButton.SetActive(true);
            Adventureog.advLogInstance.questOne.questInfo = "•There is a war looming! Go talk to the Tavern Queen to start preparing.";
        }

        if(num == 8 && name == "Tavern Queen")
        {
            Adventureog.advLogInstance.questOne.questInfo += "\n•Go to the forest and slay 5 bears.";
        }
    }
}
//*/
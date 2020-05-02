using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public Text UI_msg_displayer;
    Animator msgAnimator;
    public Image UI_big_circle_egg;
    public TextMeshProUGUI UI_text_name_input;
    public Transform UI_main_menu;
    public GameObject tabInfo;

    private void Awake()
    {
        if (Instance) Destroy(this);
        Init();
    }

    private void Init()
    {
        Instance = this;
        msgAnimator = UI_msg_displayer.transform.GetComponent<Animator>();
        ShowMessage("Its ok");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab)) tabInfo.SetActive(false);
        if (Input.GetKeyDown(KeyCode.Tab)) tabInfo.SetActive(true);
    }

    public void DisplayPlayerOnTab(string playerName, int playerNumber)
    {
        TextMeshProUGUI textDisplayer;
        switch (playerNumber)
        {
            case 1: textDisplayer = tabInfo.transform.Find("Player1").GetComponent<TextMeshProUGUI>(); break;
            case 2: textDisplayer = tabInfo.transform.Find("Player2").GetComponent<TextMeshProUGUI>(); break;
            case 3: textDisplayer = tabInfo.transform.Find("Player3").GetComponent<TextMeshProUGUI>(); break;
            default: textDisplayer = tabInfo.transform.Find("Player4").GetComponent<TextMeshProUGUI>(); break;
        }
        textDisplayer.text = playerName;
    }

    public void CreateRoom()
    {
        NetworkController.Instance.HostGame("test");
    }

    public void JoinToRoom()
    {
        NetworkController.Instance.ConnectToRoom("test");
    }


    public void ShowMessage(string msg)
    {
        UI_msg_displayer.text = msg;
        msgAnimator.Play("Fade Out");
    }

    public void OnTextNameChanged()
    {
        string value = GetTextNameValue();
        if (value.Length > 3 && value.Length < 10) ActivateEgg();
        else DesactivateEgg();
    }

    public string GetTextNameValue()
    {
        return UI_text_name_input.text;
    }

    void ActivateEgg()
    {
        UI_big_circle_egg.color = Color.white;
    }

    void DesactivateEgg()
    {
        UI_big_circle_egg.color = new Color(0, 0, 0, 0);
    }

    public void BTN_Egg()
    {
        string textName = GetTextNameValue();
        print(textName.Length);
        if (textName.Length == 7)
        {
            CreateRoom();
            Debug.Log("pollando");
        }
        else JoinToRoom();
    }

    public void DesactivateMainMenu()
    {
        UI_main_menu.gameObject.SetActive(false);
    }

}

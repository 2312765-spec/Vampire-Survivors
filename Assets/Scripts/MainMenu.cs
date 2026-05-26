using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName;

    [Tooltip("Optional title text to display 'Class Survival'.")]
    public TMP_Text gameTitleText;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mapSelectPanel;
    [SerializeField] private GameObject mapSelectPanel_BackButton;
    [SerializeField] private GameObject mapSelectPanel_CustomButton;
    [SerializeField] private GameObject mapSelectPanel_StartButton;
    [SerializeField] private GameObject mapSelectPanel_PreviewImage;
    [SerializeField] private GameObject mapSelectPanel_Decription;
    [SerializeField] private GameObject mapSelectPanel_PlainButton;
    [SerializeField] private GameObject mapSelectPanel_RoadButton;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject mapSelectButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject creditButton;
    [SerializeField] private Sprite plainImage;
    [SerializeField] private Sprite roadImage;

    private int selectedMap;
    private bool isSelected = false;

    void Start()
    {
        //if (gameTitleText != null)
        //    gameTitleText.text = "Class Survival";
    }

    public void StartGame(int index)
    {
        if(index < 0)
        {
            Debug.Log("Mising index");
            return;
        }
        switch (index)
        {
            case 1:
                SceneManager.LoadScene("Main");
                break;
            case 2:
                SceneManager.LoadScene("Map2");
                break;
            case 3:
                //Custom Map load
                break;
        }
    }

    public void MapSelectButton_OnClick()
    {
        mainMenuPanel.SetActive(false);
        mapSelectPanel.SetActive(true);

        this.isSelected = false;
        this.mapSelectPanel_StartButton.GetComponent<Button>().interactable = false;
        this.mapSelectPanel_PreviewImage.GetComponent<Image>().sprite = null;
        this.mapSelectPanel_Decription.GetComponent<TMP_Text>().text = "Description: ";
    }
    public void MapSelectPanel_BackButton_OnClick()
    {
        this.isSelected = false;
        this.mapSelectPanel_StartButton.GetComponent<Button>().interactable = false;
        this.mapSelectPanel_PreviewImage.GetComponent<Image>().sprite = null;
        this.mapSelectPanel_Decription.GetComponent<TMP_Text>().text = "Decription: ";

        mainMenuPanel.SetActive(true);
        mapSelectPanel.SetActive(false);

    }
    public void MapSelectPanel_StartButton_OnClick()
    {
        if(isSelected)
            StartGame(selectedMap);
    }
    public void MapSelectPanel_CustomButton_OnClick()
    {
        //WIP
    }
    public void MapSelectPanel_PlainButton_OnClick()
    {
        this.isSelected = true;
        this.selectedMap = 1;
        this.mapSelectPanel_StartButton.GetComponent<Button>().interactable = true;
        this.mapSelectPanel_PreviewImage.GetComponent<Image>().sprite = this.plainImage;
        this.mapSelectPanel_Decription.GetComponent<TMP_Text>().text = "Decription:\nPlain is a simply map with nothing";
    }
    public void MapSelectPanel_RoadButton_OnClick()
    {
        this.isSelected = true;
        this.selectedMap = 2;
        this.mapSelectPanel_StartButton.GetComponent<Button>().interactable = true;
        this.mapSelectPanel_PreviewImage.GetComponent<Image>().sprite = this.roadImage;
        this.mapSelectPanel_Decription.GetComponent<TMP_Text>().text = "Decription:\nA Plain map with a road in middle";
    }

    public void QuitButton_OnClick()
    {
        Application.Quit();
        Debug.Log("Quitting Class Survival");
    }

    public void OpenCredit()
    {
        Debug.Log("Open Credit");

        if (ChangeBGM.Instance != null)
            ChangeBGM.Instance.PlayCredit();

        if (creditPanel != null) creditPanel.SetActive(true);
        SetButtonActive(false);
    }

    public void CloseCredit()
    {
        Debug.Log("Close Credit");

        if (ChangeBGM.Instance != null)
            ChangeBGM.Instance.PlayMainMenu();

        if (creditPanel != null) creditPanel.SetActive(false);
        SetButtonActive(true);
    }

    private void SetButtonActive(bool active)
    {
        if (mapSelectButton != null) mapSelectButton.GetComponent<Button>().interactable = active;
        if (quitButton != null) quitButton.GetComponent<Button>().interactable = active;
        if (creditButton != null) creditButton.GetComponent<Button>().interactable = active;
    }
}
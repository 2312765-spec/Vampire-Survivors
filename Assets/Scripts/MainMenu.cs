using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject creditButton;
    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("I'm Quitting");
    }

    public void OpenCredit()
    {
        Debug.Log("Open Credit");
        ChangeBGM.Instance.PlayCredit();
        creditPanel.SetActive(true);
        this.SetButtonActive(false);
    }
    public void CloseCredit()
    {
        Debug.Log("Close Credit");
        ChangeBGM.Instance.PlayMainMenu();
        creditPanel.SetActive(false);
        this.SetButtonActive(true);

    }
    private void SetButtonActive(bool status)
    {
        startButton.GetComponent<Button>().interactable = status;
        quitButton.GetComponent<Button>().interactable = status;
        creditButton.GetComponent<Button>().interactable = status;
    }
}
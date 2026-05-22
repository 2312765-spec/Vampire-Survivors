using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName;
    [SerializeField] private GameObject creditPanel;
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
    }
    public void CloseCredit()
    {
        Debug.Log("Close Credit");
        ChangeBGM.Instance.PlayMainMenu();
        creditPanel.SetActive(false);

    }
}
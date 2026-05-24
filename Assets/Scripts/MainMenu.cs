using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName; // The first level name.

    [Tooltip("Optional title text to display 'Class Survival'.")]
    public TMP_Text gameTitleText;

    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject creditButton;

    void Start()
    {
        if (gameTitleText != null)
            gameTitleText.text = "Class Survival";
    }

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Class Survival");
    }

    public void OpenCredit()
    {
        Debug.Log("Open Credit");
        CallChangeBGM("PlayCredit");
        if (creditPanel != null) creditPanel.SetActive(true);
        SetButtonActive(false);
    }

    public void CloseCredit()
    {
        Debug.Log("Close Credit");
        CallChangeBGM("PlayMainMenu");
        if (creditPanel != null) creditPanel.SetActive(false);
        SetButtonActive(true);
    }

    private void CallChangeBGM(string methodName)
    {
        // Try to locate the ChangeBGM type across loaded assemblies
        System.Type bgmType = System.Type.GetType("ChangeBGM");
        if (bgmType == null)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                bgmType = asm.GetType("ChangeBGM");
                if (bgmType != null) break;
            }
        }

        if (bgmType != null)
        {
            var instanceProp = bgmType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (instanceProp != null)
            {
                var instance = instanceProp.GetValue(null);
                if (instance != null)
                {
                    var method = bgmType.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (method != null)
                    {
                        try { method.Invoke(instance, null); return; }
                        catch { /* ignore invocation errors */ }
                    }
                }
            }
        }

        // Fallback: runtime SendMessage search by component name (no compile-time dependency)
        foreach (var mb in FindObjectsOfType<MonoBehaviour>())
        {
            if (mb == null) continue;
            if (mb.GetType().Name == "ChangeBGM")
            {
                mb.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                return;
            }
        }
    }

    private void SetButtonActive(bool status)
    {
        if (startButton != null) startButton.GetComponent<Button>().interactable = status;
        if (quitButton != null) quitButton.GetComponent<Button>().interactable = status;
        if (creditButton != null) creditButton.GetComponent<Button>().interactable = status;
    }
}
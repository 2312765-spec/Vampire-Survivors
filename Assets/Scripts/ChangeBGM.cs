using UnityEngine;
using System.Collections;

public class ChangeBGM : MonoBehaviour
{
    public static ChangeBGM Instance;

    public AudioSource bgmSource;

    public AudioClip mainMenuBGM;
    public AudioClip creditBGM;

    [Range(0f, 1f)]
    public float maxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMainMenu(false);
    }

    public void PlayMainMenu(bool flag = true)
    {
        StartCoroutine(SwitchBGM(mainMenuBGM, flag));
    }

    public void PlayCredit()
    {
        StartCoroutine(SwitchBGM(creditBGM, true));
    }

    IEnumerator SwitchBGM(AudioClip newClip, bool enable)
    {
        if(enable){
            while (bgmSource.volume > 0)
            {
                bgmSource.volume -= Time.deltaTime;
                yield return null;
            }

            bgmSource.clip = newClip;
            bgmSource.Play();

            // while (bgmSource.volume < maxVolume)
            // {
            //     bgmSource.volume += Time.deltaTime;
            //     yield return null;
            // }

            bgmSource.volume = maxVolume;
        }
    }
}
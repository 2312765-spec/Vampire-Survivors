using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class SFXGroup
    {
        public string groupName;
        public AudioSource[] sounds;
    }

    public SFXGroup[] soundGroups;

    public void PlaySFX(int groupIndex)
    {
        AudioSource[] group = soundGroups[groupIndex].sounds;

        int randomIndex = Random.Range(0, group.Length);

        group[randomIndex].Stop();
        group[randomIndex].Play();
    }

    public void PlaySFXPitched(int groupIndex)
    {
        AudioSource[] group = soundGroups[groupIndex].sounds;

        int randomIndex = Random.Range(0, group.Length);

        group[randomIndex].pitch = Random.Range(0.8f, 1.2f);

        group[randomIndex].Stop();
        group[randomIndex].Play();
    }
}
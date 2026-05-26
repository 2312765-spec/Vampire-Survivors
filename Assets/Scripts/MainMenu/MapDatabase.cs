using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Class Survival/Map Database")]
public class MapDatabase : ScriptableObject
{
    public List<MapData> maps = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var map in maps)
        {
            map.UpdateScenePath();
        }
    }
#endif
}
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MapData
{
    [Header("Basic Info")]
    public string mapId;

    public string displayName;

    [TextArea]
    public string description;

    public Sprite previewImage;

#if UNITY_EDITOR
    [Header("Scene Reference")]
    [SerializeField]
    private SceneAsset sceneAsset;
#endif

    [SerializeField]
    private string scenePath;

    public string ScenePath => scenePath;

#if UNITY_EDITOR
    public void UpdateScenePath()
    {
        if (sceneAsset != null)
        {
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
        }
    }
#endif
}
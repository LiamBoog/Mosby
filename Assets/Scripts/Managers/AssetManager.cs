using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    #region Constants

    public static string PREFAB_FOLDER_NAME = "Prefabs/";
    public static string MANAGER_FOLDER_NAME = "Managers/";

    public static string PLAYER_PREFAB_NAME = "Player";
    public static string BOUNCER_PREFAB_NAME = "Bouncer";
    public static string BACKGROUND_PREFAB_NAME = "Background";

    public static string ASSET_MANAGER_NAME = "AssetManager";
    public static string GAMEPLAY_MANAGER_NAME = "GameplayManager";
    public static string BOUNCER_MANAGER_NAME = "BouncerManager";
    public static string PLAYER_MANAGER_NAME = "PlayerManager";

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads a gameobject from a prefab and returns it
    /// </summary>
    /// <param name="assetName">Name of the asset being loaded</param>
    /// <returns></returns>
    public static GameObject LoadGameObject(string assetName)
    {
        return Resources.Load<GameObject>(PREFAB_FOLDER_NAME + assetName);
    }

    /// <summary>
    /// Loads a component from a prefab from within resources
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <param name="assetName">Name of the asset being loaded</param>
    /// <returns>Object of specified type</returns>
    public static T LoadPrefabComponent<T>(string assetName)
    {
        GameObject resourceObject = Resources.Load<GameObject>(PREFAB_FOLDER_NAME + assetName);
        return resourceObject.GetComponent<T>();
    }

    /// <summary>
    /// Loads a manager prefab
    /// </summary>
    /// <param name="assetName">Name of the manager prefab</param>
    public static T LoadManager<T>(string assetName)
    {
        GameObject resourceObject = Resources.Load<GameObject>(MANAGER_FOLDER_NAME + assetName);
        return resourceObject.GetComponent<T>();
    }

    #endregion
}

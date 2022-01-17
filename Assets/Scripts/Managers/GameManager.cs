using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Accessors

    public AssetManager AssetManager { get; private set; }
    public GameplayManager GameplayManager { get; private set; }
    public BouncerManager BouncerManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }

    #endregion

    #region Public Methods



    #endregion

    #region Private Methods

    /// <summary>
    /// Asserts that none of the managers are null
    /// </summary>
    private void AssertManagers()
    {
        Debug.Assert(AssetManager != null, "assetManager was null", this);
        Debug.Assert(GameplayManager != null, "gameplayManager was null", this);
        Debug.Assert(BouncerManager != null, "bouncerManager was null", this);
        Debug.Assert(PlayerManager != null, "playerManager was null", this);
    }

    /// <summary>
    /// Instantiates all managers
    /// </summary>
    private void InstantiateManagers()
    {
        AssetManager = AssetManager.LoadManager<AssetManager>(AssetManager.ASSET_MANAGER_NAME);
        Instantiate(AssetManager, transform);

        GameplayManager = AssetManager.LoadManager<GameplayManager>(AssetManager.GAMEPLAY_MANAGER_NAME);
        Instantiate(GameplayManager, transform);

        BouncerManager = AssetManager.LoadManager<BouncerManager>(AssetManager.BOUNCER_MANAGER_NAME);
        Instantiate(BouncerManager, transform);

        PlayerManager = AssetManager.LoadManager<PlayerManager>(AssetManager.PLAYER_MANAGER_NAME);
        Instantiate(PlayerManager, transform);
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the GameManager
    /// </summary>
    public void Start()
    {
        InstantiateManagers();
        AssertManagers();
    }

    #endregion
}

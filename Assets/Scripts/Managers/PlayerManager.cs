using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Accessors

    public Player Player { get; private set; }

    #endregion

    #region Private Members

    private GameObject playerPrefab = null;

    #endregion


    #region Unity Methods

    /// <summary>
    /// First thing Unity calls on the game object
    /// </summary>
    public void Awake()
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates an instance of the player prefab
    /// </summary>
    public void InstantiatePlayer()
    {
        if (playerPrefab == null)
        {
            playerPrefab = AssetManager.LoadGameObject(AssetManager.PLAYER_PREFAB_NAME);
        }

        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.35f, 0f));
        spawnPosition.z = 0f;

        Player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity).GetComponent<Player>();
    }

    /// <summary>
    /// If player is not null will destroy him and set him to null
    /// </summary>
    public void DestroyPlayer()
    {
        if (Player != null)
        {
            Destroy(Player.gameObject);
            Player = null;
        }
    }

    /// <summary>
    /// Pauses Player
    /// </summary>
    public void PausePlayer()
    {
        Player.Pause(true);
    }

    /// <summary>
    /// Unpauses Player
    /// </summary>
    public void UnpausePlayer()
    {
        Player.Pause(false);
    }


    #endregion

    #region Private Methods



    #endregion
}

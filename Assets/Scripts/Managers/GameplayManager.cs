using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Member Variables

    private int currentScore = 0;
    private bool gamePaused = false;
    private GameplayScreen gameplayScreen = null;

    #endregion

    #region Accessors

    public bool GameIsActive { get; private set; } = false;

    #endregion

    #region Public Methods

    //i dont like this but lets leave it for now

    /// <summary>
    /// Called by the gameplay screen to set itself in the manager
    /// </summary>
    /// <param name="gameplayScreen">The gameplay screen script</param>
    public void SetGameplayScreen(GameplayScreen gameplayScreen)
    {
        this.gameplayScreen = gameplayScreen;
    }

    /// <summary>
    /// Initializes a new game
    /// </summary>
    public void NewGame()
    {
        if (!GameIsActive && !gamePaused)
        {
            Time.timeScale = 1f;
            GameIsActive = true;
            currentScore = 0;
            gameplayScreen.UpdateScoreText(currentScore);

            GameManager.Instance.PlayerManager.InstantiatePlayer();
            GameManager.Instance.BouncerManager.InstantiateBouncers();

            Camera.main.GetComponent<CameraMovement>().SmoothCenterOnPlayer(delegate
            {
                GameManager.Instance.BouncerManager.PrepareNextBouncer();
                GameManager.Instance.BouncerManager.LaunchBouncer();
            });
        }
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    public void PauseGame()
    {
        if (!gamePaused)
        {
            gamePaused = true;
            Time.timeScale = 0f;
            GameManager.Instance.PlayerManager.PausePlayer();
        }
    }

    /// <summary>
    /// Unpauses the game
    /// </summary>
    public void UnpauseGame()
    {
        if (gamePaused)
        {
            gamePaused = false;
            Time.timeScale = 1f;
            GameManager.Instance.PlayerManager.UnpausePlayer();
        }
    }

    /// <summary>
    /// This function is called whenever the game is lost
    /// </summary>
    public void EndGame()
    {
        if (GameIsActive)
        {
            GameIsActive = false;
            gamePaused = false;
            GameManager.Instance.PlayerManager.DestroyPlayer();
            GameManager.Instance.BouncerManager.RemoveBouncers();
        }
    }

    public void RestartGame()
    {
        if (GameIsActive)
        {
            EndGame();
            NewGame();
        }
    }

    /// <summary>
    /// This function is called whenever a collision is detected between the player and a bouncer
    /// </summary>
    public void PlayerAndBouncerCollision()
    {
        if (GameIsActive)
        {
            ++currentScore;
            gameplayScreen.UpdateScoreText(currentScore);

            GameManager.Instance.BouncerManager.PrepareNextBouncer();
            GameManager.Instance.PlayerManager.Player.StopMovement();
            Camera.main.GetComponent<CameraMovement>().SmoothCenterOnPlayer(delegate
            {
                GameManager.Instance.BouncerManager.LaunchBouncer();
            });
        }
    }

    #endregion
}
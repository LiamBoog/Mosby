using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayScreen : MonoBehaviour
{
    #region Inspector Controlled Variables

    [SerializeField] private Text currentScoreText = null;
    [SerializeField] private GameObject pauseButton = null;
    [SerializeField] private GameObject pauseMenu = null;

    #endregion


    #region Private Methods

    private void AssertInspectorVariables()
    {
        Debug.Assert(currentScoreText != null, "currentScoreText is null", this);
        Debug.Assert(pauseButton != null, "pauseButton is null", this);
        Debug.Assert(pauseMenu != null, "pauseMenu is null", this);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Called to update the current score text
    /// </summary>
    /// <param name="score">The current score</param>
    public void UpdateScoreText(int score)
    {
        currentScoreText.text = score.ToString();
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    /// <summary>
    /// Closes the game and returns to the main menu
    /// </summary>
    public void OnUIOpenMainMenu()
    {
        GameManager.Instance.GameplayManager.EndGame();
        SceneManager.LoadScene(Constants.SCENE_LANDING);
    }

    /// <summary>
    /// Opens the pause menu
    /// </summary>
    public void OnUIOpenPauseMenu()
    {
        GameManager.Instance.GameplayManager.PauseGame();
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void OnUIClosePauseMenu()
    {
        GameManager.Instance.GameplayManager.UnpauseGame();
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }


    /// <summary>
    /// Restarts the game
    /// </summary>
    public void OnUIRestartGame()
    {
        GameManager.Instance.GameplayManager.RestartGame();
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }

    #endregion

    #region Unity Methods

    public void Awake()
    {
        AssertInspectorVariables();

        //i dont like this
        GameManager.Instance.GameplayManager.SetGameplayScreen(this);
    }

    #endregion
}

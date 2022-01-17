using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandingScreen : MonoBehaviour
{

    #region Inspector Controlled Variables

    [SerializeField] private Text highScore;

    #endregion

    #region UI Methods

    /// <summary>
    /// Callback for when the main play button is pressed
    /// </summary>
    public void OnUIPressPlayButton()
    {
        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameManager.Instance.GameplayManager.NewGame();

            Background background = Camera.main.GetComponent<Background>();
            background.ClearBackground();
            background.GenerateNewBackground();
        }
        SceneManager.LoadScene(Constants.SCENE_MAIN_GAMEPLAY);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion

    #region Unity Methods

    private void Update()
    {
        Camera.main.GetComponent<CameraMovement>().Move(0.005f * Vector2.up);
    }

    private void Awake()
    {
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is used to initialize everything required for game to function properly
/// </summary>
public class Initializer : MonoBehaviour
{
    public void Awake()
    {
        //Create GameManager instance
        GameManager gameManager = GameManager.Instance;
        SceneManager.LoadScene(Constants.SCENE_LANDING);
    }
}

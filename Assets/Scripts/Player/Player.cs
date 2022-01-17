using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Inspector Controlled Variables

    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private AudioSource popSound = null;

    #endregion

    #region Private Methods

    /// <summary>
    /// Alerts the GameplayManager of a valid collision
    /// </summary>
    private void ReportCollision(Collider2D otherCollider)
    {
        if (GameManager.Instance.BouncerManager.GameObjectIsActiveBouncer(otherCollider.gameObject))
        {
            popSound.Play();
            GameManager.Instance.GameplayManager.PlayerAndBouncerCollision();
        }
    }

    /// <summary>
    /// If the player leaves the camera the game is lost
    /// </summary>
    private void CheckIfPlayerOutOfFrame()
    {
        Vector3 playerPos = Camera.main.WorldToViewportPoint(transform.position);
        float viewportPlayerRadiusX = (Camera.main.WorldToScreenPoint(transform.position + new Vector3(GetComponent<CircleCollider2D>().radius, 0f, 0f)) - Camera.main.WorldToScreenPoint(transform.position)).x / Screen.width;
        float viewportPlayerRadiusY = (Camera.main.WorldToScreenPoint(transform.position + new Vector3(GetComponent<CircleCollider2D>().radius, 0f, 0f)) - Camera.main.WorldToScreenPoint(transform.position)).x / Screen.height;

        if (playerPos.x > 1 + viewportPlayerRadiusX || playerPos.y > 1 + viewportPlayerRadiusY || playerPos.x < 0 - viewportPlayerRadiusX || playerPos.y < 0 - viewportPlayerRadiusY) //if it is in the viewport the coords will be between 0 and 1
        {
            GameManager.Instance.GameplayManager.RestartGame();
        }
    }

    private void AssertInspectorVariables()
    {
        Debug.Assert(playerMovement != null, "playerMovement was null", this);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Stops the player movement
    /// </summary>
    public void StopMovement()
    {
        playerMovement.StopMovement();
    }

    /// <summary>
    /// Pauses player based on given bool
    /// </summary>
    /// <param name="isPaused">Whether or not to pause this Player</param>
    public void Pause(bool isPaused)
    {
        playerMovement.PauseControls(isPaused);
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// First thing that's called
    /// </summary>
    public void Awake()
    {
        AssertInspectorVariables();
    }

    /// <summary>
    /// Called when a collision is detected between this gameobject and another
    /// </summary>
    public void OnTriggerEnter2D(Collider2D otherCollider)
    {
        ReportCollision(otherCollider);
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called every frame
    /// </summary>
    public void Update()
    {
        CheckIfPlayerOutOfFrame();
    }

    #endregion
}

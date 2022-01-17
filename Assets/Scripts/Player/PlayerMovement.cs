using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Inspector Controlled Variables

    [SerializeField] private float movementSpeed = 1f;

    #endregion

    #region Private Members

    private Vector3 movementDirection;
    private bool isPaused = false;
    private bool touchedInsidePlayer = false;

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the movementDirection back to 0
    /// </summary>
    public void StopMovement()
    {
        movementDirection = Vector3.zero;
    }

    /// <summary>
    /// Pauses player controls depending on given true or false value
    /// </summary>
    /// <param name="isPaused"></param>
    public void PauseControls(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks if player is touching screen, moves player when finger is released
    /// </summary>
    private void UpdatePosition()
    {
        if (!isPaused)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 aimVector = transform.position - (Vector3)touchPosition;

                if (touch.phase == TouchPhase.Began && GetComponent<CircleCollider2D>().OverlapPoint(touchPosition))
                {
                    touchedInsidePlayer = true;
                }

                if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touchedInsidePlayer)
                {
                    movementDirection = aimVector;
                    touchedInsidePlayer = false;
                }
            }
        }

        //apply velocity
        transform.position += movementDirection * movementSpeed * Time.deltaTime;
    }

    #endregion

    #region Unity Methods

    public void Update()
    {
        UpdatePosition();
    }

    #endregion
}

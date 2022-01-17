using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour

{
    #region Member Variables

    private bool isActive = false;
    private Vector2 centerOfRotation;
    private float speed;
    private bool enteredFrame = false;
    private bool changedDirection = false;

    #endregion

    #region Public Methods

    /// <summary>
    /// Activates the current instance of Bouncer
    /// </summary>
    /// <param name="spawnData">Contains the spawning data for the bouncer</param>
    public void Activate(BouncerManager.BouncerSpawnData spawnData)
    {
        transform.position = spawnData.SpawnPoint;
        gameObject.SetActive(true);

        StartCoroutine(CallAfterDelay(spawnData.Delay, delegate
        {
            this.isActive = true;
            this.enteredFrame = false;
            this.centerOfRotation = spawnData.CenterOfRotation;
            this.speed = spawnData.Speed;
            this.changedDirection = false;
        }));
    }

    /// <summary>
    /// Deactivates the current instance of Bouncer
    /// </summary>
    public void Deactivate()
    {
        this.isActive = false;

        gameObject.SetActive(false);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Coroutine for invoking callback after a delay
    /// </summary>
    /// <param name="delay">Seconds to wait</param>
    /// <param name="callback">Callback for when the delay is over</param>
    /// <returns></returns>
    private IEnumerator CallAfterDelay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);

        callback.Invoke();
    }

    /// <summary>
    /// Updates the position of this instance of Bouncer
    /// </summary>
    private void UpdatePosition()
    {
        if (isActive)
        {
            Vector2 radiusVector = centerOfRotation - (Vector2)transform.position;
            Vector2 deltaR = -speed * speed * Time.deltaTime / radiusVector.magnitude * radiusVector;
            Vector2 deltaPhi = new Vector2(-deltaR.y, deltaR.x).normalized * speed;
            transform.position += (Vector3)(deltaR + deltaPhi);

            if (!BouncerIsInFrame() && !changedDirection)
            {
                speed = -speed;
                changedDirection = true;
            }
        }
    }

    /// <summary>
    /// Returns a Vector2 where each dimension is the bouncer's radius in Viewport space in that dimension
    /// </summary>
    /// <returns></returns>
    private Vector2 GetViewportBouncerRadius()
    {
        float x = (Camera.main.WorldToScreenPoint(transform.position + new Vector3(GetComponent<CircleCollider2D>().radius, 0f, 0f)) - Camera.main.WorldToScreenPoint(transform.position)).x / Screen.width;
        float y = (Camera.main.WorldToScreenPoint(transform.position + new Vector3(GetComponent<CircleCollider2D>().radius, 0f, 0f)) - Camera.main.WorldToScreenPoint(transform.position)).x / Screen.height;

        return new Vector2(x, y);
    }

    /// <summary>
    /// Checks if the bouncer is fully in camera view. If it entered and then managed to leave the game is lost
    /// </summary>
    private void CheckIfBouncerInFrame()
    {
        Vector3 bouncerPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 viewportBouncerRadius = GetViewportBouncerRadius();

        if ((bouncerPos.x > 1 + viewportBouncerRadius.x || bouncerPos.y > 1 + viewportBouncerRadius.y || bouncerPos.x < 0 - viewportBouncerRadius.x || bouncerPos.y < 0 - viewportBouncerRadius.y) && enteredFrame) //if it is in the viewport the coords will be between 0 and 1
        {
            GameManager.Instance.GameplayManager.RestartGame();
        } 
        else if (bouncerPos.x > 0 + viewportBouncerRadius.x && bouncerPos.y > 0 + viewportBouncerRadius.y && bouncerPos.x < 1 - viewportBouncerRadius.x && bouncerPos.y < 1 - viewportBouncerRadius.y && !enteredFrame)
        {
            enteredFrame = true;
        }
    }


    /// <summary>
    /// Returns true if any part of the bouncer is in frame
    /// </summary>
    /// <returns></returns>
    private bool BouncerIsInFrame()
    {
        Vector3 bouncerPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 viewportBouncerRadius = GetViewportBouncerRadius();

        if (bouncerPos.x > 0 - viewportBouncerRadius.x && bouncerPos.y > 0 - viewportBouncerRadius.y && bouncerPos.x < 1 + viewportBouncerRadius.x && bouncerPos.y < 1 + viewportBouncerRadius.y)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called every fixed update
    /// </summary>
    public void FixedUpdate()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    public void Update()
    {
        if (isActive)
        {
            CheckIfBouncerInFrame();
        }
    }

    #endregion
}

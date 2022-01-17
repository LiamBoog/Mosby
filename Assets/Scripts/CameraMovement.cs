using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Inspector Controlled Variables

    [SerializeField] private float movementSpeed = 1.5f;

    #endregion

    #region Private Members

    private Vector3 offset;
    private Vector3 targetPosition;
    private Vector3 movementDirection;
    private bool isMoving = false;

    #endregion

    #region Public Methods

    /// <summary>
    /// Calls the coroutine to center the camera on the player
    /// </summary>
    /// <param name="callback">Callback when camera is centered</param>
    public void SmoothCenterOnPlayer(Action callback)
    {
        StartCoroutine(SmoothCenterOnPlayerCoroutine(callback));
    }

    /// <summary>
    /// Moves camera by delta
    /// </summary>
    /// <param name="delta">(x, y) vector to move camera by</param>
    public void Move(Vector2 delta)
    {
        transform.position += (Vector3)delta;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Coroutine used to center the camera
    /// </summary>
    /// <param name="callback">Callback when camera is centered</param>
    /// <returns></returns>
    private IEnumerator SmoothCenterOnPlayerCoroutine(Action callback)
    {
        //only do this if we were not previously moving
        if (!isMoving)
        {
            isMoving = true;
            targetPosition = GameManager.Instance.PlayerManager.Player.transform.position + offset;

            //when we are close enough just stop
            float distanceToTarget = (transform.position - targetPosition).magnitude;
            while (distanceToTarget > 0.1f)
            {
                movementDirection = (targetPosition - transform.position).normalized * 7f - (targetPosition - transform.position);
                transform.position += movementSpeed * movementDirection * Time.deltaTime;
                distanceToTarget = (transform.position - targetPosition).magnitude;

                yield return null;
            }

            //close the tiny gap with a tp
            transform.position = targetPosition;

            isMoving = false;
        }

        callback?.Invoke();
        yield break;
    }

    #endregion

    #region Unity Methods

    void Awake()
    {
        //scales with viewport size
        offset = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.35f, 0)) + new Vector3(0f, 0f, -100f);
    }

    #endregion
}

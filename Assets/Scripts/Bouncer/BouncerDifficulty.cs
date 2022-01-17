using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerDifficulty
{
    #region Private Members

    private float speed;
    private float rotationRadius;
    private float trajectoryWindowScale;
    private float delay;

    #endregion

    #region Accessors

    public float Speed { get { return speed; } }

    public float RotationRadius { get { return rotationRadius; } }

    public float TrajectoryWindowScale { get { return trajectoryWindowScale;  } }

    public float Delay { get { return delay; } }

    #endregion

    #region Public Methods

    ///<summary>
    ///Instatiates new BouncerDifficulty object with default difficulty parameters
    ///</summary> 
    public BouncerDifficulty()
    {
        speed = 0.05f;
        rotationRadius = 18f;
        trajectoryWindowScale = 0.5f;
        delay = 1.25f;
    }

    /// <summary>
    /// Scale all difficulty parameters by preset amount
    /// </summary>
    public void ScaleDifficulty()
    {
        speed += (0.15f - 0.05f) / 50f;

        float bouncerRadius = AssetManager.LoadPrefabComponent<CircleCollider2D>(AssetManager.BOUNCER_PREFAB_NAME).radius;
        rotationRadius -= (18f - 0.5f * ((Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero)).magnitude + 2f * bouncerRadius)) / 20f;

        trajectoryWindowScale += (1f - 0.5f) / 50f;

        delay -= (1.25f - 0.25f) / 50f;
    }

    #endregion

}

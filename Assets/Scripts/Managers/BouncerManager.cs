using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerManager : MonoBehaviour
{
    /// <summary>
    /// Used to store data for bouncer spawn together nice and tidy
    /// </summary>
    public struct BouncerSpawnData
    {
        public Vector2 SpawnPoint;
        public Vector2 CenterOfRotation;
        public float Speed;
        public float Delay;

        public BouncerSpawnData(Vector2 spawnPoint, float speed, Vector2 centerOfRotation, float delay)
        {
            this.SpawnPoint = spawnPoint;
            this.Speed = speed;
            this.CenterOfRotation = centerOfRotation;
            this.Delay = delay;
        }
    }

    #region Private Members

    private static int numberOfBouncers = 3;
    private int activeBouncerIndex = 0;
    private List<Bouncer> bouncerList = new List<Bouncer>(numberOfBouncers);
    private bool bouncersAreActive = false;
    private BouncerDifficulty bouncerDifficulty = null;

    #endregion

    #region Public Methods

    /// <summary>
    /// Destroys exisiting bouncers if any were present and instantiates new ones
    /// </summary>
    public void InstantiateBouncers()
    {
        RemoveBouncers();
        Bouncer bouncerPrefab = AssetManager.LoadPrefabComponent<Bouncer>(AssetManager.BOUNCER_PREFAB_NAME);
        bouncerDifficulty = new BouncerDifficulty();

        bouncerList = new List<Bouncer>(numberOfBouncers);
        for (int i = 0; i < numberOfBouncers; ++i)
        {
            Bouncer newBouncer = Instantiate(bouncerPrefab, new Vector3(0, -1, 0), Quaternion.identity);
            newBouncer.Deactivate();
            bouncerList.Add(newBouncer);
        }

        bouncersAreActive = true;
    }

    /// <summary>
    /// Opposite of InstantiateBouncers function. Will remove all current bouncers
    /// </summary>
    public void RemoveBouncers()
    {
        bouncersAreActive = false;
        bouncerDifficulty = null;

        for (int i = 0; i < bouncerList.Count; ++i)
        {
            if (bouncerList[i] != null)
            {
                Destroy(bouncerList[i].gameObject);
                bouncerList[i] = null;
            }
        }
    }

    /// <summary>
    /// Deactivates previous bouncer and increments the bouncer index
    /// </summary>
    public void PrepareNextBouncer()
    {
        if (bouncersAreActive)
        {
            bouncerList[activeBouncerIndex].Deactivate();
            activeBouncerIndex = (activeBouncerIndex + 1) % bouncerList.Count;

            bouncerDifficulty.ScaleDifficulty();
        }
    }

    /// <summary>
    /// Launches the current active bouncer
    /// </summary>
    public void LaunchBouncer()
    {
        bouncerList[activeBouncerIndex].Activate(GenerateBouncerSpawnData());
    }

    /// <summary>
    /// Checks whether a given GameObject is the active bouncer
    /// </summary>
    /// <param name="gameObject">The game object to check</param>
    /// <returns>Whether it is equal</returns>
    public bool GameObjectIsActiveBouncer(GameObject gameObject)
    {
        if (bouncerList[activeBouncerIndex] != null && bouncerList[activeBouncerIndex].gameObject == gameObject)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates a BouncerSpawnData object with all the necesary parameters for a bouncer to spawn
    /// </summary>
    /// <returns></returns>
    private BouncerSpawnData GenerateBouncerSpawnData()
    {
        //constants
        float bouncerRadius = bouncerList[activeBouncerIndex].GetComponent<CircleCollider2D>().radius;
        float minRadius = 0.5f * ((Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero)).magnitude + 2f * bouncerRadius);

        //difficulty parameters
        float speed = bouncerDifficulty.Speed;
        float radius = Mathf.Clamp(bouncerDifficulty.RotationRadius, minRadius, Mathf.Infinity);
        float trajectoryWindowScale = bouncerDifficulty.TrajectoryWindowScale;


        //limits of trajectory point spawning area
        Vector2 spawnPortCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, 0f));
        Vector3 viewPortZero = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector2 lowerLeftLimit = spawnPortCenter + trajectoryWindowScale * (Vector2)(Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, -0.25f, 0f)) - viewPortZero) + new Vector2(bouncerRadius, bouncerRadius);
        Vector2 lowerRightLimit = spawnPortCenter + trajectoryWindowScale * (Vector2)(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, -0.25f, 0f)) - viewPortZero) + new Vector2(-bouncerRadius, bouncerRadius);
        Vector2 upperLeftLimit = spawnPortCenter + trajectoryWindowScale * (Vector2)(Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0.25f, 0f)) - viewPortZero) + new Vector2(bouncerRadius, -bouncerRadius);
        Vector2 upperRightLimit = spawnPortCenter + trajectoryWindowScale * (Vector2)(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, 0f)) - viewPortZero) + new Vector2(-bouncerRadius, -bouncerRadius);

        //take this point from the left half of the limits
        float firstX = UnityEngine.Random.Range(lowerLeftLimit.x, (lowerLeftLimit.x + lowerRightLimit.x) * 0.5f);
        float firstY = UnityEngine.Random.Range(lowerLeftLimit.y, upperLeftLimit.y);
        Vector2 firstPoint = new Vector2(firstX, firstY);

        //take this point from the right half 
        float secondX = UnityEngine.Random.Range((lowerLeftLimit.x + lowerRightLimit.x) * 0.5f, lowerRightLimit.x);
        float secondY = UnityEngine.Random.Range(lowerLeftLimit.y, upperLeftLimit.y);
        Vector2 secondPoint = new Vector2(secondX, secondY);

        Vector2 topPoint = firstPoint.y > secondPoint.y ? firstPoint : secondPoint;
        Vector2 bottomPoint = topPoint == firstPoint ? secondPoint : firstPoint;


        //find potential spawnpoints from previously generated points
        Vector2 centerOfRotation = CustomMath.CenterFromPointsAndRadius(topPoint, bottomPoint, radius);
        Vector2 intersept1 = CustomMath.IntersectionBetweenCircleAndLine(centerOfRotation, radius, 
            Camera.main.ViewportToWorldPoint(Vector3.zero) - new Vector3(bouncerRadius, bouncerRadius, 0f),
            Camera.main.ViewportToWorldPoint(Vector3.up) + new Vector3(-bouncerRadius, bouncerRadius, 0f));//left side of viewport
        Vector2 intersept2 = CustomMath.IntersectionBetweenCircleAndLine(centerOfRotation, radius, 
            Camera.main.ViewportToWorldPoint(Vector3.up) + new Vector3(-bouncerRadius, bouncerRadius, 0f),
            Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f)) + new Vector3(bouncerRadius, bouncerRadius, 0f));//top of viewport
        Vector2 intersept3 = CustomMath.IntersectionBetweenCircleAndLine(centerOfRotation, radius, 
            Camera.main.ViewportToWorldPoint(Vector3.right) + new Vector3(bouncerRadius, -bouncerRadius, 0f),
            Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f)) + new Vector3(bouncerRadius, bouncerRadius, 0f));//right side of viewport

        Vector2 spawnPoint;
        if (intersept2 != Vector2.zero)
        {
            spawnPoint = intersept2;
        } else
        {
            spawnPoint = intersept1.y > intersept3.y ? intersept1 : intersept3;
        }         

        return new BouncerSpawnData(spawnPoint, speed, centerOfRotation, bouncerDifficulty.Delay);
    }

    #endregion
}

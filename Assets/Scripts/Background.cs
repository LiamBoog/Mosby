using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Background : MonoBehaviour
{
    #region Inspector Controlled Variables

    [SerializeField] private Camera camera;
    [SerializeField] private GameObject background;
    [SerializeField] private bool newBakckgroundOnAwake = false;

    #endregion

    #region Member Variables

    private Dictionary<Vector3, GameObject> backgroundTiles;
    private Vector2 cameraSize;

    #endregion

    #region Public Methods

    /// <summary>
    /// Generates background tile randomly within camera view
    /// </summary>
    public void GenerateNewBackground()
    {
        float xRange = Random.Range(camera.transform.position.x - cameraSize.x, camera.transform.position.x + cameraSize.x);
        float yRange = Random.Range(camera.transform.position.y - cameraSize.y, camera.transform.position.y + cameraSize.y);
        Vector3 spawnLocation = new Vector3(xRange, yRange, 1f);

        backgroundTiles = new Dictionary<Vector3, GameObject>();
        AddBackgroundTile(spawnLocation);
    }

    
    /// <summary>
    /// Removes current background
    /// </summary>
    public void ClearBackground()
    {
        List<Vector3> positionsToBeRemoved = new List<Vector3>();
        foreach (Vector3 position in backgroundTiles.Keys)
        {
            positionsToBeRemoved.Add(position);
        }

        foreach (Vector3 position in positionsToBeRemoved)
        {
            Destroy(backgroundTiles[position]);
            backgroundTiles.Remove(position);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a background tile to the backgroundTiles dictionary
    /// </summary>
    /// <param name="position">The position of the background tile to be added</param>
    private void AddBackgroundTile(Vector3 position)
    {
        bool keyExists = false;
        foreach (Vector3 key in backgroundTiles.Keys)
        {
            if (position == key)
            {
                keyExists = true;
                break;
            }
        }

        if (!keyExists)
        {
            GameObject tile = Instantiate(background, position, Quaternion.identity);
            backgroundTiles.Add(position, tile);
        }
    }

    /// <summary>
    /// Removes a background tile from the backgroundTiles dictionary and destroys the gameobject
    /// </summary>
    /// <param name="position">The position of the tile to be removed</param>
    private void RemoveBackgroundTile(Vector3 position)
    {
        if (backgroundTiles.ContainsKey(position))
        {
            Destroy(backgroundTiles[position]);
            backgroundTiles.Remove(position);
        }
    }
    
    /// <summary>
    /// Adds new background tiles outside the viewport
    /// </summary>
    private void UpdateBackground()
    {
        List<Vector3> positionsToBeAdded = new List<Vector3>();
        foreach (Vector3 position in backgroundTiles.Keys)
        {
            BoxCollider2D collider = backgroundTiles[position].GetComponent<BoxCollider2D>();
            if (position.x - collider.size.x / 2f > camera.transform.position.x - cameraSize.x - 5f)
            {
                positionsToBeAdded.Add(new Vector3(position.x - collider.size.x / 2f, position.y, 1f));
            }
            if (position.x + collider.size.x / 2f < camera.transform.position.x + cameraSize.x + 5f)
            {
                positionsToBeAdded.Add(new Vector3(position.x + collider.size.x / 2f, position.y, 1f));
            }
            if (position.y - collider.size.y / 2f > camera.transform.position.y - cameraSize.y - 5f)
            {
                positionsToBeAdded.Add(new Vector3(position.x, position.y - collider.size.y / 2f, 1f));
            }
            if (position.y + collider.size.y / 2f < camera.transform.position.y + cameraSize.y + 5f)
            {
                positionsToBeAdded.Add(new Vector3(position.x, position.y + collider.size.y / 2f, 1f));
            }
        }

        foreach (Vector3 position in positionsToBeAdded)
        {
            AddBackgroundTile(position);
        }
    }

    /// <summary>
    /// Removes old background tiles that are no longer in view
    /// </summary>
    private void RemoveGarbage()
    {
        List<Vector3> positionsToBeRemoved = new List<Vector3>();
        foreach (Vector3 position in backgroundTiles.Keys)
        {
            BoxCollider2D collider = backgroundTiles[position].GetComponent<BoxCollider2D>();
            if (position.x + collider.size.x / 2f < camera.transform.position.x - cameraSize.x - 5f
                || position.x - collider.size.x / 2f > camera.transform.position.x + cameraSize.x + 5f
                || position.y + collider.size.y / 2f < camera.transform.position.y - cameraSize.y - 5f
                || position.y - collider.size.y / 2f > camera.transform.position.y + cameraSize.y + 5f)
            {
                positionsToBeRemoved.Add(position);
            }
        }

        foreach (Vector3 position in positionsToBeRemoved)
        {
            RemoveBackgroundTile(position);
        }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        float cameraSizeX = (camera.ViewportToWorldPoint(Vector3.right) - camera.ViewportToWorldPoint(Vector3.zero)).x;
        float cameraSiezY = (camera.ViewportToWorldPoint(Vector3.up) - camera.ViewportToWorldPoint(Vector3.zero)).y;
        cameraSize = new Vector2(cameraSizeX, cameraSiezY);

        if (newBakckgroundOnAwake)
        {
            GenerateNewBackground();
        }
    }

    private void Update()
    {
        UpdateBackground();
        RemoveGarbage();
    }

    #endregion


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColour : MonoBehaviour
{
    #region Private Members

    private float time;
    private float offset;
    Vector2 dimensions;

    #endregion

    #region Public Methods



    #endregion

    #region Unity Methods

    private void Awake()
    {
        offset = Random.Range(0f, 10000f);
        GetComponent<MeshRenderer>().material.SetFloat("Vector1_CD43AC69", offset);//generate and apply a random initial colour

        dimensions = new Vector2((Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero)).x, 
            (Camera.main.ViewportToWorldPoint(Vector3.up) - Camera.main.ViewportToWorldPoint(Vector3.zero)).y);//set dimensions to screen dimensions in world space
    }

    void Update()
    {
        time += 2f * Time.deltaTime;
        GetComponent<MeshRenderer>().material.SetFloat("Vector1_CD43AC69", offset + time);//shift colour over time

        dimensions = new Vector2((Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero)).x,
           (Camera.main.ViewportToWorldPoint(Vector3.up) - Camera.main.ViewportToWorldPoint(Vector3.zero)).y);
        transform.localScale = dimensions;//update dimensions
    }

    #endregion
}

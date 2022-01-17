using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMesh : MonoBehaviour
{
    /// <summary>
    /// Class to keep track of the forces acting upon each mesh vertex
    /// </summary>
    private class MeshVertex
    {
        public int Index;
        public Vector3 InitialPosition;
        public Dictionary<int, float> Neighbours;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public MeshVertex(int index, Vector3 initialPosition)
        {
            Index = index;
            InitialPosition = initialPosition;
            Neighbours = new Dictionary<int, float>();
            Velocity = Vector2.zero;
            Acceleration = Vector2.zero;
        }
    }

    #region Private Members

    private int resolution = 1000;
    private int numOfRows = 10;

    private Mesh mesh;
    private bool touchedInsideMesh = false;
    private Vector3 initialTouch = Vector3.zero;
    private List<int> selectedPoints;

    private Dictionary<int, MeshVertex> meshVertices;

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates circular mesh based on given parameters
    /// </summary>
    /// <param name="radius">Radius of the circle to be generated</param>
    /// <param name="numOfVertices">Number of vertices to be used for generation</param>
    /// <param name="numOfRows">Number of internal rows of vertices (number of surface vertices will be numOfVertices / numOfRows)</param>
    private void GenerateCircleMesh(float radius, int numOfVertices, int numOfRows)
    {
        while (numOfVertices % numOfRows != 0)
        {
            ++numOfVertices;
        }
        int vertsPerRow = numOfVertices / numOfRows;
        
        Vector3[] verts = new Vector3[numOfVertices + 1];
        int[] tris = new int[(numOfVertices / numOfRows * (numOfRows - 1) * 2 + numOfVertices / numOfRows) * 3];
        meshVertices = new Dictionary<int, MeshVertex>();

        float angle = 0f;
        float rowOffset = 0f;
        float radiusOffset = 1f;

        //create vertices
        verts[0] = Vector3.zero;
        for (int i = 1 ; i <= numOfVertices; i++)
        {
            if (i > 1 && i % (vertsPerRow) == 1)
            {
                rowOffset += 2f * Mathf.PI / (vertsPerRow) / 2f;
                angle = 0f;
                radiusOffset -= 1f / numOfRows;
            }

            verts[i] = verts[0] + new Vector3(Mathf.Cos(angle + rowOffset), Mathf.Sin(angle + rowOffset), 0f) * radius * radiusOffset;

            angle += 2f * Mathf.PI / vertsPerRow;
        }

        //define triangles
        int triangleIndex = -1;
        for (int i = 1; i <= numOfVertices - vertsPerRow; i++)
        {
            tris[++triangleIndex] = i;
            tris[++triangleIndex] = Mod(i, vertsPerRow) == 1 ? i - 1 + 2 * vertsPerRow : i - 1 + vertsPerRow;
            tris[++triangleIndex] = i + vertsPerRow;

            tris[++triangleIndex] = i;
            tris[++triangleIndex] = i + vertsPerRow;
            tris[++triangleIndex] = Mod(i + 1, vertsPerRow) == 1 ? i + 1 - vertsPerRow : i + 1;
        }
        for (int i = numOfVertices - vertsPerRow + 1; i <= numOfVertices; i++)
        {
            tris[++triangleIndex] = i;
            tris[++triangleIndex] = Mod(i, vertsPerRow) == 1 ? i + vertsPerRow - 1 : i - 1;
            tris[++triangleIndex] = 0;
        }

        //define MeshVertex objects
        for (int i = 1; i <= vertsPerRow; i++)
        {
            meshVertices.Add(i, new MeshVertex(i, verts[i]));

            meshVertices[i].Neighbours.Add(i + vertsPerRow, (verts[i + vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i + 1, vertsPerRow) == 1 ? i + 1 - vertsPerRow : i + 1, (verts[Mod(i + 1, vertsPerRow) == 1 ? i + 1 - vertsPerRow : i + 1] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 1 ? i - 1 + 2 * vertsPerRow : i - 1 + vertsPerRow, (verts[Mod(i, vertsPerRow) == 1 ? i - 1 + 2 * vertsPerRow : i - 1 + vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 1 ? i - 1 + vertsPerRow : i - 1, (verts[Mod(i, vertsPerRow) == 1 ? i - 1 + vertsPerRow : i - 1] - verts[i]).magnitude);
        }
        for (int i = vertsPerRow + 1; i <= numOfVertices - vertsPerRow; i++)
        {
            meshVertices.Add(i, new MeshVertex(i, verts[i]));

            meshVertices[i].Neighbours.Add(i + vertsPerRow, (verts[i + vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i + 1, vertsPerRow) == 1 ? i + 1 - vertsPerRow : i + 1, (verts[Mod(i + 1, vertsPerRow) == 1 ? i + 1 - vertsPerRow : i + 1] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 1 ? i - 1 + 2 * vertsPerRow : i - 1 + vertsPerRow, (verts[Mod(i, vertsPerRow) == 1 ? i - 1 + 2 * vertsPerRow : i - 1 + vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 1 ? i - 1 + vertsPerRow : i - 1, (verts[Mod(i, vertsPerRow) == 1 ? i - 1 + vertsPerRow : i - 1] - verts[i]).magnitude);

            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 0 ? i + 1 - 2 * vertsPerRow : i + 1 - vertsPerRow, (verts[Mod(i, vertsPerRow) == 0 ? i - 2 * vertsPerRow + 1 : i + 1 - vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(i - vertsPerRow, (verts[i - vertsPerRow] - verts[i]).magnitude);
        }
        for (int i = numOfVertices - vertsPerRow + 1; i <= numOfVertices; i++)
        {
            meshVertices.Add(i, new MeshVertex(i, verts[i]));

            meshVertices[i].Neighbours.Add(0, (verts[0] - verts[i]).magnitude);

            meshVertices[i].Neighbours.Add(Mod(i, vertsPerRow) == 0 ? i + 1 - 2 * vertsPerRow : i + 1 - vertsPerRow, (verts[Mod(i, vertsPerRow) == 0 ? i - 2 * vertsPerRow + 1 : i + 1 - vertsPerRow] - verts[i]).magnitude);
            meshVertices[i].Neighbours.Add(i - vertsPerRow, (verts[i - vertsPerRow] - verts[i]).magnitude);
        }

        //Set mesh parameters
        mesh.vertices = verts;
        mesh.triangles = tris;
    }

    /// <summary>
    /// Stretches outer layer of mesh toward finger
    /// </summary>
    private void StretchMesh()
    {
        if (Input.touchCount > 0)
        {
            int vertsPerRow = mesh.vertices.Length / numOfRows;

            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = (Vector2)transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.position));

            Vector3[] verts = mesh.vertices;

            if (GetComponent<CircleCollider2D>().OverlapPoint(transform.TransformPoint(touchPosition)))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    initialTouch = touchPosition;
                    touchedInsideMesh = true;

                    selectedPoints.Add(1);
                    for (int i = 2; i <= vertsPerRow / 1; i++)
                    {
                        selectedPoints.Add(i);
                        selectedPoints.Add(Mod(-i, vertsPerRow) + 2);
                    }
                }
            }

            if (touchedInsideMesh)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 touchDelta = touchPosition - initialTouch;
                    Vector3 delta = touchDelta.normalized * 0.1f;
                    float c = 15f * delta.magnitude;
                    float a = delta.magnitude / (-Mathf.Atan(1 - c) + Mathf.PI / 2f);
                    float d = 0.25f;

                    while ((verts[selectedPoints[0]] - touchPosition).magnitude > 0.10001f && !GetComponent<CircleCollider2D>().OverlapPoint(transform.TransformPoint(touchPosition)))
                    {
                        verts[selectedPoints[0]] += delta.normalized * a * (-Mathf.Atan(d - c) + Mathf.PI / 2f);
                        for (int i = 1; i <= (selectedPoints.Count - 1) / 2; i += 2)
                        {
                            verts[selectedPoints[i]] += delta.normalized * a * (-Mathf.Atan(d * (i + 1) - c) + Mathf.PI / 2f);
                            verts[selectedPoints[i + 1]] += delta.normalized * a * (-Mathf.Atan(d * (i + 1) - c) + Mathf.PI / 2f);
                        }
                    }

                    initialTouch = touchPosition;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                touchedInsideMesh = false;
            }

            mesh.vertices = verts;
        } else
        {
            selectedPoints.Clear();
        }

    }

    /// <summary>
    /// Update meshes internal forces to acheive stretchiness and bounciness
    /// </summary>
    private void UpdateMeshForces()
    {
        Vector3[] verts = mesh.vertices;

        foreach (MeshVertex vertex in meshVertices.Values)
        {
            if (!selectedPoints.Contains(vertex.Index))
            {
                verts[vertex.Index] += 0.5f * (Vector3)vertex.Acceleration * Time.deltaTime * Time.deltaTime;
                vertex.Velocity = vertex.Acceleration * Time.deltaTime;

                Vector2[] forceComponents = new Vector2[vertex.Neighbours.Count];
                int compenentIndex = 0;
                foreach (int index in vertex.Neighbours.Keys)
                {
                    forceComponents[compenentIndex] = (vertex.Neighbours[index] - (verts[vertex.Index] - verts[index]).magnitude) * (verts[vertex.Index] - verts[index]).normalized;
                    ++compenentIndex;
                }

                for (int i = 1; i < compenentIndex; i++)
                {
                    forceComponents[0] += forceComponents[i];
                    forceComponents[0] += 0.5f * (Vector2)(vertex.InitialPosition - verts[vertex.Index]);
                }

                vertex.Acceleration = 1000f * forceComponents[0];
            }
        }

        mesh.vertices = verts;
    }

    /// <summary>
    /// Rotates mesh to point vert[0] toward finger
    /// </summary>
    private void RotateMesh()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);

            Vector3 rotationVector = touchPosition - transform.position;
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector3.down, rotationVector) - 90f);

            transform.rotation = rotation;
        }
    }

    /// <summary>
    /// Modulo function
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    #endregion

    #region Unity Methods

    public void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        selectedPoints = new List<int>();

        GenerateCircleMesh(GetComponent<CircleCollider2D>().radius, resolution, numOfRows);
    }

    public void Update()
    {
        RotateMesh();
        StretchMesh();
        UpdateMeshForces();
    }

    #endregion
}

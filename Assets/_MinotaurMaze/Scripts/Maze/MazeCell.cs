using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [Header("Generation properties")]
    public Vector2 gridPos;
    public bool visited = false;
    public TextMesh debugText;
    public Material visitedMat;

    [Header("Cell elements")]
    public MeshRenderer ground;
    public GameObject topWall;
    public GameObject rightWall;
    public GameObject bottomWall;
    public GameObject leftWall;

    #region Generation
    internal void Init(Vector2 gridPos)
    {
        this.gridPos = gridPos;
        string pos = $"({gridPos.x},{gridPos.y})";
        this.name = "Cell" + pos;

        SetDebugText(pos);
        SetVisited(false);
    }

    internal void SetVisited(bool visited) 
    { 
        this.visited = visited; 
        if (this.visited == true)
        {
            ground.material = visitedMat;
        }
    }

    internal void SetDebugText(string text) { debugText.text = text; }

    public void RemoveWall(Vector2 dir)
    {
        GameObject destroyWall = null;

        if (dir.x == 1)
        {
            destroyWall = rightWall;
        } else if (dir.x == -1)
        {
            destroyWall = leftWall;
        } else if (dir.y == 1)
        {
            destroyWall = topWall;
        } else if (dir.y == -1)
        {
            destroyWall = bottomWall;
        }

        if (destroyWall != null)
            Destroy(destroyWall);
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [Header("Generation properties")]
    public Vector2 gridPos;
    public List<MazeCell> neighbors;
    internal bool visited = false;

    [Header("Debug materials")]
    public Material visitedMat;
    public Material revisitedMat;

    [Header("Cell elements")]
    public TextMesh debugText;
    public MeshRenderer ground;
    public GameObject topWall;
    public GameObject rightWall;
    public GameObject bottomWall;
    public GameObject leftWall;

    #region Generation
    /// <summary>
    /// Initialization of the cell
    /// </summary>
    /// <param name="gridPos">Position of the cell within the grid</param>
    internal void Init(Vector2 gridPos)
    {
        this.gridPos = gridPos;
        string pos = $"({gridPos.x},{gridPos.y})";
        this.name = "Cell" + pos;

        debugText.text = pos;
        neighbors = new List<MazeCell>();
        SetVisited(false);
    }

    /// <summary>
    /// Method to mark the cell as visited when generating the maze
    /// </summary>
    /// <param name="visited"></param>
    internal void SetVisited(bool visited) 
    {
        if (this.visited)
        {
            // If this cell has already been visited then change the material
            ground.material = revisitedMat;
        } else
        {
            this.visited = visited;
            if (this.visited == true)
            {
                ground.material = visitedMat;
            }
        }
    }

    /// <summary>
    /// Method to remove the wall which belongs to the assigned direction
    /// </summary>
    /// <param name="dir">Direction of the wall that needs to be removed</param>
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

    /// <summary>
    /// Method to add the assigned neighbor to the neighbor list of this cell
    /// </summary>
    /// <param name="mazeCell">Neighbor to add</param>
    internal void AddNeighbor(MazeCell mazeCell)
    {
        if (neighbors.Contains(mazeCell) == false) { neighbors.Add(mazeCell); }
    }
    #endregion
}

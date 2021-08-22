using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private MazeCell startCell, endCell;
    public Material startMat, endMat, pathMat;

    public MazeGenerator generator;
    internal List<MazeCell> path;

    private void Update()
    {
        // Only allow clicking once generation is finished
        if (generator.finishedGeneration == false) { return; }

        // Check raycast to select a start point
        if (Input.GetMouseButtonUp(0))
        {
            MazeCell selectedObj = SelectCell();
            if (selectedObj) 
            {
                // Can't place the start cell ontop of the endcell
                if (selectedObj == endCell) { return; }

                // Deselect the endcell if we click on it again
                if (selectedObj == startCell)
                {
                    startCell.ground.material = startCell.revisitedMat;
                    startCell = null;
                    ResetPath();
                    return;
                }
                else if (startCell)
                {
                    // Deslect previous cell if there is one assigned
                    startCell.ground.material = startCell.revisitedMat;
                }

                // Cell is not yet selected so select it.
                ResetPath();
                startCell = selectedObj;
                startCell.ground.material = startMat;
            }
        }

        // Check raycast to select a end point
        if (Input.GetMouseButtonUp(1))
        {
            MazeCell selectedObj = SelectCell();
            if (selectedObj)
            {
                // Can't place the end cell ontop of the start cell
                if (selectedObj == startCell) { return; }

                // Deselect the endcell if we click on it again
                if (selectedObj == endCell)
                {
                    endCell.ground.material = endCell.revisitedMat;
                    endCell = null;
                    ResetPath();
                    return;
                } else if (endCell)
                {
                    // Deslect previous cell if there is one assigned
                    endCell.ground.material = endCell.revisitedMat;
                }

                // Cell is not yet selected so select it.
                ResetPath();
                endCell = selectedObj;
                endCell.ground.material = endMat;
            }
        }
    }

    /// <summary>
    /// Method to check for a mazeCell object through a raycast
    /// </summary>
    /// <returns>MazeCell</returns>
    private MazeCell SelectCell()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            MazeCell cell = hit.transform.gameObject.GetComponent<MazeCell>();
            
            // Block the user from selecting this cell since it is not reachable
            if (cell != null && cell.visited == false) { cell = null; }
            return cell;
        }
        return null;
    }

    /// <summary>
    /// Method to reset the path and materials of every cell
    /// </summary>
    internal void ResetPath()
    {
        if (path != null && path.Count > 0)
        {
            foreach (MazeCell cell in path.Skip(1))
            {
                cell.ground.material = cell.revisitedMat;
            }
        }

        path = new List<MazeCell>();
    }

    /// <summary>
    /// Method to find the path between the already assigned start/end cell
    /// </summary>
    /// <returns>List of cells which make the path</returns>
    internal List<MazeCell> FindPath() { return FindPath(startCell, endCell); }

    /// <summary>
    /// Method to find the path between the assigned start/end cell
    /// </summary>
    /// <returns>List of cells which make the path</returns>
    internal List<MazeCell> FindPath(MazeCell startCell, MazeCell endCell)
    {
        if (startCell == null || endCell == null)
        {
            Debug.LogError("Unable to find the path with the assigned location variables!");
            return null;
        }

        ResetPath();
        this.startCell = startCell;
        this.endCell = endCell;

        // Start finding the path
        CheckNextNeighbor(startCell, path);

        // Debugging material
        foreach (MazeCell cell in path.Skip(1))
        {
            cell.ground.material = pathMat;
        }
        return path;
    }

    /// <summary>
    /// Method to check the neighbors of the current cell within the path for a possible path
    /// </summary>
    /// <param name="takenPath"></param>
    private void CheckNextNeighbor(MazeCell currentCell, List<MazeCell> takenPath)
    {
        takenPath.Add(currentCell);

        // Check every neighbor for a possible path
        for (int i = 0; i < currentCell.neighbors.Count; i++)
        {
            if (takenPath.Contains(currentCell.neighbors[i])) {
                // We have been here already, Check the next neighbor
                continue;
            } 
            else if (currentCell.neighbors[i] == endCell) 
            {
                // We have found the end cell, check if we didn't find another path which is faster
                if (path.Count <= 1 || takenPath.Count < path.Count) 
                {
                    path = takenPath;
                    break;
                }
            } else
            {
                // This is a new cell, so move towards this one
                List<MazeCell> newPath = new List<MazeCell>(takenPath);
                CheckNextNeighbor(currentCell.neighbors[i], newPath);
            }
        }
    }
}

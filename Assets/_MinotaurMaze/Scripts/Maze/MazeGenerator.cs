using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static event Action GenerationFinished;

    [Header("Grid Generation")]
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float cellMargin = 0f;
    private float cellWidth = 1;
    public float generateDelay = 0;

    internal bool finishedGeneration;

    public Transform mazeParent;
    public Transform cellParent;
    public MazeCell cellPrefab;

    private MazeCell[][] cells;
    private List<MazeCell> generatedPath;
    private Vector2[] directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left }; // N, E, S, W

    /// <summary>
    /// Method to generate a new maze
    /// </summary>
    internal void CreateMaze()
    {
        ClearMaze();
        CreateCells();
        GenerateMaze();
    }

    /// <summary>
    /// Method to remove the current generated maze.
    /// </summary>
    private void ClearMaze()
    {
        // Only clear the maze if one has been generated before
        if (cells != null)
        {
            for (int c = 0; c < cells.Length; c++)
            {
                for (int r = 0; r < cells[c].Length; r++)
                {
                    Destroy(cells[c][r].gameObject);
                }
            }
        }

        // Create a new a new list and array
        generatedPath = new List<MazeCell>();
        cells = new MazeCell[gridWidth][];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new MazeCell[gridHeight];
        }

        // Reset the parent to it"t original location
        mazeParent.position = new Vector3(0, mazeParent.position.y, 0);
    }

    /// <summary>
    /// Method to create a grid with the assigned width and height
    /// </summary>
    private void CreateCells()
    {
        finishedGeneration = false;

        Debug.Log($"Generating grid... \n - Width: {gridWidth} \n - Height: {gridHeight} ");
        for (int r = 0; r < gridWidth; r++)
        {
            for (int c = 0; c < gridHeight; c++)
            {
                MazeCell cell = Instantiate(cellPrefab, cellParent);
                float xPos = (cellWidth * r) + (cellMargin * r);
                float zPos = (cellWidth * c) + (cellMargin * c);

                cell.Init(new Vector2(r, c));
                cell.transform.position = new Vector3(xPos, cellParent.position.y, zPos);
                cells[r][c] = cell;
            }
        }

        // Relocate the parent so that the maze is in the center view of the camera
        float mazeX = ((cellWidth * gridWidth + cellMargin * gridWidth) / 2);
        float mazeY = ((cellWidth * gridHeight + cellMargin * gridHeight) / 2);
        mazeParent.position = new Vector3(-mazeX, mazeParent.position.y, -mazeY);
    }

    /// <summary>
    /// Method to generate a maze within the created grid using the 'Recursive Backtracking'
    /// </summary>
    private void GenerateMaze()
    {
        MazeCell startCell = cells[0][0];

        StartCoroutine(CheckCell(startCell));
    }

    /// <summary>
    /// Method to check for a new neighbor to go towards
    /// </summary>
    /// <param name="currentCell">The current cell that is being checked for neighbors</param>
    private IEnumerator CheckCell(MazeCell currentCell)
    {
        yield return new WaitForSeconds(generateDelay);

        // Add the cell to the list if this is a new cell
        if (currentCell.visited == false)
        {
            generatedPath.Add(currentCell);
        } else
        {
            // If this is not a new cell then we backtracked so remove him from the list.
            generatedPath.Remove(currentCell);

            if (generatedPath.Count == 0)
            {
                // We are back to out starting point
                Debug.Log("Generation has been finished!");
                finishedGeneration = true;
                GenerationFinished?.Invoke();
                currentCell.SetVisited(true);
                yield break;
            }
        }

        currentCell.SetVisited(true);
        CheckNeighbor(currentCell);
    }

    /// <summary>
    /// Method to check if there is an available neighbor to go towards
    /// </summary>
    /// <param name="currentCell">The current cell that is being checked for neighbors</param>
    private void CheckNeighbor(MazeCell currentCell)
    {
        // Pick random unvisited neighbor, leave all directions open to check
        MazeCell randomNeighbor = GetRandomUnvisitedNeighbor(currentCell, directions);
        if (randomNeighbor == null)
        {
            // Go back the previous cell to check if this one still has unvisited neighbors
            StartCoroutine(CheckCell(generatedPath[generatedPath.Count - 1]));
        }
        else
        {
            // This cell has not been visited yet, continue
            currentCell.AddNeighbor(randomNeighbor);
            randomNeighbor.AddNeighbor(currentCell);
            StartCoroutine(CheckCell(randomNeighbor));
        }
    }

    /// <summary>
    /// Method to get a random unvisited neighbor
    /// </summary>
    /// <param name="currentCell">Current cell which checks for neighbors</param>
    /// <param name="availableDirs">The directions which have not been checked yet</param>
    /// <returns></returns>
    private MazeCell GetRandomUnvisitedNeighbor(MazeCell currentCell, Vector2[] availableDirs)
    {
        // Check if there are still directions to check
        if (availableDirs.Length <= 0) { return null; }

        // Get a random available direction
        List<Vector2> availableDirections = availableDirs.ToList();
        int randomIndex = UnityEngine.Random.Range(0, availableDirections.Count);
        Vector2 dir = directions[randomIndex];
        availableDirections.RemoveAt(randomIndex);

        // Check if there is a border in that direction
        if (currentCell.gridPos.x + dir.x < 0 || currentCell.gridPos.x + dir.x > gridWidth - 1 ||
            currentCell.gridPos.y + dir.y < 0 || currentCell.gridPos.y + dir.y > gridHeight - 1)
        {
            // Try another side
            return GetRandomUnvisitedNeighbor(currentCell, availableDirections.ToArray());
        }

        // Check if the neighbor in that direction has already been visited
        MazeCell neighbor = cells[(int)(currentCell.gridPos.x + dir.x)][(int)(currentCell.gridPos.y + dir.y)];
        if (neighbor.visited == true)
        {
            // Try another side
            return GetRandomUnvisitedNeighbor(currentCell, availableDirections.ToArray());
        }

        // Return this unvisited neighbor
        currentCell.RemoveWall(dir);
        neighbor.RemoveWall(-dir);
        return neighbor;
    }

    /// <summary>
    /// Method to get the dimensions of the current generated maze
    /// </summary>
    /// <returns>Current maze dimensions</returns>
    internal Vector2 GetMazeDimensions() { return new Vector2(gridWidth, gridHeight); }

    /// <summary>
    /// Method to set the dimensions of the maze
    /// </summary>
    /// <param name="dimensions">The new dimensions of the maze</param>
    internal void SetMazeDimensions(Vector2 dimensions, float margin)
    {
        gridWidth = (int)dimensions.x;
        gridHeight = (int)dimensions.y;
        cellMargin = margin;
    }
}

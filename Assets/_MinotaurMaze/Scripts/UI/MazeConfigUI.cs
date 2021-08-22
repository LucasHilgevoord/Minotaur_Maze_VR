using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeConfigUI : MonoBehaviour
{
    public MazeGenerator generator;
    public PathFinder pathFinder;
    public RectTransform rect;
    private bool isOpen = true;

    public bool generateOnStart = false;
    public int minMazeWidth = 10;
    public int minMazeHeight = 10;
    public float minMazeMargin = 0;

    [Header("Input fields")]
    public InputField mazeWidth;
    public InputField mazeHeight;
    public InputField mazeMargin;

    [Header("Buttons")]
    public Button generateButton;
    public Button findPathButton;

    private void Awake()
    {
        MazeGenerator.GenerationFinished += OnGenerationFinished;
    }

    private void OnDestroy()
    {
        MazeGenerator.GenerationFinished -= OnGenerationFinished;
    }

    private void Start()
    {
        findPathButton.interactable = false;

        Vector2 d = generator.GetMazeDimensions();
        mazeWidth.text = d.x.ToString();
        mazeHeight.text = d.y.ToString();
        mazeMargin.text = minMazeMargin.ToString();

        if (generateOnStart) { GenerateMaze(); }
    }

    /// <summary>
    /// Method to cap the width to the required value once done editing
    /// </summary>
    public void CheckWidth() 
    { 
        if (string.IsNullOrEmpty(mazeWidth.text) || int.Parse(mazeWidth.text) < minMazeWidth)
        {
            mazeWidth.text = minMazeWidth.ToString();
        }
    }
    
    /// <summary>
    /// Method to cap the height to the required value once done editing
    /// </summary>
    public void CheckHeight() 
    { 
        if (string.IsNullOrEmpty(mazeHeight.text) || int.Parse(mazeHeight.text) < minMazeHeight)
        {
            mazeHeight.text = minMazeHeight.ToString();
        }
    }
    
    /// <summary>
    /// Method to cap the margin to the required value once done editing
    /// </summary>
    public void CheckMargin() 
    { 
        if (string.IsNullOrEmpty(mazeMargin.text) || float.Parse(mazeMargin.text) < minMazeMargin)
        {
            mazeMargin.text = minMazeMargin.ToString();
        }
    }

    /// <summary>
    /// Method to show/hide the UI
    /// </summary>
    public void ToggleUI()
    {
        DOTween.Kill(rect);

        isOpen = !isOpen;
        float xPos = isOpen ? 0 : rect.rect.width;
        rect.DOAnchorPosX(xPos, 0.5f);
    }

    /// <summary>
    /// Method to tell the genarator to create a new maze
    /// </summary>
    public void GenerateMaze()
    {
        Vector2 d = new Vector2(int.Parse(mazeWidth.text), int.Parse(mazeHeight.text));
        float m = float.Parse(mazeMargin.text);
        generator.SetMazeDimensions(d, m);
        generator.CreateMaze();

        pathFinder.path = null;

        generateButton.interactable = false;
        findPathButton.interactable = false;
    }

    /// <summary>
    /// Method to start the pathfinder
    /// </summary>
    public void FindPath() { pathFinder.FindPath(); }

    /// <summary>
    /// Method which gets called once the generation has been finished which toggles everything on again
    /// </summary>
    private void OnGenerationFinished()
    {
        generateButton.interactable = true;
        findPathButton.interactable = true;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize = 1;

    private PuzzleCell[,] grid;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize; 

    public PuzzleCell[,] Grid => grid;

    private void Awake()
    {
        CreateGrid(width, height);
    }

    public void CreateGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        grid = new PuzzleCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new PuzzleCell(x, y);
            }
        }
    }

    public PuzzleCell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        return grid[x, y];
    }
}

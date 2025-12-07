using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGridModel
{


    // public Cell[,] grid;
    public int width { get; }
    public int height { get; }
    public int colorCount { get; }


    //public GridModel(int widht, int height);

    public void InitGrid();

    public bool HasMatches();

    public List<Vector2Int> FindAllMathes();

    public void Swap(Vector2Int position1, Vector2Int position2);

    public Cell GetCell(int i, int j);

    public Cell[,] GetGrid();
}


[Serializable]
public class Cell
{
    public int type { get; set; }
    public bool isEmpty;
    public Crystal view;
}
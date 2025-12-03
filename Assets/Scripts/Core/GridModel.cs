using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering;



[Serializable]
public class GridModel
{
    [Serializable]
    public class Cell
    {
        public int type;
        public bool isEmpty;
        public Crystal view;
    }

    public Cell[,] grid;
    public int width = 5;
    public int height = 5;
    public int colorCount = 5;


    public GridModel(int widht, int height)
    {
        this.width = widht;
        this.height = height;
        grid = new Cell[this.width, this.height];

        for (int i = 0; i < widht; i++)
        {
            for(int j = 0; j < height; j++) 
            {
                grid[i, j] = new Cell();
            }
        }
    }

    public void InitGrid() 
    {
        do
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j].type = UnityEngine.Random.Range(0, colorCount);
                    grid[i, j].isEmpty = false;
                }
            }
        }
        while (HasMatches());
    }

    public bool HasMatches() 
    {
        return FindAllMathes().Count > 0;
    }


    public List<Vector2Int> FindAllMathes()
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>(); // the result of the method execution will be written here after all matches have been found


        // Horizontal search
        for (int j = 0; j < height; j++)
        {
            int matchLength = 1;

            for(int i = 1; i < width; i++)
            {
                if (!grid[i, j].isEmpty && !grid[i - 1, j].isEmpty && grid[i, j].type == grid[i - 1, j].type)
                {
                    matchLength++;
                }
                else 
                {
                    if(matchLength >= 3)
                    {
                        for(int k = i - matchLength; k < i; k++)
                        {
                            result.Add(new Vector2Int(k, j));
                        }
                    }

                    matchLength = 1;
                }
            }

            if(matchLength >= 3)
            {
                for (int k = width - matchLength; k < width; k++)
                {
                    result.Add(new Vector2Int(k, j));
                }
            }
        }

        //Vertical search
        for(int i = 0; i < width; i++)
        {
            int matchLength = 1;

            for(int j = 1; j < height; j++)
            {
                if (!grid[i,j].isEmpty && !grid[i, j - 1].isEmpty && grid[i, j].type == grid[i, j - 1].type)
                {
                    matchLength++;
                }
                else 
                {
                    if (matchLength >= 3)
                    {
                        for (int k = j - matchLength; k < j; k++)
                        {
                            result.Add(new Vector2Int(i, k));
                        }
                    }

                    matchLength = 1;
                }
            }

            if (matchLength >= 3) 
            {
                for (int k = height - matchLength; k < height; k++)
                {
                    result.Add(new Vector2Int(i, k));
                }
            }
        }


        return new List<Vector2Int>(result);
    }


    public void Swap(Vector2Int position1, Vector2Int position2)
    {
        int tempType = grid[position1.x, position1.y].type;
        Crystal tempVeiw = grid[position1.x, position1.y].view;

        grid[position1.x, position1.y].type = grid[position2.x, position2.y].type;
        grid[position1.x, position1.y].view = grid[position2.x, position2.y].view;

        grid[position2.x, position2.y].type = tempType;
        grid[position2.x, position2.y].view = tempVeiw;

    }

    public Cell GetCell(int i, int j) 
    {
        if(i < 0 && i > width && j < 0 && j > height)
            return null;

        return grid[i, j];
    }
}

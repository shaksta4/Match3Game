using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    //public vars
    public List<Cell> Neighbours = new List<Cell>();
    public Tile tile;
    public GameObject tilePrefab;
    public Cell LeftCell, BelowCell;
    public bool isLeftEdge, isBottomEdge;

    //private vars
    private int xPos, yPos;
   
    //Initialises a cell by setting its xPos and yPos variables, and sets whether its a left edge or bottom edge cell. It also generates a new tile for the cell.
    public void Initialise(int xPos, int yPos)
    {
        //Sets xPos and yPos. 
        this.xPos = xPos;
        this.yPos = yPos;

        if(xPos == 0)
        {
            this.isLeftEdge = true;
        }
        if(yPos == 0)
        {
            this.isBottomEdge = true;
        }

        tile = GenerateNewTile();
    }

    /*This function instantiates a tile, sets it's position and returns it.*/
    public Tile GenerateNewTile()
    {
        GameObject g = Instantiate(tilePrefab, this.transform.position, Quaternion.identity) as GameObject;
        g.transform.parent = gameObject.transform;

        // Done so the collider is able to be clicked properly.
        Vector3 position = g.GetComponent<Tile>().transform.localPosition;
        position.z = -0.1f;
        g.GetComponent<Tile>().transform.localPosition = position;

        return g.GetComponent<Tile>();
    }

    public void AddNeighbour(Cell c)
    {
        this.Neighbours.Add(c);
    }

    public bool IsNeighbour(Cell c)
    {
        if (Neighbours.Contains(c))
        {
            return true;
        }
        return false;
    }
}

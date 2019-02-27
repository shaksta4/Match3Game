using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public List<Cell> Neighbours = new List<Cell>();
    public Tile tile;
    public GameObject tilePrefab;

    public Cell LeftCell;
    public Cell BelowCell;

    public bool isLeftEdge;
    public bool isBottomEdge;

    public int xPos, yPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Initialises a cell
    public void Initialise(int xPos, int yPos)
    {
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

    //Is called when a tile intersects a cell.
    void OnCollisionEnter2D(Collision2D col)
    {
        print("collision!!");
    }

    public string toString()
    {
        return "This cell is a position: " + xPos + ", " + yPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();
    public int GridWidth;
    public int GridHeight;
    public GameObject tilePrefab;
    public Tile PrevTile;

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
        FindNeighbours();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateBoard()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                GameObject g = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                g.transform.parent = gameObject.transform;
                tiles.Add(g.GetComponent<Tile>());

                tiles[GridWidth * y].isLeftEdge = true;
                if (y == 0)
                {
                    tiles[x].isBottomEdge = true;
                }
            }
        }
        gameObject.transform.position = new Vector3(-2.5f, -2f, -0.5f);
    }

    void FindNeighbours()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].isLeftEdge)
            {
                tiles[i].LeftTile = tiles[i - 1];
            }

            if (!tiles[i].isBottomEdge)
            {
                tiles[i].BelowTile = tiles[i - GridWidth];
            }
        }
        foreach (Tile t in tiles)
        {
            if(t.LeftTile != null)
            {
                t.AddNeighbour(t.LeftTile);
                t.LeftTile.AddNeighbour(t);
            }

            if(t.BelowTile != null)
            {
                t.AddNeighbour(t.BelowTile);
                t.BelowTile.AddNeighbour(t);
            } 
        }
    }

    public void TileSwap(Tile CurrentTile)
    {
        if (PrevTile == null)
        {
            print("First tile selected");
            PrevTile = CurrentTile;
            CurrentTile.ToggleSelect();
        }
        else if (PrevTile == CurrentTile)
        {
            print("Deselecting tile");
            PrevTile = null;
            CurrentTile.ToggleSelect();
        }
        //If Second clicked tile is neighbour with first clicked tile
        else if (PrevTile.IsNeighbour(CurrentTile))
        {
            print("Second tile selected");
            CurrentTile.ToggleSelect();
        }
        else
        {
            print("error");
        }


        /*else
        {
            if (PrevTile.IsNeighbour(CurrentTile))
            {

            }
            else
            {
                PrevTile.ToggleSelect();
                PrevTile = CurrentTile;
            }
        }*/
    }
}

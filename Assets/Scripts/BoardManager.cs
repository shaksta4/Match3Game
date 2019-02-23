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
        //Loop through all Tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            //If it's not a Left edge tile
            if (!tiles[i].isLeftEdge)
            {
                //Set its left tile to one element to its left
                tiles[i].LeftTile = tiles[i - 1];
            }

            //If its not a bottom edge tile
            if (!tiles[i].isBottomEdge)
            {
                //Set its bottom tile to the tile below it
                tiles[i].BelowTile = tiles[i - GridWidth];
            }
        }

        //Loop through all tiles
        foreach (Tile t in tiles)
        {
            //If it has a tile on its left
            if(t.LeftTile != null)
            {
                //Add the left tile as a neighbour
                t.AddNeighbour(t.LeftTile);
                //Add this tile as the left tile's neighbour
                t.LeftTile.AddNeighbour(t);
            }

            //If it has a tile below it
            if(t.BelowTile != null)
            {
                //Add the tile below as a neighbour
                t.AddNeighbour(t.BelowTile);
                //Add thhis tile as the below tile's neighbour
                t.BelowTile.AddNeighbour(t);
            } 
        }
    }

    public void TileSwap(Tile CurrentTile)
    {
        Sprite SpriteBuffer;
        int ValueBuffer;

        if (PrevTile == null)
        {
            print("First tile selected");
            PrevTile = CurrentTile;
            CurrentTile.ToggleSelect();
        }
        else if (CurrentTile == PrevTile)
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

            //Swap positions of both sprites. It is done this way to maintain the correct neighbours.
            SpriteBuffer = PrevTile.GetComponent<SpriteRenderer>().sprite;
            PrevTile.GetComponent<SpriteRenderer>().sprite = CurrentTile.GetComponent<SpriteRenderer>().sprite;
            CurrentTile.GetComponent<SpriteRenderer>().sprite = SpriteBuffer;

            //Swapping values of both tiles
            ValueBuffer = PrevTile.value;
            PrevTile.value = CurrentTile.value;
            CurrentTile.value = ValueBuffer;

            //Find selected tiles and deselect them
            ResetBoard();
            CurrentTile = null;
            PrevTile = null;
        }
        else
        {
            print("Error 80085: Not a valid neighbour.");
        }
    }

    void ResetBoard()
    {
        foreach (Tile t in tiles)
        {
            if (t.isSelected)
            {
                t.ToggleSelect();
            }
        }
    }
}

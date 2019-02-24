using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();
    public List<Tile> MatchingSet = new List<Tile>();
    public List<Tile> AboveTile = new List<Tile>();

    [SerializeField]
    public Dictionary<string, List<Tile>> BoardRayCasts = new Dictionary<string, List<Tile>>();

    public int GridWidth;
    public int GridHeight;
    public GameObject tilePrefab;
    public Tile PrevTile;

    public bool hasMadeTurn = false;
    public bool needReshuffle = false;
    public bool hasReachedTop = false;

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
        FindNeighbours();
        InitDictionary();
        CheckBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if(hasMadeTurn)
        {
            CheckBoard(); //Implement something to do after checking board.
            if(MatchingSet.Count < 1) // If the move does not result in 3 in a row from happening, do something.
            {

            }

            for (int i = MatchingSet.Count - 1; i >= 0; i--)
            {
                if(MatchingSet[i].toBeNulled == false)
                {
                    MatchingSet[i].toBeNulled = true;
                }

                MoveTilesUp(MatchingSet[i], MatchingSet[i].Neighbours);

            }
            

            /*for(int i= 0; i < MatchingSet.Count; i++)
            {
                MatchingSet[i] = null;
            }*/
            //Resets MatchingSet
           // MatchingSet = new List<Tile>();
            
            if(hasReachedTop)
            {
                MatchingSet = new List<Tile>();
                hasMadeTurn = false;
            }
        }
        if(needReshuffle)
        {
            Reshuffle();
            needReshuffle = false;
            CheckBoard();
        }
    }

    void MoveTilesUp(Tile TileToMove, List<Tile> NeighbouringSet)
    {
        foreach (Tile t in TileToMove.Neighbours)
        {
            if (TileToMove.transform.localPosition.x == t.transform.localPosition.x && TileToMove.transform.localPosition.y < t.transform.localPosition.y)
            {
                TileToMove.SwapTile(t);
            }
            else
            {
                hasReachedTop = true;
            }
        } 
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

    void Reshuffle()
    {
        print("Reshuffling tiles!");

        foreach (Tile t in tiles)
        {
            t.CreateTile();
        }
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
            hasMadeTurn = true;
            ResetBoard();
            CurrentTile = null;
            PrevTile = null;
        }
        else
        {
            print("Error 80085: Not a valid neighbour.");
        }
    }

    List<Tile> GetRow(Tile t)
    {
        List<Tile> myList = new List<Tile>();
        RaycastHit2D[] ray = Physics2D.RaycastAll(t.transform.localPosition, Vector2.up, (float)GridHeight);

        for(int i = 0; i < ray.Length; i++)
        {
            myList.Add(ray[i].collider.GetComponentInParent<Tile>());
        }

        //print("Row length is: " + myList.Count);

        return myList;
    }

    List<Tile> GetColumn(Tile t)
    {
        List<Tile> myList = new List<Tile>();
        RaycastHit2D[] ray = Physics2D.RaycastAll(t.transform.localPosition, Vector2.right, (float)GridWidth);

        for (int i = 0; i < ray.Length; i++)
        {
            myList.Add(ray[i].collider.GetComponentInParent<Tile>());
        }

        //print("Column length is: " + myList.Count);

        return myList;
    }

    /*List<Tile> GetTilesAbove(Tile t)
    {
        List<Tile> myList = new List<Tile>();
        RaycastHit2D[] ray = Physics2D.RaycastAll(t.transform.localPosition, Vector2.up, (float)GridHeight);
        int distance;

        for(int i=0; i < ray.Length; i++)
        {
            if(ray[1].collider.GetComponentInParent<Tile>().toBeNulled)
            {
                distance = 1;
            }
            else
            {
                distance = 
            }
        }

    }*/

    void InitDictionary()
    {
        int ColCount = 0;
        int RowCount = 0;

        foreach (Tile t in tiles)
        {
            if (t.isBottomEdge)
            {
                ColCount++;
                BoardRayCasts.Add("col" + ColCount, GetRow(t));
            }
            if (t.isLeftEdge)
            {
                RowCount++;
                BoardRayCasts.Add("row" + RowCount, GetColumn(t));
            }
        }
    }

    void FindMatches(List<Tile> myList)
    {
        //print("finding matches...");
        for(int i = 0; i < myList.Count-2; i++)
        {
            if (myList[i].value == myList[i + 1].value && myList[i].value == myList[i + 2].value)
            {
                print("There is a set of 3 starting from: " + myList[i].transform.localPosition + " Ending at: " + myList[i + 2].transform.localPosition);

                if(hasMadeTurn)
                {
                    MatchingSet.Add(myList[i]);
                    MatchingSet.Add(myList[i + 1]);
                    MatchingSet.Add(myList[i + 2]);
                }

                //If the row is not a consequence of the player making 3 in a row, reshuffle.
                if (!hasMadeTurn)
                {
                    needReshuffle = true;
                }
            }
        }
    }

    void CheckBoard()
    {
        print("Checking rows");

        FindMatches(BoardRayCasts["row1"]);
        FindMatches(BoardRayCasts["row2"]);
        FindMatches(BoardRayCasts["row3"]);
        FindMatches(BoardRayCasts["row4"]);
        FindMatches(BoardRayCasts["row5"]);
        FindMatches(BoardRayCasts["row6"]);

        print("Checking Columns");

        FindMatches(BoardRayCasts["col1"]);
        FindMatches(BoardRayCasts["col2"]);
        FindMatches(BoardRayCasts["col3"]);
        FindMatches(BoardRayCasts["col4"]);
        FindMatches(BoardRayCasts["col5"]);
        FindMatches(BoardRayCasts["col6"]);
    }

    /*
    void FindMatchesRecursive(Tile t, List<Tile> Matches)
    {
        foreach(Tile tn in t.Neighbours)
        {
            //Horizontal
            if(t.value == tn.value && !Matches.Contains(tn) && t.transform.localPosition.y == tn.transform.localPosition.y)
            {
                Matches.Add(t);
                FindMatchesRecursive(tn, Matches);
            }
            //Vertical
            else if(t.value == tn.value && !Matches.Contains(tn) && t.transform.localPosition.x == tn.transform.localPosition.x)
            {
                Matches.Add(t);
                FindMatchesRecursive(tn, Matches);
            }
            else
            {
                return;
            }
        }
        return;
    }*/



    /*
    List<Tile> FindMatches(List<Tile> matches)
    {
        List<Tile> VertMatches = new List<Tile>();
        List<Tile> HoriMatches = new List<Tile>();

        //For each tile on board
        foreach (Tile t in tiles)
        {
            //For each neighbour for each tile
            foreach (Tile tn in t.Neighbours)
            {
                //If the tile's value is = to the neighbours value
                if(t.value == tn.value)
                {
                    // ---------------- Searching horizontally ------------------
                    if (t.transform.localPosition.y == tn.transform.localPosition.y)
                    {
                        foreach (Tile tnn in tn.Neighbours)
                        {
                            //If values are equal, they are on the same axis (i.e. horizontal), and neighbours position is NOT the same as the original tile.
                            if ((tn.value == tnn.value) && (tn.transform.localPosition.y == tnn.transform.localPosition.y) && (tnn.transform.localPosition.x != t.transform.localPosition.x))
                            {
                                if (!HoriMatches.Contains(t))
                                {
                                    HoriMatches.Add(t); //3 in list
                                }
                            }
                        }
                    }

                    // ---------------- Searching Vertically ------------------
                    else if (t.transform.localPosition.x == tn.transform.localPosition.x)
                    {
                        foreach (Tile tnn in tn.Neighbours)
                        {
                            if ((tn.value == tnn.value) && (tn.transform.localPosition.x == tnn.transform.localPosition.x) && (tnn.transform.localPosition.y != t.transform.localPosition.y))
                            {
                                if(!VertMatches.Contains(t))
                                {
                                    VertMatches.Add(t); //3 in list
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach(Tile t in HoriMatches)
        {
            int count = 0;
            count++;
            print("The position of Tile " + count + " is: " + t.transform.localPosition);
        }

        foreach (Tile t in VertMatches)
        {
            int count = 0;
            count++;
            print("The position of Tile "+count+" is: "+t.transform.localPosition);
        }

        print(HoriMatches.Count);
        print(VertMatches.Count);

        //CURRENTLY RETURNING ONLY VERTICALLY MATCHED TILES
        return HoriMatches;
    }
    */


    void ResetBoard()
    {
        foreach (Tile t in tiles)
        {
            if (t.isSelected)
            {
                t.ToggleSelect();
            }
        }
        //hasMadeTurn = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();
    public Cell[,] cellsArray;
    public List<Tile> tiles = new List<Tile>();

    public List<Tile> MatchingSet = new List<Tile>();
    public List<Tile> AboveTiles = new List<Tile>();

    public int GridWidth;
    public int GridHeight;

    public GameObject tilePrefab;
    public GameObject cell;

    public Tile PrevTile;
    public Tile CurrentTile;

    public bool hasMadeTurn = false;
    public bool needReshuffle = false;
    public bool isMatched = false;

    public AudioClip MatchSound;
    public int LastValue = 0; // initial value so its not null

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
        FindCellNeighbours();
        CopiedFunction();
    }

    // Update is called once per frame
    void Update()
    {
        CopyCellNeighbourToTile();

        if (hasMadeTurn)
        {
            //Finds matches.
            CopiedFunction();
            hasMadeTurn = false;
        }

        if (isMatched)
        {
            SoundManager.instance.PlaySingle(MatchSound);
            print("There is a match of:"+MatchingSet.Count+" tiles to resolve.");

            foreach (Cell c in cells)
            {
                if (c.tile.toBeNulled)
                {
                    //AboveTiles.AddRange(c.tile.GetAboveTilesInColumn(cells));
                    //Get the tile's current value, and create a new tile of a different value.
                    int ValueToAvoid = c.tile.value;

                    StatsManager.scoreValue += c.tile.tileScore;
                    //If more than 3 in a row
                    if(MatchingSet.Count > 3)
                    {
                        //Add bonus score
                        StatsManager.scoreValue += 25;
                    }
                    //Create a tile avoiding both values!
                    c.tile.CreateTile(ValueToAvoid, LastValue);
                    //print("Creating new tile with the value of "+c.tile.value+". It is a "+c.tile.GetComponent<SpriteRenderer>().sprite.name+" coloured sprite");
                    LastValue = c.tile.value;
                }
            }

            print("------------------------NEXT TURN ------------------------------------");

            isMatched = false;
            MatchingSet.Clear();
        }

        if (needReshuffle)
        {
            Reshuffle();
            needReshuffle = false;
            CopiedFunction();
        }
    }

    void CreateBoard()
    {
        cellsArray = new Cell[GridHeight, GridWidth];

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                //Create cells, add to list
                GameObject g1 = Instantiate(cell, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                g1.transform.parent = gameObject.transform;

                //Get in list form
                cells.Add(g1.GetComponent<Cell>());
                //Get in 2d array form (for manipulation purposes)
                cellsArray[y,x] = g1.GetComponent<Cell>();
            }
        }
        gameObject.transform.position = new Vector3(-2.5f, -2f, -0.5f);

        //Set each cells xPos and yPos
        foreach(Cell c in cells)
        {
            c.Initialise((int)c.transform.localPosition.x, (int)c.transform.localPosition.y);
        }
    }

    void CopyCellNeighbourToTile()
    {
        foreach (Cell c in cells)
        {
            c.tile.Neighbours.Clear();

            foreach (Cell cn in c.Neighbours)
            {
                if (!c.tile.Neighbours.Contains(cn.tile))
                {
                    c.tile.Neighbours.Add(cn.tile);
                }
            }
        }
    }


    void Reshuffle()
    {
        print("Reshuffling tiles!");

        foreach (Cell c in cells)
        {
            c.tile.CreateTile();
        }

        MatchingSet.Clear();
    }

    void FindCellNeighbours()
    {
        //Loop through all Tiles
        for (int i = 0; i < cells.Count; i++)
        {
            //If it's not a Left edge tile
            if (!cells[i].isLeftEdge)
            {
                //Set its left tile to one element to its left
                cells[i].LeftCell = cells[i - 1];
            }

            //If its not a bottom edge tile
            if (!cells[i].isBottomEdge)
            {
                //Set its bottom tile to the tile below it
                cells[i].BelowCell = cells[i - GridWidth];
            }
        }

        //Loop through all tiles
        foreach (Cell c in cells)
        {
            //If it has a tile on its left
            if (c.LeftCell != null)
            {
                //Add the left tile as a neighbour
                c.AddNeighbour(c.LeftCell);
                //Add this tile as the left tile's neighbour
                c.LeftCell.AddNeighbour(c);
            }

            //If it has a tile below it
            if (c.BelowCell != null)
            {
                //Add the tile below as a neighbour
                c.AddNeighbour(c.BelowCell);
                //Add this tile as the below tile's neighbour
                c.BelowCell.AddNeighbour(c);
            }
        }

        //CopyCellNeighbourToTile();
    }

    public void TileSelection(Tile CurrentTile)
    {
        if (PrevTile == null)
        {
            PrevTile = CurrentTile;
            CurrentTile.ToggleSelect();
        }
        else if (CurrentTile == PrevTile)
        {
            PrevTile = null;
            CurrentTile.ToggleSelect();
        }
        //If Second clicked tile is neighbour with first clicked tile
        else if (PrevTile.IsNeighbour(CurrentTile))
        {
            CurrentTile.ToggleSelect();
            PrevTile.SwapTile(CurrentTile);
            StatsManager.numTurns++;

            //Reset neighbours after swapping. IN UPDATE, IT SETS TILES NEIGHBOURS AGAIN.
            foreach (Cell c in cells)
            {
                c.tile.Neighbours.Clear();
            }

            //Find selected tiles and deselect them
            hasMadeTurn = true;
            ResetBoard();
            CurrentTile = null;
            PrevTile = null;
        }
        else
        {
            print("Error: Not a valid neighbour.");
        }
    }


    /* MUST CHANGE THE NAME OF THIS FUNCTION AND LOOK AT AGAIN CAREFULLY ---------------------------------------------------------------------*/
    void CopiedFunction()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 2; x < GridWidth; x++)
            {
                if (cellsArray[x,y].tile.value == cellsArray[x-1,y].tile.value && cellsArray[x,y].tile.value == cellsArray[x-2,y].tile.value)
                {
                    // Found 3 in a row!
                    // Clear any extra tiles in the row that match
                    for (int i = x + 1; i < GridWidth; i++)
                    {
                        if (cellsArray[i,y].tile.value == cellsArray[x,y].tile.value)
                        {
                            cellsArray[i, y].tile.toBeNulled = true;

                            if (!MatchingSet.Contains(cellsArray[i, y].tile))
                            {
                                MatchingSet.Add(cellsArray[i, y].tile);
                            }
                        }
                        else
                        {
                            // stop checking if it did not match
                            break;
                        } // if-else
                    } // for i
                      // Clear the 3 . . .
                    if (hasMadeTurn)
                    {
                        cellsArray[x, y].tile.toBeNulled = true;
                        cellsArray[x - 1, y].tile.toBeNulled = true;
                        cellsArray[x - 2, y].tile.toBeNulled = true;

                        if (!MatchingSet.Contains(cellsArray[x, y].tile))
                        {
                            MatchingSet.Add(cellsArray[x, y].tile);
                        }

                        if (!MatchingSet.Contains(cellsArray[x - 1, y].tile))
                        {
                            MatchingSet.Add(cellsArray[x - 1, y].tile);
                        }

                        if (!MatchingSet.Contains(cellsArray[x - 2, y].tile))
                        {
                            MatchingSet.Add(cellsArray[x - 2, y].tile);
                        }
                    }

                    if(!hasMadeTurn)
                    {
                        needReshuffle = true;
                    }
                } // if match-3
            } // for x
        } // for y

        // Check for vertical 3-in-a-row
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 2; y < GridHeight; y++)
            {
                if (cellsArray[x,y].tile.value == cellsArray[x,y - 1].tile.value &&cellsArray[x,y].tile.value == cellsArray[x,y - 2].tile.value)
                {
                    // Found 3 in a row!
                    // Clear any extra tiles in the column that match
                    for (int i = y + 1; i < GridHeight; i++)
                    {
                        if (cellsArray[x,i].tile.value == cellsArray[x,y].tile.value)
                        {
                            cellsArray[x, i].tile.toBeNulled = true;

                            if (!MatchingSet.Contains(cellsArray[x, i].tile))
                            {
                                MatchingSet.Add(cellsArray[x, i].tile);
                            }
                        }
                        else
                        {
                            // stop checking if it did not match
                            break;
                        } // if-else
                    } // for i

                    cellsArray[x, y].tile.toBeNulled = true;
                    cellsArray[x, y-1].tile.toBeNulled = true;
                    cellsArray[x, y-2].tile.toBeNulled = true;

                    if (!MatchingSet.Contains(cellsArray[x, y].tile))
                    {
                        MatchingSet.Add(cellsArray[x, y].tile);
                    }

                    if (!MatchingSet.Contains(cellsArray[x, y - 1].tile))
                    {
                        MatchingSet.Add(cellsArray[x, y - 1].tile);
                    }

                    if (!MatchingSet.Contains(cellsArray[x, y - 2].tile))
                    {
                        MatchingSet.Add(cellsArray[x, y - 2].tile);
                    }

                    if (!hasMadeTurn)
                    {
                        needReshuffle = true;
                    }

                } // if match-3
            } // for y
        } // for x

        if(MatchingSet.Count > 0 && hasMadeTurn)
        {
            isMatched = true;
        }
    }

    void ResetBoard()
    {
        foreach (Cell c in cells)
        {
            if (c.tile.isSelected)
            {
                c.tile.ToggleSelect();
            }
        }
    }
}

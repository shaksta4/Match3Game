using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Cell> cells = new List<Cell>();
    public Cell[,] cellsArray;
    public List<Tile> tiles = new List<Tile>();

    public List<Tile> ListOfPossible = new List<Tile>();
    public List<Tile> MatchingSet = new List<Tile>();
    public List<Tile> AboveTiles = new List<Tile>();

    public int GridWidth;
    public int GridHeight;

    public GameObject tilePrefab;
    public GameObject cell;

    public Tile PrevTile;
    public Tile PrevSelectedTile;
    public Tile CurrentSelectedTile;

    public bool hasMadeTurn = false;
    public bool needReshuffle = false;
    public bool isMatched = false;
    public bool invalidMove = false;
    public bool hasSearched = false;

    public AudioClip MatchSound;
    public int LastValue = 0; // initial value so its not null


    void Awake()
    {
        CreateBoard();
        FindCellNeighbours();
        MatchingSet = GetMatches();
        print("Initially, this is how many matches found:" + MatchingSet.Count);
    }

    // Update is called once per frame
    void Update()
    {
        CopyCellNeighbourToTile();

        if (MatchingSet.Count > 0)
        {
            if (hasMadeTurn)
            {
                isMatched = true;
            }
            if (!hasMadeTurn)
            {
                print("Matching set has got " + MatchingSet.Count + " ... Also you didn't make a turn, so im reshuffling!");
                needReshuffle = true;
            }
        }

        if (!needReshuffle && !hasSearched)
        {
            GetPossibleMatches();
            hasSearched = true;
        }

        if (hasMadeTurn)
        {
            //Finds matches.
            MatchingSet = GetMatches();

            if(MatchingSet.Count < 3)
            {
                invalidMove = true;
            }
        }

        if(invalidMove)
        {
            StatsManager.numTurns--;
            CurrentSelectedTile.SwapTile(PrevSelectedTile);
            print("Illegal move, sorry");
            ResetBoard();
            hasMadeTurn = false;
            invalidMove = false;
        }

        if (isMatched)
        {
            //SoundManager.instance.PlaySingle(MatchSound);
            print("There is a match of:" + MatchingSet.Count + " tiles to resolve.");

            foreach (Cell c in cells)
            {
                if (c.tile.toBeNulled)
                {
                    //AboveTiles.AddRange(c.tile.GetAboveTilesInColumn(cells));
                    //Get the tile's current value, and create a new tile of a different value.
                    int ValueToAvoid = c.tile.value;

                    StatsManager.scoreValue += c.tile.tileScore;
                    //If more than 3 in a row
                    if (MatchingSet.Count > 3)
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

            MatchingSet.Clear();
            ListOfPossible.Clear();
            hasSearched = false;
            hasMadeTurn = false;
            isMatched = false;
        }

        if (needReshuffle)
        {
            Reshuffle();
            MatchingSet = GetMatches();
            needReshuffle = false;
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
                cellsArray[y, x] = g1.GetComponent<Cell>();
            }
        }
        gameObject.transform.position = new Vector3(-2.5f, -2f, -0.5f);

        //Set each cells xPos and yPos
        foreach (Cell c in cells)
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
    }

    public void TileSelection(Tile CurrentTile)
    {
        CurrentSelectedTile = CurrentTile;
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
            print("Swapping tiles");
            PrevTile.SwapTile(CurrentTile);

          /*  
            if (myList.Count < 3)
            {
                CurrentTile.SwapTile(PrevTile);
                print("Illegal move, sorry");
                ResetBoard();
                CurrentTile = null;
                PrevTile = null;
                return;
            }
            */

            //print("Invalid move was made before");
            StatsManager.numTurns++;

            //Reset neighbours after swapping. IN UPDATE, IT SETS TILES NEIGHBOURS AGAIN.
            foreach (Cell c in cells)
            {
                c.tile.Neighbours.Clear();
            }

            //Find selected tiles and deselect them
            hasMadeTurn = true;
            ResetBoard();
            PrevSelectedTile = PrevTile;
            CurrentTile = null;
            PrevTile = null;
        }
        else
        {
            print("Error: Not a valid neighbour.");
        }
    }

    /* MUST CHANGE THE NAME OF THIS FUNCTION AND LOOK AT AGAIN CAREFULLY ---------------------------------------------------------------------*/
    List<Tile> GetMatches()
    {
        List<Tile> VerticalList = GetVerticalMatches();
        List<Tile> HorizontalList = GetHorizontalMatches();
        List<Tile> CombinedList = new List<Tile>();

        foreach (Tile t in VerticalList)
        {
            if(!CombinedList.Contains(t))
            {
                CombinedList.Add(t);
            }
        }
        foreach (Tile t in HorizontalList)
        {
            if (!CombinedList.Contains(t))
            {
                CombinedList.Add(t);
            }
        }

        foreach(Tile t in CombinedList)
        {
            t.toBeNulled = true;
        }

        return CombinedList;
    }

    List<Tile> GetVerticalMatches()
    {
        List<Tile> MatchingList = new List<Tile>();
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 2; y < GridHeight; y++)
            {
                if (cellsArray[x, y].tile.value == cellsArray[x, y - 1].tile.value && cellsArray[x, y].tile.value == cellsArray[x, y - 2].tile.value)
                {
                    // Found 3 in a row!
                    for (int i = y + 1; i < GridHeight; i++)
                    {
                        if (cellsArray[x, i].tile.value == cellsArray[x, y].tile.value)
                        {
                            if (!MatchingList.Contains(cellsArray[x, i].tile))
                            {
                                MatchingList.Add(cellsArray[x, i].tile);
                            }
                        }
                        else
                        {
                            // stop checking if it did not match
                            break;
                        } // if-else
                    } // for i

                    if (!MatchingList.Contains(cellsArray[x, y].tile))
                    {
                        MatchingList.Add(cellsArray[x, y].tile);
                    }

                    if (!MatchingList.Contains(cellsArray[x, y - 1].tile))
                    {
                        MatchingList.Add(cellsArray[x, y - 1].tile);
                    }

                    if (!MatchingList.Contains(cellsArray[x, y - 2].tile))
                    {
                        MatchingList.Add(cellsArray[x, y - 2].tile);
                    }
                } // if match-3
            } // for y
        } // for x

        return MatchingList;
    }

    List<Tile> GetHorizontalMatches()
    {
        List<Tile> MatchingList = new List<Tile>();

        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 2; x < GridWidth; x++)
            {
                if (cellsArray[x, y].tile.value == cellsArray[x - 1, y].tile.value && cellsArray[x, y].tile.value == cellsArray[x - 2, y].tile.value)
                {
                    // Found 3 in a row!
                    // Clear any extra tiles in the row that match
                    for (int i = x + 1; i < GridWidth; i++)
                    {
                        if (cellsArray[i, y].tile.value == cellsArray[x, y].tile.value)
                        {
                            if (!MatchingList.Contains(cellsArray[i, y].tile))
                            {
                                MatchingList.Add(cellsArray[i, y].tile);
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
                        if (!MatchingList.Contains(cellsArray[x, y].tile))
                        {
                            MatchingList.Add(cellsArray[x, y].tile);
                        }

                        if (!MatchingList.Contains(cellsArray[x - 1, y].tile))
                        {
                            MatchingList.Add(cellsArray[x - 1, y].tile);
                        }

                        if (!MatchingList.Contains(cellsArray[x - 2, y].tile))
                        {
                            MatchingList.Add(cellsArray[x - 2, y].tile);
                        }
                    }
                } // if match-3
            } // for x
        } // for y

        return MatchingList;
    }

    //Yet to add 5 and 6.
    void GetPossibleMatches()
    {
        foreach(Cell c in cells)
        {
            foreach(Cell cn in c.Neighbours)
            {
                if(c.tile.value == cn.tile.value)
                {
                    foreach(Cell cnn in cn.Neighbours)
                    {
                        //If neighbours neighbour is on right
                        if(c.transform.position.y == cnn.transform.position.y && c.transform.position.x < cnn.transform.position.x && cnn.transform.position != c.transform.position && cnn.transform.position != cn.transform.position)
                        {
                            foreach(Cell cnnn in cnn.Neighbours)
                            {
                                if(cnnn.tile.value == c.tile.value && cnnn.transform.position != cn.transform.position)
                                {
                                   // print("The root for this match is " + c.transform.localPosition+" with a value of "+c.tile.value+" and its neighbour chosen was "+cn.transform.localPosition+". My good neighbour is "+cnn.transform.localPosition
                                   //    +" and my position is "+cnnn.transform.localPosition);
                                    if (!ListOfPossible.Contains(cnnn.tile))
                                    {
                                        ListOfPossible.Add(cnnn.tile);
                                    }
                                    //add to list
                                }
                            }
                        }
                        //If neighbours neighbour is on left
                        else if (c.transform.position.y == cnn.transform.position.y && c.transform.position.x > cnn.transform.position.x && cnn.transform.position != c.transform.position && cnn.transform.position != cn.transform.position)
                        {
                            foreach (Cell cnnn in cnn.Neighbours)
                            {
                                if (cnnn.tile.value == c.tile.value && cnnn.transform.position != cn.transform.position)
                                {
                                    //add to list
                                    if (!ListOfPossible.Contains(cnnn.tile))
                                    {
                                        ListOfPossible.Add(cnnn.tile);
                                    }
                                }
                            }
                        }
                        //above
                        else if (c.transform.position.y < cnn.transform.position.y && c.transform.position.x == cnn.transform.position.x && cnn.transform.position != c.transform.position && cnn.transform.position != cn.transform.position)
                        {
                            foreach (Cell cnnn in cnn.Neighbours)
                            {
                                if (cnnn.tile.value == c.tile.value && cnnn.transform.position != cn.transform.position)
                                {
                                    //add to list
                                    if (!ListOfPossible.Contains(cnnn.tile))
                                    {
                                        ListOfPossible.Add(cnnn.tile);
                                    }
                                }
                            }
                        }
                        //below
                        else if (c.transform.position.y > cnn.transform.position.y && c.transform.position.x == cnn.transform.position.x && cnn.transform.position != c.transform.position && cnn.transform.position != cn.transform.position)
                        {
                            foreach (Cell cnnn in cnn.Neighbours)
                            {
                                if (cnnn.tile.value == c.tile.value && cnnn.transform.position != cn.transform.position)
                                {
                                    //add to list
                                    if (!ListOfPossible.Contains(cnnn.tile))
                                    {
                                        ListOfPossible.Add(cnnn.tile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

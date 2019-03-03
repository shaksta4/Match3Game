using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //public variables.
    public StatsManager sm;

    public int GridWidth, GridHeight;

    public GameObject tilePrefab, cell; 
    public AudioClip MatchSound;

    //private variables
    private Cell[,] cellsArray;
    private List<Cell> cells = new List<Cell>();
    private List<Tile> ListOfPossible = new List<Tile>(); // Holds list tiles possible to make matches with
    private List<Tile> MatchingSet = new List<Tile>(); // Holds list of all sets that match 3 or more in a row.

    private Tile PrevTile; // Used in swapping tiles (TileSelection)
    private Tile PrevSelectedTile, CurrentSelectedTile; // Used in stopping illegal moves (Update).

    private bool hasMadeTurn = false;
    private bool needReshuffle = false;
    private bool isMatched = false;
    private bool invalidMove = false;
    private bool hasSearched = false; // Bool for checking if board has been searched for possible moves. (Update)
    private bool gameEnd = false;

    private int LastValue = 0; // Holds the value of the last created tile. (Update)
    private int HintTileIndex; // Holds the index of the tile that will animate as a hint. (Update)

    //Timers
    private float HintTimer = 5f; // How long before a hint is shown.
    private float CountdownTimer = 15f; // How long before game ends.
    private float GameTimer = 0f; // Stopwatch for game
    private float timerincrease = 1f; // Amount added to countdown per match made.

    /*Create board, find neighbours of each cell, copy neighbours to each tile, and look for matches. */
    void Awake()
    {
        CreateBoard();
        FindCellNeighbours();
        CopyCellNeighbourToTile();
        MatchingSet = GetMatches();
    }

    void Update()
    {
        //If game has not ended
        if (!gameEnd)
        {
            GameTimer += Time.deltaTime;
            CountdownTimer -= Time.deltaTime;
            HintTimer -= Time.deltaTime;

            sm.gameTimer = GameTimer;
            sm.timerValue = CountdownTimer;

            CopyCellNeighbourToTile();
        }
        
        //If timer runs out and user was playing.
        if (CountdownTimer <= 0 && GameController.instance.Playing)
        {
            sm.ReasonOfLoss = "you ran out of time";
            gameEnd = true;
        }

        //If game ends
        if(gameEnd)
        {
            GameController.instance.End = true;
            GameController.instance.Playing = false;
            Reset(); // Reset timers and bools.
        }

        //If a there are any matches. 
        if (MatchingSet.Count > 0)
        {
            //If match is result of player
            if (hasMadeTurn)
            {
                foreach (Tile t in MatchingSet)
                {
                    t.toBeNulled = true;
                }
                isMatched = true;
            }
            //If match was present on board creation, flag for reshuffling.
            if (!hasMadeTurn)
            {
                print("Matching set has got " + MatchingSet.Count + " ... Also you didn't make a turn, so im reshuffling!");
                needReshuffle = true;
            }
        }

        //If board hasn't been searched for possible moves, and board doesn't need reshuffle
        if (!needReshuffle && !hasSearched)
        {
            GetPossibleMatches();
            GetPossibleMatches2();
            //Sent the hint tile to be a random tile in the list.
            HintTileIndex = Random.Range(0, ListOfPossible.Count);

            hasSearched = true;
        }

        //If board has been searched
        if (hasSearched)
        {
            //If no more possible moves to make
            if (ListOfPossible.Count == 0)
            {
                sm.ReasonOfLoss = "there were no more possible moves";
                gameEnd = true;
            }
            //Else play a hint once the hint timer is 0 and a hint isn't currently playing.
            else
            {
                if (HintTimer <= 0 && !ListOfPossible[HintTileIndex].GetComponent<ParticleSystem>().isPlaying)
                {
                    ListOfPossible[HintTileIndex].GetComponent<ParticleSystem>().Play();
                }
            }
        }

        //If player makes a turn.
        if (hasMadeTurn)
        {
            //Meaning if the hint was triggered, stop it.
            if (ListOfPossible.Count > 0)
            {
                if (ListOfPossible[HintTileIndex].GetComponent<ParticleSystem>().isPlaying)
                {
                    print("Turning off the hint");
                    ListOfPossible[HintTileIndex].GetComponent<ParticleSystem>().Stop();
                }
            }
            //Finds matches.
            MatchingSet = GetMatches();

            //If match was not made
            if(MatchingSet.Count < 3)
            {
                GameController.instance.InvalidMove = true;
                invalidMove = true;
            }
        }

        //If player made invalid move
        if(invalidMove)
        {
            sm.numTurns--;
            CurrentSelectedTile.SwapTile(PrevSelectedTile); // Swap back tiles to original places
            print("Illegal move, sorry");
            ResetSelected();
            hasMadeTurn = false;
            invalidMove = false;
        }

        //If there is any player made matches
        if (isMatched)
        {
            SoundManager.instance.PlaySingle(MatchSound);
            //Reset hint timer.
            HintTimer = 10.0f;
            print("There is a match of:" + MatchingSet.Count + " tiles to resolve.");

            foreach (Cell c in cells)
            {
                if (c.tile.toBeNulled)
                {
                    //Get the tile's current value, and create a new tile of a different value.
                    int ValueToAvoid = c.tile.value;

                    sm.scoreValue += c.tile.TileScore;
                    //If more than 3 in a row
                    if (MatchingSet.Count > 3)
                    {
                        //Add bonus score
                        sm.scoreValue += 10;
                    }
                    //Create a tile avoiding match's current values, and previously created values!.
                    c.tile.CreateTile(ValueToAvoid, LastValue);
                    print("Creating new tile with the value of "+c.tile.value+". It is a "+c.tile.GetComponent<SpriteRenderer>().sprite.name+" coloured sprite");
                    LastValue = c.tile.value;
                    CountdownTimer += timerincrease;
                }
            }

            print("------------------------NEXT TURN ------------------------------------");

            MatchingSet.Clear();
            ListOfPossible.Clear();
            hasSearched = false;
            hasMadeTurn = false;
            isMatched = false;
        }
        // If reshuffling required
        if (needReshuffle)
        {
            Reshuffle();
            CopyCellNeighbourToTile();
            MatchingSet = GetMatches();
            needReshuffle = false;
        }
    }

    /* This function instantiates a cell object in a grid form. It stores the created objects in a list and 2d Array and sets their position */
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

    /* This function loops through each cell, clears their tile's neighbours and then sets their new neighbours based on the cell's fixed neighbours */
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

    /*This function loops through each cell, and creates a new tile in it. It then clears any matches previously stored in MatchingSet*/
    void Reshuffle()
    {
        print("Reshuffling tiles!");
        foreach (Cell c in cells)
        {
            c.tile.CreateTile();
        }
        MatchingSet.Clear();
    }

    /*This function sets each cells neighbours based on whether it's a bottom edge, or a left edge*/
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

    /*This function takes in a tile parameter. It compared the tile that is clicked first, 
     * with the tile that is clicked second, and swaps their places if its a valid move.
     * It is called in Tile.cs's OnMouseDown method*/
    public void TileSelection(Tile CurrentTile)
    {
        CurrentSelectedTile = CurrentTile;
        //If its the first tile selected
        if (PrevTile == null)
        {
            PrevTile = CurrentTile;
            CurrentTile.ToggleSelect();
        }
        //If the first tile selected is clicked again
        else if (CurrentTile == PrevTile)
        {
            PrevTile = null;
            CurrentTile.ToggleSelect();
        }
        //If the second tile selected is not a neighbour, deselect the first tile and set the second tile as the new first tile.
        else if(!PrevTile.IsNeighbour(CurrentTile))
        {
            PrevTile.ToggleSelect();
            PrevTile = CurrentTile;
            CurrentTile.ToggleSelect();
        }
        //If Second clicked tile is neighbour with first clicked tile
        else if (PrevTile.IsNeighbour(CurrentTile))
        {
            CurrentTile.ToggleSelect();
            print("Swapping tiles");

            //Swap tiles
            PrevTile.SwapTile(CurrentTile);

            sm.numTurns++;

            //Reset neighbours after swapping.
            foreach (Cell c in cells)
            {
                c.tile.Neighbours.Clear();
            }

            //Find selected tiles and deselect them
            hasMadeTurn = true;
            ResetSelected();
            PrevSelectedTile = PrevTile;
            CurrentTile = null;
            PrevTile = null;
        }
    }

    /*This function combines the Lists from GetVerticalMatches and GetHorizontalMatches into a single list and returns it*/
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
        return CombinedList;
    }

    /*This function searches through the board, and checks vertically for any matches of 3 or more. It then returns them in a list*/
    List<Tile> GetVerticalMatches()
    {
        List<Tile> myList = new List<Tile>();
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 2; y < GridHeight; y++)
            {
                //Check previous two tiles
                if (cellsArray[x, y].tile.value == cellsArray[x, y - 1].tile.value && cellsArray[x, y].tile.value == cellsArray[x, y - 2].tile.value)
                {
                    // Found 3 in a row
                    for (int i = y + 1; i < GridHeight; i++)
                    {
                        //Keep checking for more tiles in a row.
                        if (cellsArray[x, i].tile.value == cellsArray[x, y].tile.value)
                        {
                            if (!myList.Contains(cellsArray[x, i].tile))
                            {
                                myList.Add(cellsArray[x, i].tile);
                            }
                        }
                        else
                        {
                            // stop checking if it did not match
                            break;
                        } 
                    } 
                    // Uniquely add to list.
                    if (!myList.Contains(cellsArray[x, y].tile))
                    {
                        myList.Add(cellsArray[x, y].tile);
                    }
                    if (!myList.Contains(cellsArray[x, y - 1].tile))
                    {
                        myList.Add(cellsArray[x, y - 1].tile);
                    }
                    if (!myList.Contains(cellsArray[x, y - 2].tile))
                    {
                        myList.Add(cellsArray[x, y - 2].tile);
                    }
                } 
            } 
        } 
        return myList;
    }

    /*This function does the same thing as GetVerticalMatches, but for rows horizontal. It returns a list of matched tiles.*/
    List<Tile> GetHorizontalMatches()
    {
        List<Tile> myList = new List<Tile>();
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 2; x < GridWidth; x++)
            {
                //Check previous two tiles
                if (cellsArray[x, y].tile.value == cellsArray[x - 1, y].tile.value && cellsArray[x, y].tile.value == cellsArray[x - 2, y].tile.value)
                {
                    // Found 3 in a row
                    for (int i = x + 1; i < GridWidth; i++)
                    {
                        //Keep checking for more tiles in a row.
                        if (cellsArray[i, y].tile.value == cellsArray[x, y].tile.value)
                        {
                            if (!myList.Contains(cellsArray[i, y].tile))
                            {
                                myList.Add(cellsArray[i, y].tile);
                            }
                        }
                        else
                        {
                            // stop checking if it did not match
                            break;
                        }
                    }
                    //Uniquely add to list.
                    if (!myList.Contains(cellsArray[x, y].tile))
                    {
                        myList.Add(cellsArray[x, y].tile);
                    }

                    if (!myList.Contains(cellsArray[x - 1, y].tile))
                    {
                        myList.Add(cellsArray[x - 1, y].tile);
                    }

                    if (!myList.Contains(cellsArray[x - 2, y].tile))
                    {
                        myList.Add(cellsArray[x - 2, y].tile);
                    }
                }
            }
        }
        return myList;
    }

    /*This function searches through the board, and looks for any places where it is possible to make a 3 in a row match.
     Note it does not find XOX matches. It stores possible 3 match tiles in a list ListOfPossible
                           OXO */
    void GetPossibleMatches()
    {
        foreach(Cell c in cells)
        {
            //Search each cell's neighbour
            foreach(Cell cn in c.Neighbours)
            {
                //If neighbour is equal to me
                if(c.tile.value == cn.tile.value)
                {
                    //Search neighbours neighbours
                    foreach(Cell cnn in cn.Neighbours)
                    {
                        //If neighbours neighbour is on right
                        if(c.transform.position.y == cnn.transform.position.y && c.transform.position.x < cnn.transform.position.x && cnn.transform.position != c.transform.position && cnn.transform.position != cn.transform.position)
                        {
                            //Search neighbours neighbours neighbour
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

    /*This is an extension of GetPossibleMatches. It searches for any possible match not covered
     in GetPossibleMatches. It also stores possible 3 matches in a list ListOfPossible*/
    void GetPossibleMatches2()
    {
        int CurrentTileValue;
        for(int y = 0; y < GridHeight-2; y++)
        {
            for(int x = 0; x < GridWidth-2; x++)
            {
                CurrentTileValue = cellsArray[y, x].tile.value;
                //Where X is starting tile, x is other matches, O are other tiles.
                //Horizontal
                if(cellsArray[y,x+1].tile.value != CurrentTileValue)
                {
                    //If on left edge of board
                    if(y == 0)
                    {
                        if(cellsArray[y, x+2].tile.value == CurrentTileValue)
                        {
                            if (cellsArray[y + 1, x + 1].tile.value == CurrentTileValue)
                            {
                                if (!ListOfPossible.Contains(cellsArray[y + 1, x + 1].tile))
                                {
                                    ListOfPossible.Add(cellsArray[y + 1, x + 1].tile);
                                    break;
                                }
                            }
                        }
                    }
                    //OxO
                    //XOx
                    //OxO
                    else if (cellsArray[y, x + 2].tile.value == CurrentTileValue)
                    {
                        if (cellsArray[y + 1, x + 1].tile.value == CurrentTileValue)
                        {
                            if (!ListOfPossible.Contains(cellsArray[y + 1, x + 1].tile))
                            {
                                ListOfPossible.Add(cellsArray[y + 1, x + 1].tile);
                            }
                        }
                        else if (cellsArray[y - 1, x + 1].tile.value == CurrentTileValue)
                        {
                            if (!ListOfPossible.Contains(cellsArray[y - 1, x + 1].tile))
                            {
                                ListOfPossible.Add(cellsArray[y - 1, x + 1].tile);
                            }
                        }
                    }
                }
                //Vertical
                if(cellsArray[y+1, x].tile.value != CurrentTileValue)
                {
                    //If on bottom edge of board
                    if(x == 0)
                    {
                        if(cellsArray[y+2, x].tile.value == CurrentTileValue)
                        {
                            if (cellsArray[y + 1, x + 1].tile.value == CurrentTileValue)
                            {
                                if (!ListOfPossible.Contains(cellsArray[y + 1, x + 1].tile))
                                {
                                    ListOfPossible.Add(cellsArray[y + 1, x + 1].tile);
                                    break;
                                }
                            }
                        }
                    }

                    // OxO
                    // xOx
                    // OXO
                    else if (cellsArray[y + 2, x].tile.value == CurrentTileValue)
                    {
                        if (cellsArray[y + 1, x + 1].tile.value == CurrentTileValue)
                        {
                            if (!ListOfPossible.Contains(cellsArray[y + 1, x + 1].tile))
                            {
                                ListOfPossible.Add(cellsArray[y + 1, x + 1].tile);
                            }
                        }
                        else if (cellsArray[y + 1, x - 1].tile.value == CurrentTileValue)
                        {
                            if (!ListOfPossible.Contains(cellsArray[y + 1, x - 1].tile))
                            {
                                ListOfPossible.Add(cellsArray[y + 1, x - 1].tile);
                            }
                        }
                    }
                }
            }
        }
    }

    /*This function resets booleans and any appropriate timers. It is called on game end*/
    private void Reset()
    {
        CountdownTimer = 15f;
        gameEnd = false;
        GameTimer = 0f;
        hasMadeTurn = false;
        hasSearched = false;
        invalidMove = false;
        isMatched = false;
        needReshuffle = false;
    }

    /*This function deselected all tiles on the board*/
    void ResetSelected()
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //variables
    public List<Tile> Neighbours = new List<Tile>();
    public Sprite[] MySpriteImages = new Sprite[8];
    public Sprite HighlightSprite;

    public bool isSelected, toBeNulled;
    public int TileScore = 5;
    public int value;

    private GameObject Highlighter;
    private SpriteRenderer SpriteRenderVar;


    void Awake()
    {
        CreateTile();
    }

    void Update()
    {
        //If this tile is selected
        if (isSelected)
        {
            //If it doesn't have a highlighter, create one and set it at its position.
            if (Highlighter == null)
            {
                Highlighter = new GameObject();
                Highlighter.AddComponent<SpriteRenderer>();
                Highlighter.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
                Highlighter.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);// Color.gray;
                Highlighter.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
            }
        }
        //If not selected
        else
        {
            //If it has a highlighter, destroy it.
            if(Highlighter != null)
            {
                Destroy(Highlighter);
                Highlighter = null;
            }
        }
    }

    /*This function is used to set a tile's sprite and value. It uses RNG to determine which sprite and value it has*/
    public void CreateTile() 
    {
        int RandomTileSelector = Random.Range(0, MySpriteImages.Length);

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = MySpriteImages[RandomTileSelector];

        value = RandomTileSelector;

        toBeNulled = false;
    }

    /*This function is an overload of CreateTile. It takes in two integers which are values to avoid. If the RNG 
     lands on either number, it increments and sets the next sprite and value instead.*/
    public void CreateTile(int CurrentValue, int PreviousValue)
    {
        int RandomTileSelector = Random.Range(0, MySpriteImages.Length-1);

        //If its gonna make the same colour tile as what was previous there OR if its going to create the same colour tile as the previously created one.
        if(CurrentValue == RandomTileSelector || PreviousValue == RandomTileSelector)
        {
            RandomTileSelector++;
        }

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = MySpriteImages[RandomTileSelector];

        value = RandomTileSelector;

        toBeNulled = false;
    }

    public void AddNeighbour(Tile t)
    {
        this.Neighbours.Add(t);
    }

    public bool IsNeighbour(Tile t)
    {
        if(Neighbours.Contains(t))
        {
            return true;
        }

        return false;
    }

    /*This function takes in a tile, it swaps this tile's position and parents with, the parameter tile*/
    public void SwapTile(Tile CurrentTile)
    {
        Vector3 PositionBuffer;
        Transform ParentBuffer;
        Cell PrevCell;
        Cell CurrentCell;
        Tile buffer;

        //print("Moving this tile from " + this.GetComponentInParent<Cell>().transform.localPosition + " to " + CurrentTile.GetComponentInParent<Cell>().transform.localPosition);
        //Swap positions
        PositionBuffer = this.transform.position;
        this.transform.SetPositionAndRotation(CurrentTile.transform.position, Quaternion.identity);
        CurrentTile.transform.SetPositionAndRotation(PositionBuffer, Quaternion.identity);

        //Swap parents (position wise)
        ParentBuffer = this.transform.parent;
        this.transform.SetParent(CurrentTile.transform.parent);
        CurrentTile.transform.SetParent(ParentBuffer);

        //Swap actual parent cells (Script wise)
        PrevCell = this.GetComponentInParent<Cell>();
        CurrentCell = CurrentTile.GetComponentInParent<Cell>();
        buffer = PrevCell.tile;
        PrevCell.tile = CurrentCell.tile;
        CurrentCell.tile = buffer;
    }

    public void ToggleSelect()
    {
        isSelected = !isSelected;
    }

    /*When clicked, if game is not paused, call TileSelection from the BoardManager*/
    void OnMouseDown()
    {
        if (!GameController.instance.Paused)
        {
            GameObject.Find("BoardManager").GetComponent<BoardManager>().TileSelection(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> Neighbours = new List<Tile>();
    public Sprite[] MySpriteImages = new Sprite[8];

    public Sprite HighlightSprite;

    GameObject highlighter;

    public bool isSelected;
    public bool toBeNulled;
    public bool canFall;

    public int tileScore = 15;

    private SpriteRenderer SpriteRenderVar;

    public int value;

    void Awake()
    {
        CreateTile();
    }

    // MIGHT NEED TO USE TO CREATE NEW TILES TO ADD INTO NULLED PLACS
   /* void Start()
    {
        CreateTile();
    }
    */

    // Update is called once per frame [TILE]
    void Update()
    {
        if (isSelected)
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            if (highlighter == null)
            {
                highlighter = new GameObject();
                highlighter.AddComponent<SpriteRenderer>();
                highlighter.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
                highlighter.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);// Color.gray;
                highlighter.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
            }
        }
        else
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            if(highlighter != null)
            {
                Destroy(highlighter);
                highlighter = null;
            }
        }
    }

    //Used to create or Recreate tiles [TILE] 
    public void CreateTile() 
    {
        int RandomTileSelector = Random.Range(0, MySpriteImages.Length);

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = MySpriteImages[RandomTileSelector];

        value = RandomTileSelector;

        toBeNulled = false;
    }

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

    public List<Tile> GetAboveTilesInColumn(List<Cell> cells)
    {
        List<Tile> myList = new List<Tile>();

        foreach(Cell c in cells)
        {
            if (c.tile.transform.position.x == this.transform.position.x && c.tile.transform.position.y > this.transform.position.y)
            {
                if(!c.tile.toBeNulled)
                {
                    myList.Add(c.tile);
                }
            }
        }

        return myList;
    }


    //tn.transform.position.x == t.transform.position.x && tn.transform.position.y > t.transform.position.y

    public void CheckCanFall()
    {
        foreach(Tile t in Neighbours)
        {
            if(t.toBeNulled)
            {
                if(t.transform.position.x == this.transform.position.x)
                {
                    if(t.transform.position.y < this.transform.position.y)
                    {
                        canFall = true;
                    }
                    else
                    {
                        canFall = false;
                    }
                }
            }
        }
    }

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

    void OnMouseDown()
    {
        GameObject.Find("BoardManager").GetComponent<BoardManager>().TileSelection(this);
    }
}

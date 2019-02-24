using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> Neighbours = new List<Tile>();
    public Sprite[] MySpriteImages = new Sprite[8];

    public Tile LeftTile;
    public Tile BelowTile;
    public Tile PrevTile;

    public bool isLeftEdge;
    public bool isBottomEdge;
    public bool isSelected;
    public bool toBeNulled;

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

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //Used to create or Recreate tiles
    public void CreateTile()
    {
        int RandomTileSelector = Random.Range(0, MySpriteImages.Length);

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = MySpriteImages[RandomTileSelector];

        value = RandomTileSelector;
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

    public Tile SwapTile(Tile t)
    {
        //Swap positions of both sprites. It is done this way to maintain the correct neighbours.
        Sprite SpriteBuffer = this.GetComponent<SpriteRenderer>().sprite;
        this.GetComponent<SpriteRenderer>().sprite = t.GetComponent<SpriteRenderer>().sprite;
        t.GetComponent<SpriteRenderer>().sprite = SpriteBuffer;

        
        //Swapping values of both tiles
        int ValueBuffer = this.value;
        this.value = t.value;
        t.value = ValueBuffer;

        return this;

    }

    public void ToggleSelect()
    {
        isSelected = !isSelected;
    }

    void OnMouseDown()
    {
        GameObject.Find("BoardManager").GetComponent<BoardManager>().TileSwap(this);

    }
}

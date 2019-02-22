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

    private SpriteRenderer SpriteRenderVar;

    // Start is called before the first frame update
    void Start()
    {
        CreateTile();
    }

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

    void CreateTile()
    {
        int RandomTileSelector = Random.Range(0, MySpriteImages.Length);

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = MySpriteImages[RandomTileSelector];
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

    public void ToggleSelect()
    {
        isSelected = !isSelected;
    }

    void OnMouseDown()
    {

        GameObject.Find("BoardManager").GetComponent<BoardManager>().TileSwap(this);
    }
}

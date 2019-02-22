using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Tile> Neighbours = new List<Tile>();
    public Sprite[] mySpriteImages = new Sprite[8];
    public Tile PreviousTile;
    public Tile BelowTile;
    public bool isLeftEdge;
    public bool isBottomEdge;

    private SpriteRenderer SpriteRenderVar;


    // Start is called before the first frame update
    void Start()
    {
        CreateTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTile()
    {
        int RandomTileSelector = Random.Range(0, mySpriteImages.Length);

        SpriteRenderVar = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderVar.sprite = mySpriteImages[RandomTileSelector];
        
    }


    void OnMouseDown()
    {
        print("Clicked");
    }
}

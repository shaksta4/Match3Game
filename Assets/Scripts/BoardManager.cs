using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();
    public int GridWidth;
    public int GridHeight;
    public GameObject tilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int y=0; y<GridHeight; y++)
        {
            for(int x=0; x < GridWidth; x++)
            {
                GameObject g = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                g.transform.parent = gameObject.transform;
                tiles.Add(g.GetComponent<Tile>());

                tiles[GridWidth * y].isLeftEdge = true;
                if(y == 0)
                {
                    tiles[x].isBottomEdge = true;
                }
            }
        }
        gameObject.transform.position = new Vector3(-2.5f, -2f, -0.5f);

        for(int i=0;i<tiles.Count;i++)
        {
            if(!tiles[i].isLeftEdge)
            {
                tiles[i].PreviousTile = tiles[i - 1];
            }

            if(!tiles[i].isBottomEdge)
            {
                tiles[i].BelowTile = tiles[i - GridWidth];
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

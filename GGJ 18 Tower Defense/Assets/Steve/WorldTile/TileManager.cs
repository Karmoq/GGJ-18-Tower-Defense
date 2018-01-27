using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour {
    public static TileManager singleton;
    public LayerMask tileMask;
    
    //Tile Settings
    public Vector3 TileSize;
    public float PathIncrements;

    [SerializeField] private List<GameObject> l_prefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> l_groundPrefabs = new List<GameObject>();
    [SerializeField] private WorldTile StartTile;

    // grid
    public Vector2Int gridSize;
    public WorldTile[,] l_worldTiles;

    public Text DebugText;

    public void Awake()
    {
        singleton = this;
    }

    public void Start()
    {
        l_worldTiles = new WorldTile[gridSize.x+1,gridSize.y+1];
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                CreateNewTile(x, y);
            }
        }
        StartTile.NorthTile = l_worldTiles[0, 0];
    }

    public WorldTile CreateNewTile(int x, int y)
    {
        //choose a random tile to spawn
        GameObject newTile;
        newTile = Instantiate(l_prefabs[Random.Range(0, l_prefabs.Count)]);

        WorldTile newTileScript = newTile.GetComponent<WorldTile>();
        l_worldTiles[x, y] = newTileScript;
        newTileScript.SetPosition(new Vector2Int(x, y));
        
        GameObject groundTile = Instantiate(l_groundPrefabs[Random.Range(0, l_groundPrefabs.Count)]);
        groundTile.transform.position += Vector3.down * 0.05f;
        groundTile.transform.SetParent(newTileScript.models);

        newTileScript.TurnRandom();

        newTile.transform.position = new Vector3(x * TileSize.x, 0, y * TileSize.z);
        newTile.transform.localScale = TileSize;
        newTile.name += " = " + x + " - " + y;

        return newTileScript;
    }
    public WorldTile CreateNewTile(int x, int y, WorldTile.Type type)
    {
        //choose a random tile to spawn
        GameObject newTile;
        newTile = Instantiate(l_prefabs[Random.Range(0, l_prefabs.Count)]);

        WorldTile newTileScript = newTile.GetComponent<WorldTile>();
        l_worldTiles[x, y] = newTileScript;
        newTileScript.SetPosition(new Vector2Int(x, y));

        GameObject groundTile = Instantiate(l_groundPrefabs[Random.Range(0, l_groundPrefabs.Count)]);
        groundTile.transform.position += Vector3.down * 0.05f;
        groundTile.transform.SetParent(newTileScript.models);

        newTileScript.TurnRandom();

        newTile.transform.position = new Vector3(x * TileSize.x, 0, y * TileSize.z);
        newTile.transform.localScale = TileSize;
        newTile.name += " = " + x + " - " + y;

        return newTileScript;
    }

    public WorldTile GetTileFromPosition(int x, int y)
    {
        if(x < 0) return null;
        if (x > gridSize.x)
            return null;
        if (y < 0)
            return null;
        if (y > gridSize.y)
            return null;
        return l_worldTiles[x, y];
    }

    //public WorldTile[] CreateGuaranteedPath(WorldTile[] l_worldTiles, Vector2Int StartPosition, Vector2Int EndPosition)
    //{
    //    WorldTile currentTile = CreateNewTile(StartPosition.x, StartPosition.y);
    //    Vector2 direction = EndPosition - currentTile.GetPosition();
    //    Debug.Log(direction);
    //    while (currentTile.GetPosition() != EndPosition)
    //    {
    //
    //    }
    //        
    //}

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(ray, out hitInfo, 500, tileMask))
            {
                hitInfo.collider.GetComponent<WorldTile>().TurnRight();
                Debug.Log("Turned " + hitInfo.collider.name);
            }
        }
    }
}

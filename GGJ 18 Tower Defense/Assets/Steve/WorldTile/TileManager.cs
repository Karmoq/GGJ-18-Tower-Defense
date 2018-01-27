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
    //random tiles
    [SerializeField] private List<GameObject> l_prefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> l_groundPrefabs = new List<GameObject>();
    [SerializeField] public WorldTile StartTile;

    // grid
    public Vector2Int gridSize;
    public WorldTile[,] l_worldTiles;

    public Text DebugText;

    public void Awake()
    {
        singleton = this;
        l_worldTiles = new WorldTile[gridSize.x + 1, gridSize.y + 1];
        StartTile = CreateTileOfType(-1, gridSize.y / 2, WorldTile.Type.Start, false);
        CreateGuaranteedPath(l_worldTiles, new Vector2Int(0, gridSize.y / 2), new Vector2Int(gridSize.x, gridSize.y / 2));
        //fill the empty slots
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                if (l_worldTiles[x, y] == null)
                {
                    l_worldTiles[x, y] = CreateRandomTile(x, y);
                }
            }
        }
        StartTile.NorthTile = l_worldTiles[0, gridSize.y/2];
    }
 
    public WorldTile CreateRandomTile(int x, int y)
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

    public WorldTile CreateTileOfType(int x, int y, WorldTile.Type type, bool turnRandom)
    {
        //choose a random tile to spawn
        GameObject newTile;
        newTile = Instantiate(l_prefabs[(int)type]);

        WorldTile newTileScript = newTile.GetComponent<WorldTile>();
        newTileScript.SetPosition(new Vector2Int(x, y));

        GameObject groundTile = Instantiate(l_groundPrefabs[Random.Range(0, l_groundPrefabs.Count)]);
        groundTile.transform.position += Vector3.down * 0.05f;
        groundTile.transform.SetParent(newTileScript.models);

        if(turnRandom)
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

    //Create a guaranteed path so the train player has the possibility to reach the target
    public WorldTile[,] CreateGuaranteedPath(WorldTile[,] l_worldTiles, Vector2Int StartPosition, Vector2Int EndPosition)
    {
        WorldTile.Rotation lastRotation;
        Vector2Int currentPosition = StartPosition;
        Vector2 direction = EndPosition - StartPosition;
        WorldTile.Rotation currentRotation =  GetRotationFromDirection(direction);

        float randomFactor = 0.5f;
        while(currentPosition != EndPosition)
        {
            lastRotation = currentRotation;
            direction = EndPosition - currentPosition;
            currentRotation = GetRotationFromDirection(direction);
            //Randomize the rotation a bit
            if(Random.Range(0,1f) > randomFactor && !isEdgePosition(currentPosition)) // randomize and check if it is not an edge tile
            {
                currentRotation = WorldTile.TurnBy(currentRotation, Random.Range(-1, 2));
                randomFactor = 0;
            } else
            {
                randomFactor += 0.05f;
            }

            // spawn the tile
            if (currentRotation == lastRotation) // straight Tile
            {
                WorldTile newTile;
                if(Random.Range(0,2) == 0)
                    newTile = CreateTileOfType(currentPosition.x, currentPosition.y, WorldTile.Type.Straight, true);
                else
                    newTile = CreateTileOfType(currentPosition.x, currentPosition.y, WorldTile.Type.T1, true);

                if(l_worldTiles[currentPosition.x, currentPosition.y] != null)
                {
                    Destroy(l_worldTiles[currentPosition.x, currentPosition.y].gameObject);
                }
                l_worldTiles[currentPosition.x, currentPosition.y] = newTile;
                currentPosition += WorldTile.GetVector2IntFromRotation(currentRotation);
            } else  // curve Tile
            {
                WorldTile newTile = CreateTileOfType(currentPosition.x, currentPosition.y, WorldTile.Type.Curve, true);
                if (l_worldTiles[currentPosition.x, currentPosition.y] != null)
                {
                    Destroy(l_worldTiles[currentPosition.x, currentPosition.y].gameObject);
                }
                l_worldTiles[currentPosition.x, currentPosition.y] = newTile;
                currentPosition += WorldTile.GetVector2IntFromRotation(currentRotation);
            }
        }
        return l_worldTiles;
    }

    public WorldTile.Rotation GetRotationFromDirection(Vector2 direction)
    {
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                return WorldTile.Rotation.North;
            else
                return WorldTile.Rotation.South;
        } else
        {
            if (direction.y > 0)
                return WorldTile.Rotation.West;
            else
                return WorldTile.Rotation.East;
        }
    }

    public bool isEdgePosition(Vector2Int position)
    {
        if (position.x == 0 || position.x == gridSize.x || position.y == 0 || position.y == gridSize.y)
        {
            return true;
        }
        return false;
    }

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

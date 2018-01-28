using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour {
    public static TileManager singleton;
    
    //Tile Settings
    public Vector3 TileSize;
    public float PathIncrements;
    //random tiles
    [SerializeField] private List<GameObject> l_prefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> l_groundPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> l_randomCleanGroundTiles = new List<GameObject>();
    [SerializeField] public WorldTile StartTile;

    public float StraightAmount;
    public float CurveAmount;
    public float T1Amount;
    public float T2Amount;
    public float CleanAmount;

    // grid
    public Vector2Int gridSize;
    public WorldTile[,] l_worldTiles;

    public Text DebugText;

    public void Awake()
    {
        singleton = this;
        l_worldTiles = new WorldTile[gridSize.x + 1, gridSize.y + 1];
        StartTile = CreateTileOfType(-1, gridSize.y / 2, WorldTile.Type.Start, false);
        CreateGuaranteedPath(l_worldTiles, new Vector2Int(0, gridSize.y / 2), new Vector2Int(gridSize.x, (int)(gridSize.y * 0.25f)));
        CreateGuaranteedPath(l_worldTiles, new Vector2Int(0, gridSize.y / 2), new Vector2Int(gridSize.x, (int)(gridSize.y * 0.75f)));
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
        float random = Random.Range(0, StraightAmount + CurveAmount + T1Amount + T2Amount + CleanAmount);
        WorldTile.Type tileType;
        if (random < StraightAmount)
            tileType = WorldTile.Type.Straight;
        else if (random < StraightAmount + CurveAmount)
            tileType = WorldTile.Type.Curve;
        else if (random < StraightAmount + CurveAmount + T1Amount)
            tileType = WorldTile.Type.T1;
        else if (random < StraightAmount + CurveAmount + T1Amount+ T2Amount)
            tileType = WorldTile.Type.T2;
        else
            tileType = WorldTile.Type.Clean;
        
        return CreateTileOfType(x, y, tileType, true);
    }

    public WorldTile CreateTileOfType(int x, int y, WorldTile.Type type, bool turnRandom)
    {
        //choose a random tile to spawn
        GameObject newTile;
        newTile = Instantiate(l_prefabs[(int)type]);

        WorldTile newTileScript = newTile.GetComponent<WorldTile>();
        newTileScript.SetPosition(new Vector2Int(x, y));

        GameObject groundTile;
        groundTile = Instantiate(l_groundPrefabs[Random.Range(0, l_groundPrefabs.Count)]);

        if (type == WorldTile.Type.Clean)
        {
            GameObject cleanTileAdditional = Instantiate(l_randomCleanGroundTiles[Random.Range(0, l_randomCleanGroundTiles.Count)]);
            cleanTileAdditional.transform.position += Vector3.down * 0.05f;
            cleanTileAdditional.transform.SetParent(newTileScript.models);
        }

        groundTile.transform.position += Vector3.down * 0.05f;
        groundTile.transform.SetParent(newTileScript.models);

        if(turnRandom)
            newTileScript.TurnRandom();

        newTile.transform.position = new Vector3(x * TileSize.x, 0, y * TileSize.z);
        newTile.transform.localScale = TileSize;
        newTile.name += " = " + x + " - " + y;

        newTileScript.type = type;
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
        WorldTile.Rotation lastRotation = WorldTile.Rotation.North;
        Vector2Int currentPosition = StartPosition;
        Vector2 direction = EndPosition - StartPosition;
        WorldTile.Rotation currentRotation =  GetRotationFromDirection(direction);

        float randomFactor = 0.5f;
        while(currentPosition != EndPosition)
        {
            bool allowRandomizer = false;
            if(lastRotation == currentRotation)
            {
                allowRandomizer = true;
            }
            lastRotation = currentRotation;
            direction = EndPosition - currentPosition;
            currentRotation = GetRotationFromDirection(direction);
            //Randomize the rotation a bit
            if (allowRandomizer)
            {
                if (Random.Range(0, 1f) < randomFactor && !isEdgePosition(currentPosition)) // randomize and check if it is not an edge tile
                {
                    WorldTile.Rotation randomRotation = WorldTile.TurnBy(currentRotation, Random.Range(-1, 2));
                    if ((randomRotation - currentRotation) % 2 == 1)
                    {
                        currentRotation = WorldTile.TurnBy(currentRotation, Random.Range(-1, 2));
                        randomFactor = -0.1f;
                    }
                }
                else
                {
                    randomFactor += 0.5f;
                }
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

        WorldTile endTile = CreateTileOfType(EndPosition.x, EndPosition.y, WorldTile.Type.End, false);
        l_worldTiles[endTile.GetPosition().x, endTile.GetPosition().y] = endTile;
        l_worldTiles[EndPosition.x-1, EndPosition.y].NorthTile = endTile;
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

    public void FixedUpdate()
    {
        foreach (WorldTile tile in l_worldTiles)
        {
            tile.selected = false;
        }
    }
}

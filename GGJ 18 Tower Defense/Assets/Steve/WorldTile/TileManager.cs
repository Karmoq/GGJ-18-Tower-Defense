using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour {
    public static TileManager singleton;
    public LayerMask tileMask;
    
    //Tile Settings
    public Vector3 TileSize;
    [SerializeField] private GameObject StraightPrefab;
    [SerializeField] private GameObject CurvePrefab;
    [SerializeField] private GameObject TPrefab;

    [SerializeField] private float StraightAmount;
    [SerializeField] private float CurveAmount;
    [SerializeField] private float TAmount;

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

    public void CreateNewTile(int x, int y)
    {
        float randomTogether = StraightAmount + CurveAmount + TAmount;
        float randomFloat = Random.Range(0, randomTogether);
        DebugText.text = "" + randomFloat + " - " + randomTogether;

        //choose a random tile to spawn
        GameObject newTile;
        if (randomFloat <= StraightAmount / randomTogether)
            newTile = Instantiate(StraightPrefab);
        else if (randomFloat <= StraightAmount + CurveAmount / randomTogether)
            newTile = Instantiate(CurvePrefab);
        else
            newTile = Instantiate(TPrefab);

        //
        l_worldTiles[x, y] = newTile.GetComponent<WorldTile>();
        newTile.GetComponent<WorldTile>().SetPosition(new Vector2Int(x, y));
        newTile.transform.position = new Vector3(x * TileSize.x, 0, y * TileSize.z);
        newTile.transform.localScale = TileSize;
        newTile.name += " = " + x + " - " + y;

        newTile.GetComponent<WorldTile>().TurnRandom();
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

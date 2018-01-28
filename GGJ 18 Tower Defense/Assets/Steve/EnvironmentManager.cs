using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour {
    [SerializeField] private GameObject TemplePrefab;
    [SerializeField] private GameObject SlaughterHousePrefab;
    [SerializeField] private List<GameObject> CanyonPrefabs = new List<GameObject>();


    private void Awake()
    {
        //SpawnPrefab(TemplePrefab, new Vector3(-3, 0, TileManager.singleton.gridSize.y / 2 * TileManager.singleton.TileSize.y));
        //SpawnPrefab(SlaughterHousePrefab, new Vector3(TileManager.singleton.gridSize.x * TileManager.singleton.TileSize.x + 3, 0, TileManager.singleton.gridSize.y / 2 * TileManager.singleton.TileSize.y));

        for (float x = -12; x < TileManager.singleton.gridSize.x * TileManager.singleton.TileSize.x * 2; x += 3.5f)
        {
                SpawnPrefab(CanyonPrefabs[Random.Range(0, CanyonPrefabs.Count)], new Vector3(x, 0, TileManager.singleton.gridSize.y * TileManager.singleton.TileSize.y +3));
        }
    }

    private void SpawnPrefab(GameObject prefab, Vector3 position)
    {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.position = position;
    }

}

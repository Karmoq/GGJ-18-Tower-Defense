using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWagon : MonoBehaviour {

    public Train train;

    public List<Vector3> TrainPath = new List<Vector3>();
    public float offset = 0;

    public WorldTile currentTile;

    public LayerMask tileMask;

    public void Update()
    {
        if (TrainPath.Count > (int)(offset*TileManager.singleton.PathIncrements))
        {
            if (Vector3.Distance(TrainPath[0], transform.position) > 0.05f)
            {
                Vector3 velocity = TrainPath[0] - transform.position;
                transform.position += Vector3.ClampMagnitude(velocity, train.currentSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(TrainPath[0] - transform.position);
            }
            else
            {
                TrainPath.RemoveAt(0);
            }
        }

        Ray ray = new Ray(transform.position + Vector3.up * 1, Vector3.down);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, 50, tileMask))
        {
            if(currentTile != hitInfo.collider.GetComponent<WorldTile>() && currentTile != null) // if the new Tile is a new one
            {
                currentTile.locked = false;
            }
            currentTile = hitInfo.collider.GetComponent<WorldTile>();
            currentTile.locked = true;
        }
        else // if the wagon left the tile
        {
            if(currentTile != null)
                currentTile.locked = false;
            currentTile = null;
        }
    }
}

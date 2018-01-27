﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWagon : MonoBehaviour {

    public Train train;

    public float offsetDistance = 0;

    public WorldTile currentTile;

    public LayerMask tileMask;

    public void Update()
    {
        transform.position = train.GetPathPositionByOffset(offsetDistance);
        if(transform.position == Vector3.zero)
        {
            transform.position = TileManager.singleton.StartTile.transform.position;
        }
        if(train.GetPathPositionByOffset(offsetDistance - 0.1f) - transform.position != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(train.GetPathPositionByOffset(offsetDistance - 0.1f) - transform.position);

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

    public void Destroy()
    {
        if (currentTile != null)
            currentTile.locked = false;
        Destroy(gameObject);
    }
}

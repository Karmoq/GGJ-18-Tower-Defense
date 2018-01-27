using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : MonoBehaviour
{
    public WorldTile tile;

    public WorldTile.Rotation StartPoint;
    public WorldTile.Rotation EndPoint;



    public Vector3[] GetPath()
    {
        Vector3 startPosition = tile.GetPositionOfDirection(StartPoint);
        Vector3 endPosition = tile.GetPositionOfDirection(EndPoint);
        float distance = Vector3.Distance(startPosition, endPosition);
        List<Vector3> pathPoints = new List<Vector3>();
        for(float x = 0; x < distance; x+= 0.1f)
        {
            pathPoints.Add(Vector3.Lerp(startPosition, endPosition, x / distance));
        }
        return pathPoints.ToArray();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tile.GetPositionOfDirection(StartPoint), tile.GetPositionOfDirection(EndPoint));
        foreach(Vector3 t_pos in GetPath())
        {
            Gizmos.DrawSphere(t_pos, 0.02f);
        }
    }
}

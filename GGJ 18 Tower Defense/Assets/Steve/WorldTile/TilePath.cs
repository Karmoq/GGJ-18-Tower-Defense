using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : MonoBehaviour
{
    public WorldTile tile;

    public WorldTile.Rotation StartPoint;
    public WorldTile.Rotation EndPoint;

    public List<Vector3> GetPath()
    {
        Vector3 startPosition = tile.GetPositionOfDirection(StartPoint);
        Vector3 endPosition = tile.GetPositionOfDirection(EndPoint);
        float distance = Vector3.Distance(startPosition, endPosition);
        List<Vector3> pathPoints = new List<Vector3>();
        if (((int)StartPoint - (int)EndPoint) % 2 == 0) // if the path is straight
        {
            for (float x = 0; x < distance; x += 1 / TileManager.singleton.PathIncrements)
            {
                pathPoints.Add(Vector3.Lerp(startPosition, endPosition, x / distance));
            }
        } else // if the path is a curve
        {
            for (float i = 0; i < 90; i += (1f/TileManager.singleton.PathIncrements) * 90f)
            {
                float x = Mathf.Sin(i * Mathf.Deg2Rad) * 0.5f;
                float y = Mathf.Cos(i * Mathf.Deg2Rad) * 0.5f;

                int rotation = WorldTile.CurveDirection(StartPoint, EndPoint);
                Vector2 point = Vector2.zero;
                if (StartPoint == WorldTile.Rotation.North)
                {
                    point = new Vector3(-x, -y * rotation);
                }
                else if (StartPoint == WorldTile.Rotation.South)
                {
                    point = new Vector3(x, y * rotation);
                }
                else if (StartPoint == WorldTile.Rotation.East)
                {
                    point = new Vector3(-y * rotation, x);
                }
                else 
                {
                    point = new Vector3(y * rotation, -x);
                }
                point += (Vector2)WorldTile.GetVector2IntFromRotation(StartPoint) * 0.5f + (Vector2)WorldTile.GetVector2IntFromRotation(EndPoint) * 0.5f;
                pathPoints.Add(new Vector3(point.x, startPosition.y, point.y)+transform.position);
            }
        }
        return pathPoints;
    }
}

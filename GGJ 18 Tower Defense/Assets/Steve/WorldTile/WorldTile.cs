using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour {

    [SerializeField] private Vector2Int position;

    public Vector3 goalRotation;
    public Vector3 goalPosition;

    [Range(0,3)] public int currentRotation = 0;

    public List<TilePath> l_paths = new List<TilePath>();

    public WorldTile NorthTile;
    public WorldTile EastTile;
    public WorldTile SouthTile;
    public WorldTile WestTile;

    [SerializeField] public Transform models;
    public bool selected = false;
    public bool locked = false;

    public void Awake()
    {
        foreach(TilePath t_path in GetComponents<TilePath>())
        {
            l_paths.Add(t_path);
        }
    }

    public void Start()
    {
        NorthTile = TileManager.singleton.GetTileFromPosition(position.x + 1, position.y);
        EastTile = TileManager.singleton.GetTileFromPosition(position.x, position.y - 1);
        SouthTile = TileManager.singleton.GetTileFromPosition(position.x - 1, position.y);
        WestTile = TileManager.singleton.GetTileFromPosition(position.x, position.y + 1);

        goalPosition = transform.position;
    }

    public void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(goalRotation), 0.25f);
        
        models.transform.position = Vector3.Lerp(models.transform.position, goalPosition, 0.25f);
    }

    public void TurnRight()
    {
        if (locked)
            return;
        currentRotation++;
        if (currentRotation > 3)
        {
            currentRotation -= 4;
        }
        foreach(TilePath t_path in l_paths)
        {
            t_path.StartPoint = TurnBy(t_path.StartPoint, 1);
            t_path.EndPoint = TurnBy(t_path.EndPoint, 1);
        }
        goalRotation = new Vector3(0, currentRotation * 90, 0);
    }

    public void TurnLeft()
    {
        if (locked)
            return;
        currentRotation--;
        if (currentRotation < 0)
        {
            currentRotation += 4;
        }
        foreach (TilePath t_path in l_paths)
        {
            t_path.StartPoint = TurnBy(t_path.StartPoint, -1);
            t_path.EndPoint = TurnBy(t_path.EndPoint, -1);
        }

        goalRotation = new Vector3(0, currentRotation * 90, 0);
    }

    public void TurnRandom()
    {
        if (locked)
            return;
        int randomTurn = Random.Range(0, 3);
        currentRotation += randomTurn;
        if (currentRotation < 0)
        {
            currentRotation += 4;
        }
        foreach (TilePath t_path in l_paths)
        {
            t_path.StartPoint = TurnBy(t_path.StartPoint, randomTurn);
            t_path.EndPoint = TurnBy(t_path.EndPoint, randomTurn);
        }

        goalRotation = new Vector3(0, currentRotation * 90, 0);
    }

    public TilePath GetTilePathByEntryPoint(Rotation t_entryPoint)
    {
        foreach(TilePath t_path in l_paths)
        {
            if(t_path.StartPoint == t_entryPoint)
            {
                return t_path;
            }
        }
        return null;
    }

    public WorldTile GetWorldTileByRotation(Rotation t_rotation)
    {
        if (t_rotation == Rotation.North)
            return NorthTile;
        if (t_rotation == Rotation.East)
            return EastTile;
        if (t_rotation == Rotation.South)
            return SouthTile;
        if (t_rotation == Rotation.West)
            return WestTile;
        return null;
    }

    public Vector3 GetPositionOfDirection(Rotation rotation)
    {
        if (rotation == Rotation.North)
            return transform.position + new Vector3(0.5f, 0.25f, 0);
        if (rotation == Rotation.East)
            return transform.position + new Vector3(0, 0.25f, -0.5f);
        if (rotation == Rotation.South)
            return transform.position + new Vector3(-0.5f, 0.25f, 0);
        if (rotation == Rotation.West)
            return transform.position + new Vector3(0, 0.25f, 0.5f);
        return transform.position;
    }


    public static Vector2Int GetVector2IntFromRotation(Rotation rotation)
    {
        if (rotation == Rotation.North)
            return Vector2Int.right;
        else if (rotation == Rotation.East)
            return Vector2Int.down;
        else if (rotation == Rotation.South)
            return Vector2Int.left;
        else
            return Vector2Int.up;
    }

    public Vector2Int GetPosition() { return position; }
    public void SetPosition(Vector2Int t_pos) { position = t_pos; }

    public static Rotation TurnBy(Rotation current, int value)
    {
        int currentRotation = (int)current;
        currentRotation += value;
        if (currentRotation > 3)
            currentRotation -= 4;
        if (currentRotation < 0)
            currentRotation += 4;
        return (Rotation)currentRotation;
    }

    public enum Rotation { North, East, South, West }

    public enum Type { Straight, Curve, T1, T2, Clean, Start }
}

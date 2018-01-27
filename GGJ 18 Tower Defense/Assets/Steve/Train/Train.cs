using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Train : MonoBehaviour {
    //Train Stats
    [SerializeField] private float targetSpeed;
    [SerializeField] public float currentSpeed;
    [SerializeField] public List<TrainWagon> l_wagons = new List<TrainWagon>();

    // Pathing stuff
    [SerializeField] private WorldTile StartTile;
    [SerializeField] private WorldTile currentTile;
    [SerializeField] private TilePath currentPath;
    [SerializeField] private Vector3[] currentPathPoints;
    [SerializeField] private int pointIndex = 0;

    public Text DebugText;

    public void Awake()
    {
        currentTile = StartTile;
        currentPath = StartTile.GetComponent<TilePath>();
        currentPathPoints = currentPath.GetPath();
        pointIndex = 0;
    }

    public virtual void Update()
    {
        if(pointIndex < currentPathPoints.Length)
        {
            if (Vector3.Distance(currentPathPoints[pointIndex], transform.position) > 0.05f) // move into the direction of the next point
            {
                Vector3 velocity = currentPathPoints[pointIndex] - transform.position;
                currentSpeed = Mathf.Clamp(currentSpeed + 0.1f * Time.deltaTime, 0, targetSpeed);
                transform.position += Vector3.ClampMagnitude(velocity, currentSpeed * Time.deltaTime);
            }
            else
            {// load the next point
                foreach(TrainWagon t_wagon in l_wagons)
                {
                    t_wagon.TrainPath.Add(currentPathPoints[pointIndex]);
                }
                pointIndex++;
            }
        }
        else
        {
            WorldTile nextTile = currentTile.GetWorldTileByRotation(currentPath.EndPoint);
            if (nextTile != null && !nextTile.selected)
            {
                TilePath nextPath = nextTile.GetTilePathByEntryPoint(WorldTile.TurnBy(currentPath.EndPoint, 2));
                if (nextPath != null)
                {
                    currentTile.locked = false;
                    currentTile = nextTile;
                    currentPath = nextPath;
                    currentPathPoints = nextPath.GetPath();
                    pointIndex = 0;
                    currentTile.locked = true;
                }
            }
            else
            {
                currentSpeed = 0;
            }
        }
    }



    public enum State { Driving, WaitingForNextTile }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Train : MonoBehaviour {
    //Train Stats
    [SerializeField] private float targetSpeed;
    [SerializeField] public float currentSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] public List<TrainWagon> l_wagons = new List<TrainWagon>();
    private int wagonAmount;
    private int wagonMaxAmount;

    [SerializeField] private Animator c_animator;
    private TrainHealth trainHealth;

    // Pathing stuff
    [SerializeField] public WorldTile StartTile;
    [SerializeField] private WorldTile currentTile;
    [SerializeField] private TilePath currentPath;
    [SerializeField] private List<Vector3> currentPathPoints = new List<Vector3>();
    [SerializeField] private int pointIndex = 0;

    [SerializeField] private float distanceToNextPoint = 0;
    [SerializeField] private List<Vector3> pathForWagons = new List<Vector3>();

    public Text DebugText;

    private void Awake()
    {
        c_animator = GetComponentInChildren<Animator>();
        trainHealth = GetComponent<TrainHealth>();
    }

    public void Start()
    {
        StartTile.locked = true;
        currentTile = StartTile;
        currentPath = StartTile.GetComponent<TilePath>();
        currentPathPoints = currentPath.GetPath();
        transform.position = currentPathPoints[0];
        foreach(TrainWagon t_wagon in l_wagons)
        {
            t_wagon.transform.position = currentPathPoints[0];
        }
        pointIndex = 0;
        wagonMaxAmount = l_wagons.Count;
    }

    public virtual void Update()
    {
        if(pointIndex < currentPathPoints.Count)
        {
            distanceToNextPoint = Vector3.Distance(currentPathPoints[pointIndex], transform.position);
            if (distanceToNextPoint > currentSpeed * Time.deltaTime) // move into the direction of the next point
            {
                transform.position = GetPathPositionBySpeed(currentSpeed);
                currentSpeed = Mathf.Clamp(currentSpeed + acceleration, 0, targetSpeed);
                transform.rotation = Quaternion.LookRotation()
            }
            else
            {// load the next point
                pathForWagons.Add(currentPathPoints[pointIndex]);
                pointIndex++;
            }
        }
        else
        {
            WorldTile nextTile = currentTile.GetWorldTileByRotation(currentPath.EndPoint);
            if (nextTile != null && !nextTile.selected && !nextTile.locked)
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

        c_animator.speed = currentSpeed*3;
    }

    public void FixedUpdate()
    {
        wagonAmount = l_wagons.Count;
        float wagonPercentage = (float)wagonAmount / (float)wagonMaxAmount;
        if(wagonPercentage > trainHealth.GetCurrentHealthPercentage() && trainHealth.GetCurrentHealthPercentage() > 0)
        {
            TrainWagon lastWagon = l_wagons[l_wagons.Count-1];
            l_wagons.Remove(lastWagon);
            lastWagon.Destroy();
        }
    }

    public void OnDestroy()
    {
        currentTile.locked = false;
    }

    public Vector3 GetPathPositionByOffset(float offset)
    {
        float offsetCounter = 0;
        int pathIndex = pathForWagons.Count-1;
        if (pathIndex < 0)
            return Vector3.zero;
        Vector3 lastPoint = pathForWagons[pathIndex];
        Vector3 thisPoint = pathForWagons[pathIndex];
        while(offsetCounter < offset)
        {
            if (pathIndex < 1)
            {
                return Vector3.zero;
            }
            thisPoint = pathForWagons[--pathIndex];
            offsetCounter += Vector3.Distance(lastPoint, thisPoint);
            lastPoint = thisPoint;
        }
        return pathForWagons[pathIndex];
    }

    public Vector3 GetPathPositionBySpeed(float speed, bool remove)
    {
        float speedCounter = 0;
        int pathIndex = pointIndex;

        Vector3 lastPoint = currentPathPoints[pointIndex];
        Vector3 thisPoint = currentPathPoints[pointIndex];

        while(speedCounter < speed)
        {
            if(pathIndex > currentPathPoints.Count - 1)
            {
                return currentPathPoints[currentPathPoints.Count - 1];
            }
            thisPoint = currentPathPoints[++pathIndex];
            speedCounter += Vector3.Distance(lastPoint, thisPoint);
            lastPoint = thisPoint;
        }
        if(pathIndex < currentPathPoints.Count - 2)
        {
            float dist = Vector3.Distance(lastPoint, currentPathPoints[++pathIndex]);
        }
        return currentPathPoints[pathIndex];

    }

    public enum State { Driving, WaitingForNextTile }
}

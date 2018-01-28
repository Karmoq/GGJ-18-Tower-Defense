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

    [SerializeField] private bool hasReachedEndOfPath = false;

    [SerializeField] private int pointIndex = 0;
    [SerializeField] private int currentPathIndex = 0;

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
        currentPathIndex = 0;
        wagonMaxAmount = l_wagons.Count;
    }

    public virtual void Update()
    {
        float speedLeft = currentSpeed * Time.deltaTime;
        if(pointIndex < currentPathPoints.Count)
        {
            Vector3 position = transform.position;
            while (speedLeft > 0)
            {
                if(pointIndex >= currentPathPoints.Count)
                {
                    break;
                }
                distanceToNextPoint = Vector3.Distance(currentPathPoints[pointIndex], transform.position);
                position = currentPathPoints[pointIndex];
                if(currentPathPoints[pointIndex] - transform.position != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(currentPathPoints[pointIndex] - transform.position);
                speedLeft -= distanceToNextPoint;
                pathForWagons.Add(position);
                pointIndex++;
            }
            transform.position = position;

            currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, targetSpeed);
        }
        else
        {
            if(currentTile.type == WorldTile.Type.End)
            {

            }
            WorldTile nextTile = currentTile.GetWorldTileByRotation(currentPath.EndPoint);
            if(nextTile == null)
            {
                foreach(TrainWagon t_wagon in l_wagons)
                {
                    t_wagon.Destroy();
                }
                trainHealth.TakeDamage(999);
            }
            if (nextTile != null && !nextTile.selected && !nextTile.locked)
            {
                if(nextTile.type == WorldTile.Type.Clean)
                {
                    foreach (TrainWagon t_wagon in l_wagons)
                    {
                        t_wagon.Destroy();
                    }
                    trainHealth.Die();
                }
                TilePath nextPath = nextTile.GetTilePathByEntryPoint(WorldTile.TurnBy(currentPath.EndPoint, 2));
                if (nextPath != null)
                {
                    currentTile.locked = false;
                    currentTile = nextTile;
                    currentPath = nextPath;
                    currentPathPoints = nextPath.GetPath();
                    pointIndex = 0;
                    currentTile.locked = true;

                    currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, targetSpeed);
                }
                else
                {
                    currentSpeed = 0;
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

    public Vector3 GetPathPositionBySpeed(float speed)
    {
        float speedCounter = speed;
        Debug.Log(speedCounter);
        Vector3 thisPoint = currentPathPoints[0];
        Vector3 lastPoint = currentPathPoints[0];
        
        while(speedCounter > 0)
        {
            if(currentPathIndex >= currentPathPoints.Count)
            {
                hasReachedEndOfPath = true;
                Debug.Log("Reached End Of Path");
                return currentPathPoints[currentPathPoints.Count-1];
            }
            thisPoint = currentPathPoints[currentPathIndex++];
            speedCounter -= Vector3.Distance(thisPoint, lastPoint);
            lastPoint = thisPoint;
        }
        return Vector3.Lerp(thisPoint, lastPoint, Mathf.Abs(speedCounter));
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

    public enum State { Driving, WaitingForNextTile }
}

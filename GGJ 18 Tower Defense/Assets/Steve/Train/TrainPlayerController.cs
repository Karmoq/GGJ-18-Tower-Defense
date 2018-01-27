using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainPlayerController : MonoBehaviour {

    // Tile Selection
    [SerializeField] private WorldTile selectedTile;
    [SerializeField] private bool moved = false;
    [SerializeField] private bool turned = false;


    // TrainSpawning
    [SerializeField] private float nextTrainCooldown = 30;
    [SerializeField] private float nextTrainTimer = 30;

    [SerializeField] private GameObject TrainPrefab;
    [SerializeField] private GameObject CoalWagonPrefab;
    [SerializeField] private GameObject WagonPrefab;

    public void Start()
    {
        selectedTile = TileManager.singleton.StartTile.NorthTile;
        selectedTile.selected = true;
        selectedTile.goalPosition += Vector3.up * 0.5f;

        SpawnTrain(5);
    }

    public void FixedUpdate()
    {
        float x = Input.GetAxis("Joystick1Axis1");
        float y = Input.GetAxis("Joystick1Axis2");
        float threshhold = 0.2f;
        if (x > threshhold && y < -threshhold)
            Move(WorldTile.Rotation.North);
        else if (x < -threshhold && y > threshhold)
            Move(WorldTile.Rotation.South);
        else if (x > threshhold && y > threshhold)
            Move(WorldTile.Rotation.East);
        else if (x < -threshhold && y < -threshhold)
            Move(WorldTile.Rotation.West);
        else
            moved = false;

        float turn = Input.GetAxis("Joystick1Axis3");

        if (turn > 0.5f || turn < -0.5f)
            Turn(turn);
        else
            turned = false;

        if(nextTrainTimer > 0)
        {
            nextTrainTimer -= Time.deltaTime;
        } else
        {
            if (!TileManager.singleton.StartTile.locked)
            {
                SpawnTrain(5);
                nextTrainTimer = nextTrainCooldown;
            }
        }

    }

    private void SpawnTrain(int wagons)
    {
        GameObject Train = Instantiate(TrainPrefab);
        Train TrainController = Train.GetComponent<Train>();
        TrainController.StartTile = TileManager.singleton.StartTile;

        GameObject CoalWagon = Instantiate(CoalWagonPrefab);
        TrainWagon CoalWagonController = CoalWagon.GetComponent<TrainWagon>();
        CoalWagonController.train = TrainController;
        CoalWagonController.offset = 1.2f;
        TrainController.l_wagons.Add(CoalWagonController);

        for (int x = 0; x < wagons; x++)
        {
            GameObject newWagon = Instantiate(WagonPrefab);
            TrainWagon WagonController = newWagon.GetComponent<TrainWagon>();
            WagonController.train = TrainController;
            WagonController.offset = x + 2;
            TrainController.l_wagons.Add(WagonController);
        }
    }

    private void Move(WorldTile.Rotation rotation)
    {
        if (!moved)
        {
            if (selectedTile.GetWorldTileByRotation(rotation) != null)
            {
                if (!selectedTile.GetWorldTileByRotation(rotation).locked)
                {
                    selectedTile.selected = false;
                    selectedTile.goalPosition += Vector3.down * 0.5f;
                    selectedTile = selectedTile.GetWorldTileByRotation(rotation);
                    selectedTile.goalPosition += Vector3.up * 0.5f;
                    selectedTile.selected = true;
                } else
                {
                    WorldTile nextTile = selectedTile.GetWorldTileByRotation(rotation).GetWorldTileByRotation(rotation);
                    if (nextTile != null)
                    {
                        if (!nextTile.locked)
                        {
                            selectedTile.selected = false;
                            selectedTile.goalPosition += Vector3.down * 0.5f;
                            selectedTile = nextTile;
                            selectedTile.goalPosition += Vector3.up * 0.5f;
                            selectedTile.selected = true;
                        }
                    }
                }
            }
            moved = true;
        }
    }

    private void Turn(float turn)
    {
        if (!turned)
        {
            if (turn > 0)
                selectedTile.TurnRight();
            else
                selectedTile.TurnLeft();
            turned = true;
        }
    }

    
}

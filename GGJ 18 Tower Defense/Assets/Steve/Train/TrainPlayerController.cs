using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainPlayerController : MonoBehaviour {

    // Tile Selection
    [SerializeField] private WorldTile selectedTile;
    [SerializeField] private bool moved = false;
    [SerializeField] private bool turned = false;
    private bool ControllerInput = true;

    public LayerMask tileMask;


    // TrainSpawning
    [SerializeField] private float nextTrainCooldown = 30;
    [SerializeField] private float nextTrainTimer = 30;

    [SerializeField] private GameObject TrainPrefab;
    [SerializeField] private GameObject CoalWagonPrefab;
    [SerializeField] private GameObject WagonPrefab;
    public int wagons;

    public void Start()
    {
        selectedTile = TileManager.singleton.StartTile.NorthTile;
        selectedTile.selected = true;
        selectedTile.goalPosition += Vector3.up * 0.5f;

        SpawnTrain(wagons);
    }
    

    public void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            ControllerInput = false;
        }
        else if(Mathf.Abs(Input.GetAxis("Joystick2Axis3"))> 0.1f)
        {
            ControllerInput = true;
            if(selectedTile == null)
            {
                selectedTile = TileManager.singleton.StartTile.NorthTile;
                selectedTile.goalPosition += Vector3.up * 0.5f;
            }
        }


        if (ControllerInput)
        {
            GetControllerInput();
        }
        else
        {
            GetMouseInput();
        }


        //TrainSpawning
        if (nextTrainTimer > 0)
        {
            nextTrainTimer -= Time.deltaTime;
        }
        else
        {
            if (!TileManager.singleton.StartTile.locked)
            {
                SpawnTrain(wagons);
                nextTrainTimer = nextTrainCooldown--;
            }
        }
    }
    
    public void GetControllerInput()
    {
        float xJoyStick = Input.GetAxis("Joystick2Axis1");
        float yJoyStick = Input.GetAxis("Joystick2Axis2");

        float xDPad = Input.GetAxis("Joystick2Axis6");
        float yDPad = Input.GetAxis("Joystick2Axis7");

        float threshhold = 0.2f;
        if (xJoyStick > threshhold && yJoyStick < -threshhold)
            MoveWithController(WorldTile.Rotation.North);
        else if (xJoyStick < -threshhold && yJoyStick > threshhold)
            MoveWithController(WorldTile.Rotation.South);
        else if (xJoyStick > threshhold && yJoyStick > threshhold)
            MoveWithController(WorldTile.Rotation.East);
        else if (xJoyStick < -threshhold && yJoyStick < -threshhold)
            MoveWithController(WorldTile.Rotation.West);
        else if (xDPad > threshhold)
            MoveWithController(WorldTile.Rotation.North);
        else if (xDPad < -threshhold)
            MoveWithController(WorldTile.Rotation.South);
        else if (yDPad < -threshhold)
            MoveWithController(WorldTile.Rotation.East);
        else if (yDPad > threshhold)
            MoveWithController(WorldTile.Rotation.West);
        else
            moved = false;


        float turn = Input.GetAxis("Joystick2Axis3");

        if (turn > 0.5f || turn < -0.5f)
            Turn(turn);
        else
            turned = false;



    }

    public void GetMouseInput()
    {
        Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, 500, tileMask))
        {
            WorldTile hoverTile = hitInfo.collider.GetComponent<WorldTile>();
            if (!hoverTile.locked)
            {
                if (selectedTile != null)
                {
                    selectedTile.selected = false;
                    selectedTile.ResetGoalPosition();
                }
                selectedTile = hoverTile;
                selectedTile.goalPosition += Vector3.up * 0.5f;
                selectedTile.selected = true;
            }

            selectedTile = hitInfo.collider.GetComponent<WorldTile>();
            selectedTile.selected = true;
        }
        else
        {
            if (selectedTile != null)
            {
                selectedTile.selected = false;
                selectedTile.ResetGoalPosition();
            }
            selectedTile = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedTile != null)
                selectedTile.TurnLeft();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedTile != null)
                selectedTile.TurnRight();
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
        CoalWagonController.offsetDistance = 1.2f;
        TrainController.l_wagons.Add(CoalWagonController);

        for (int x = 0; x < wagons; x++)
        {
            GameObject newWagon = Instantiate(WagonPrefab);
            TrainWagon WagonController = newWagon.GetComponent<TrainWagon>();
            WagonController.train = TrainController;
            WagonController.offsetDistance = x + 2;
            TrainController.l_wagons.Add(WagonController);
        }
    }

    private void MoveWithController(WorldTile.Rotation rotation)
    {
        if(ControllerInput == false)
        {
            ControllerInput = true;
            if(selectedTile == null)
            {
                selectedTile = TileManager.singleton.StartTile.NorthTile;
            }
        }
        if (!moved && selectedTile != null)
        {
            if (selectedTile.GetWorldTileByRotation(rotation) != null)
            {
                if (!selectedTile.GetWorldTileByRotation(rotation).locked)
                {
                    selectedTile.selected = false;
                    selectedTile.ResetGoalPosition();
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
                            selectedTile.ResetGoalPosition();
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

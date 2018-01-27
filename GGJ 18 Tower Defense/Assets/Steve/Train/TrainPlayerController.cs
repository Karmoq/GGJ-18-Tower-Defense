using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPlayerController : MonoBehaviour {
    [SerializeField] private WorldTile selectedTile;
    [SerializeField] private bool moved = false;
    [SerializeField] private bool turned = false;

    public void Start()
    {
        selectedTile.transform.position += Vector3.up * 0.5f;
    }

    public void FixedUpdate()
    {
        float x = Input.GetAxis("Joystick1Axis1");
        float y = Input.GetAxis("Joystick1Axis2");

        if (x > 0.5f)
            Move(WorldTile.Rotation.North);
        else if (x < -0.5f)
            Move(WorldTile.Rotation.South);
        else if (y > 0.5f)
            Move(WorldTile.Rotation.East);
        else if (y < -0.5f)
            Move(WorldTile.Rotation.West);
        else
            moved = false;

        float turn = Input.GetAxis("Joystick1Axis3");

        if (turn > 0.5f || turn < -0.5f)
            Turn(turn);
        else
            turned = false;
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

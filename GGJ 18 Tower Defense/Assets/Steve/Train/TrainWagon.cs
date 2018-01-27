using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWagon : MonoBehaviour {

    public Train train;

    public List<Vector3> TrainPath = new List<Vector3>();
    public float offset = 0;

    public void Update()
    {
        if (TrainPath.Count > (int)(offset*TileManager.singleton.PathIncrements))
        {
            if (Vector3.Distance(TrainPath[0], transform.position) > 0.05f)
            {
                Vector3 velocity = TrainPath[0] - transform.position;
                transform.position += Vector3.ClampMagnitude(velocity, train.currentSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(TrainPath[0] - transform.position);
            }
            else
            {
                TrainPath.RemoveAt(0);
            }
        }
    }
}

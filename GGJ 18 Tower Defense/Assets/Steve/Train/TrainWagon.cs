using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWagon : MonoBehaviour {

    public Train train;

    public List<Vector3> TrainPath = new List<Vector3>();
    public int offset = 0;

    public void Update()
    {
        if (TrainPath.Count > offset)
        {
            if (Vector3.Distance(TrainPath[0], transform.position) > 0.05f)
            {
                Vector3 velocity = TrainPath[0] - transform.position;
                transform.position += Vector3.ClampMagnitude(velocity, train.currentSpeed * Time.deltaTime);
            }
            else
            {
                TrainPath.RemoveAt(0);
            }
        }
    }
}

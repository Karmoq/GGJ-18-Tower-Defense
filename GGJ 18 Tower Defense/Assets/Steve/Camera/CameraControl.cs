using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    private Camera c_camera;

    public void Awake()
    {
        c_camera = GetComponent<Camera>();

        float orthoSize = Mathf.Max(TileManager.singleton.gridSize.x, TileManager.singleton.gridSize.y);

        transform.position = Vector3.up * orthoSize *0.75f;//new Vector3(-orthoSize/2, 10, -orthoSize/2);
        c_camera.orthographicSize = orthoSize*0.7f;
    }

    public void Update()
    {
        
    }
}

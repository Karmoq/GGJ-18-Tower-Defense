using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShipPlayerController : MonoBehaviour {

    [SerializeField]
    private float moveSpeed;

    private Vector3 movement;

    [SerializeField]
    private GameObject towerPrefab;

    public int maxTowerNumber = 5;
    private int currentTowerNumber;

    [SerializeField]
    private float towerLifeTime = 30.0f;

    [SerializeField]
    private float towerBetweenTime = 5.0f; //Time between a tower disappears and a new one is added back

    [SerializeField]
    private float windStrength = 1;
    public static float WindStrength;
    public static Vector3 WindDirection;

    [SerializeField]
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    private Rect movementRect;

    [SerializeField]
    private Vector3 TowerSpawnOffset = Vector3.zero;

    [SerializeField]
    private bool placedTower = true;

    void Start ()
    {
        currentTowerNumber = maxTowerNumber;
	}

    void Update()
    {
        WindStrength = windStrength;

        float xIn = Input.GetAxis("Joystick1Axis1") * moveSpeed;
        float yIn = Input.GetAxis("Joystick1Axis2") * moveSpeed;

        movement = Quaternion.Euler(0, 45, 0) * new Vector3(xIn, 0, -yIn);

        if(Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetAxis("Joystick1Axis3") != 0)
        {
            if (currentTowerNumber > 0 && !placedTower)
            {
                GameObject tower = Instantiate(towerPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                TowerController tc = tower.GetComponent<TowerController>();
                tc.SetTowerLifeTime(towerLifeTime);
                currentTowerNumber--;
                StartCoroutine(AddBackTower());
                placedTower = true;
            }
        }
        else
        {
            placedTower = false;
        }

        float xIn2 = Input.GetAxis("Joystick1Axis4") * moveSpeed;
        float yIn2 = Input.GetAxis("Joystick1Axis5") * moveSpeed;

        WindDirection = Quaternion.Euler(0, 45, 0) * new Vector3(xIn2, 0, -yIn2);
    }

	void FixedUpdate ()
    {
        //transform.position += movement;
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, transform.position + movement, ref velocity, smoothTime);

        if (transform.position.x > movementRect.x + movementRect.width)
        {
            targetPosition.x = movementRect.x + movementRect.width;
        }
        if (transform.position.x < movementRect.x)
        {
            targetPosition.x = movementRect.x;
        }
        if (transform.position.z > movementRect.y + movementRect.height)
        {
            targetPosition.z = movementRect.y + movementRect.height;
        }
        if (transform.position.z < movementRect.y)
        {
            targetPosition.z = movementRect.y;
        }
        transform.position = targetPosition;
    }

    IEnumerator AddBackTower()
    {
        yield return new WaitForSeconds(towerLifeTime + towerBetweenTime);
        currentTowerNumber++;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(new Vector3(movementRect.x+ movementRect.width/2, 5, movementRect.y+ movementRect.height/2), new Vector3(movementRect.width, 0, movementRect.height));
    }
}

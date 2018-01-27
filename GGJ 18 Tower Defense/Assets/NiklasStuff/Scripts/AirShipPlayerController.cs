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

	void Start ()
    {
        currentTowerNumber = maxTowerNumber;
	}

    void Update()
    {
        WindStrength = windStrength;

        float xIn = Input.GetAxis("Joystick2Axis1") * moveSpeed;
        float yIn = Input.GetAxis("Joystick2Axis2") * moveSpeed;

        movement = new Vector3(xIn, 0, -yIn);

        if(Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            if (currentTowerNumber > 0)
            {
                GameObject tower = Instantiate(towerPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                TowerController tc = tower.GetComponent<TowerController>();
                tc.SetTowerLifeTime(towerLifeTime);
                currentTowerNumber--;
                StartCoroutine(AddBackTower());
            }
        }

        float xIn2 = Input.GetAxis("Joystick2Axis4") * moveSpeed;
        float yIn2 = Input.GetAxis("Joystick2Axis5") * moveSpeed;

        WindDirection = new Vector3(xIn2, 0, -yIn2);
    }

	void FixedUpdate ()
    {
        transform.position += movement;
    }

    IEnumerator AddBackTower()
    {
        yield return new WaitForSeconds(towerLifeTime + towerBetweenTime);
        currentTowerNumber++;
    }
}

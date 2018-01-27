using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public bool paused = false;
    [SerializeField] private GameObject Menu;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick2Button7))
        {
            paused = !paused;
            Pause(paused);
        }
    }

    public void Pause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
        }
        else
            Time.timeScale = 1;
        OpenMenu(paused);
    }

    public void OpenMenu(bool open)
    {
        Menu.SetActive(open);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

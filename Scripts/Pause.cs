using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

    public GameObject pauseText;

    bool paused = false;

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0;
                Debug.Log("pause");
                pauseText.SetActive(true);
            }
            else if (paused)
            {
                paused = false;
                Time.timeScale = 1;
                Debug.Log("unpause");
                pauseText.SetActive(false);
            }
        }
	
	}
}

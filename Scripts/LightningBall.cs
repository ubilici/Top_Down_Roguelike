using UnityEngine;
using System.Collections;

public class LightningBall : MonoBehaviour {

	void Update () {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);	
	}
}

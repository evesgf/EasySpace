using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Transform target;
    public float RotateSpeed;
    public float Throttle;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (RotateSpeed != 0)
        {
            var angle = RotateSpeed * Time.deltaTime * 360f;
            transform.RotateAround(target.position, target.up, angle);
        }
	}
}

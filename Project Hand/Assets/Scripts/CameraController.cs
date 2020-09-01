using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    public float offsetX, offsetY, offsetZ, shakeDuration;

    private Vector3 newPos;
    private Vector3 velocity;

    void Start() {
        velocity = Vector3.zero;
    }

    void FixedUpdate() {
        newPos = new Vector3(target.transform.position.x + offsetX, offsetY, target.transform.position.z + offsetZ);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, 0.2f);
    }

    public void cameraShake() {
        StartCoroutine(Shake());
    }

    public IEnumerator Shake() {
        float timePassed = 0f;

        while(timePassed < shakeDuration) {
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(-0.5f, 0.5f);

            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
            timePassed += Time.deltaTime;
            yield return 0;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public float radius, frequency;
    public int enemyCount;

    public GameObject enemy;

	void Start () {
        enemyCount = 1;
        InvokeRepeating("Spawn", frequency, frequency);
	}

    private void Spawn() {
        if (enemyCount <= 10) {
            Vector2 points = Random.insideUnitCircle;
            points.Normalize();

            Instantiate(enemy, new Vector3(points.x * radius, 1.58f, points.y * radius), Quaternion.identity, this.transform);
            enemyCount++;
        }
    }

    public void decrementFrequency() {
        if (frequency >= 0.5f) {
            frequency -= 0.2f;
            CancelInvoke();
            InvokeRepeating("Spawn", frequency, frequency);
        }
    }

}

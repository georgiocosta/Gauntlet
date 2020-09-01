using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject player;
    public UIManager uimanager;
    AudioSource sfx;
    ParticleSystem particles;
    Animator animator;
    CameraController cameraController;
    EnemySpawner spawner;

    public float speed;

    public State state;
    public Material rockMat, paperMat, scissorsMat;

    private Vector3 target;

    Renderer[] rend;
    public Renderer mesh;

    void Start () {
        player = GameObject.FindWithTag("Player");
        uimanager = GameObject.FindWithTag("UI").GetComponent<UIManager>();
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        spawner = transform.parent.GetComponent<EnemySpawner>();

        sfx = GetComponent<AudioSource>();
        state = (State)Random.Range(2, 5);
        rend = GetComponentsInChildren<Renderer>();
        particles = GetComponent<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();

        animator.SetInteger("State", (int)state);

        if (state == State.ROCK) {
            rend[0].material = rockMat;
            rend[1].material = rockMat;
        } 
        else if (state == State.PAPER) {
            rend[0].material = paperMat;
            rend[1].material = paperMat;
        } 
        else if (state == State.SCISSORS) {
            rend[0].material = scissorsMat;
            rend[1].material = scissorsMat;
        }
    }
	
	void Update () {
        if(state == State.SCISSORS)
            target = player.transform.position + player.transform.forward;
        else if(state == State.ROCK)
            target = player.transform.position + player.transform.forward * -4;
        else if(state == State.PAPER)
            target = player.transform.position + player.transform.right * Random.Range(-1f,1f);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(player.transform, transform.up);
	}

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.GetComponent<Player>()) {
            if(state == State.ROCK && player.GetComponent<Player>().state == State.PAPER) {
                //RenderSettings.ambientLight = Color.green * 0.3f;
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, Color.green * 0.3f, 0.5f);
                Perish();
            }

            if (state == State.PAPER && player.GetComponent<Player>().state == State.SCISSORS) {
                //RenderSettings.ambientLight = Color.red;
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, Color.red, 0.5f);
                Perish();
            }

            if (state == State.SCISSORS && player.GetComponent<Player>().state == State.ROCK) {
                //RenderSettings.ambientLight = Color.blue;
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, Color.blue, 0.5f);
                Perish();
            }

            if ((state == State.PAPER && player.GetComponent<Player>().state == State.PAPER) ||
                    (state == State.SCISSORS && player.GetComponent<Player>().state == State.PAPER)) {
                Vector3 dir = other.gameObject.transform.position - transform.position;
                dir = -dir.normalized;
                GetComponent<Rigidbody>().AddForce(dir * 10, ForceMode.Impulse);
            }

            if(other.gameObject.CompareTag("Enemy")) {
                Vector3 dir = other.gameObject.transform.position - transform.position;
                dir = -dir.normalized;
                GetComponent<Rigidbody>().AddForce(dir * 10, ForceMode.Impulse);
            }
        }
    }

    private void Perish() {
        sfx.Play();
        particles.Play();
        cameraController.cameraShake();

        uimanager.SendMessage("addScore", 1);
        spawner.enemyCount--;
        GetComponent<BoxCollider>().enabled = false;
        rend[1].enabled = false;
        Invoke("Cease", 0.5f);
    }

    private void Cease() {
        Destroy(this.gameObject);
    }
}

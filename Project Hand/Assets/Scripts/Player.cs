using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    CharacterController controller;
    CameraController cameraController;
    Rigidbody rb;
    Renderer[] rend;
    AudioSource[] sfx;
    BoxCollider hitbox;
    SphereCollider sphereCol;
    ParticleSystem particles;
    Animator animator;
    public UIManager uimanager;
    public GameObject spawner, musicManager;

    public Material idleMat, rockMat, paperMat, scissorsMat;
    public Renderer mesh;

    public float speed, paperSpeed, scissorSpeed, cooldown;
    public int maxHp, hp;
    public State state, lastState;

    private bool hittable, canRock, canPaper, canScissors;

	void Start () {
        rb = GetComponent<Rigidbody>();
        sfx = GetComponents<AudioSource>();
        rend = GetComponentsInChildren<Renderer>();
        hitbox = GetComponent<BoxCollider>();
        sphereCol = GetComponent<SphereCollider>();
        particles = GetComponent<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();

        hp = maxHp;
        hittable = true;
        canRock = true;
        canPaper = true;
        canScissors = true;
        state = State.IDLE;
	}
	
	void Update () {
        rockPaperScissors();
        moveCharacter();
	}

    private void moveCharacter() {
        if (state == State.IDLE || state == State.WALK) {
            Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            playerInput = Vector3.ClampMagnitude(playerInput, 1);

            rb.AddForce(playerInput * speed, ForceMode.VelocityChange);

            if (playerInput != Vector3.zero) {
                if(rb.velocity.normalized != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
                //model.localScale = new Vector3(3, rb.velocity.magnitude / 3, 3);
                changeState(State.WALK);
            } else {
                changeState(State.IDLE);
            }
        
        }
    }

    private void rockPaperScissors() {
        if((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.B)) && canRock) {
            //rock
            changeState(State.ROCK);
            sfx[3].Play();
            mesh.material = rockMat;
            canRock = false;
            if (lastState == State.SCISSORS) {
                //CancelInvoke();
                rb.AddForce(transform.forward * scissorSpeed, ForceMode.VelocityChange);
                Invoke("endStateRS", 0.3f);
            }
            Invoke("endState", 0.3f);
            Invoke("rockCooldown", cooldown);
        }
        else if((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.N)) && canPaper) {
            //paper
            changeState(State.PAPER);
            sfx[4].Play();
            mesh.material = paperMat;
            canPaper = false;
            rb.velocity = Vector3.zero;
            sphereCol.enabled = true;

            Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            playerInput = Vector3.ClampMagnitude(playerInput, 1);

            if(playerInput == Vector3.zero)
                rb.AddForce(transform.forward * paperSpeed, ForceMode.VelocityChange);
            else {
                transform.rotation = Quaternion.LookRotation(playerInput);
                rb.AddForce(playerInput * paperSpeed, ForceMode.VelocityChange);
            }
            Invoke("endState", 0.2f);
            Invoke("paperCooldown", cooldown);
        }
        else if((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.M)) && canScissors) {
            //scissors
            changeState(State.SCISSORS);
            sfx[5].Play();
            mesh.material = scissorsMat;
            canScissors = false;
            rb.AddForce(transform.forward * scissorSpeed, ForceMode.VelocityChange);
            Invoke("endState", 0.2f);
            Invoke("scissorsCooldown", cooldown);
        }
    }

    private void changeState(State newState) {
        lastState = state;
        state = newState;
        animator.SetInteger("State", (int)state);
    }

    private void endState() {
        if (sphereCol.enabled == true)
            sphereCol.enabled = false;
        if (state == State.ROCK && lastState == State.SCISSORS)
            return;
        state = State.IDLE;
        mesh.material = idleMat;
    }

    private void endStateRS() {
        state = State.IDLE;
        mesh.material = idleMat;
    }

    private void OnTriggerStay(Collider other) {
        if ((state == State.IDLE || state == State.WALK) && hittable && other.CompareTag("Enemy")) {
            hittable = false;
            Invoke("hitboxCooldown", 0.5f);

            hp -= 1;
            uimanager.SendMessage("updateHp", hp);
            cameraController.cameraShake();

            sfx[0].Play();
            Vector3 dir = other.gameObject.transform.position - transform.position;
            dir = -dir.normalized;
            rb.AddForce(dir * 10, ForceMode.Impulse);
        }
        else if(other.CompareTag("Enemy")) {
            if(musicManager.GetComponents<AudioSource>()[2].volume == 0f)
                musicManager.GetComponents<AudioSource>()[2].volume = 1f;
        }
    }

    private void OnTriggerExit(Collider other) {

        if(other.CompareTag("Boundary")) {
            float velocityMagnitude = rb.velocity.magnitude;

            Vector3 dir = other.gameObject.transform.position - transform.position;
            rb.velocity = Vector3.zero;    

            dir = new Vector3(dir.x, transform.position.y, dir.z);
            dir = dir.normalized;

            rb.AddForce(dir * velocityMagnitude, ForceMode.Impulse);

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

        }

        if(other.CompareTag("Zone") && !spawner.activeInHierarchy) {
            spawner.SetActive(true);
            sfx[1].Play();
            musicManager.GetComponents<AudioSource>()[1].volume = 1f;
        }

    }

    private void hitboxCooldown() {
        hittable = true;
    }

    private void rockCooldown() {
        canRock = true;
    }

    private void paperCooldown() {
        canPaper = true;
    }

    private void scissorsCooldown() {
        canScissors = true;
    }

    private void Die() {
        mesh.enabled = false;
        hitbox.enabled = false;
        changeState(State.DEAD);
        sfx[2].Play();
        particles.Play();
    }
}

public enum State {
    IDLE,
    WALK,
    ROCK,
    PAPER,
    SCISSORS,
    DEAD
}

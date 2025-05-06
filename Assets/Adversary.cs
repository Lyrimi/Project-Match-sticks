using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adversary : MonoBehaviour
{
    public GameObject enemy;
    public Collider2D cone1;
    public Collider2D cone2;
    public float accel;
    public float turnSpeed;
    public float angulDecay;
    public float friction;
    public GameObject DEBUG_FACING;

    bool enemyInSight = false;
    float rotation = 0;
    float angulVel = 0;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (angulVel != 0) {
            rotation += angulVel*Time.fixedDeltaTime;
            if (rotation >= Mathf.PI) {
                rotation -= Mathf.PI*2;
            } else if (rotation < -Mathf.PI) {
                rotation += Mathf.PI*2;
            }
            float angulVelChange = angulDecay*Time.fixedDeltaTime;
            if (Mathf.Abs(angulVel) <= angulVelChange) {
                angulVel = 0;
            } else {
                angulVel += angulVelChange * -Mathf.Sign(angulVel);
            }
        }
        DEBUG_FACING.transform.position = transform.position+new Vector3(Mathf.Cos(rotation), Mathf.Sin(rotation), 0)*.3f;
        if (enemyInSight) {
            float angleDiff = Mathf.Atan2(enemy.transform.position.y-transform.position.y, enemy.transform.position.x-transform.position.x)-rotation;
            if (angleDiff <= -Mathf.PI) {
                angleDiff += Mathf.PI*2;
            } else if (angleDiff > Mathf.PI) {
                angleDiff -= Mathf.PI*2;
            }
            angulVel += (Mathf.Sign(angleDiff))*turnSpeed;
            rb.AddForce(new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized*accel);
        }
        rb.AddForce(-rb.velocity*friction);
        Quaternion q = Quaternion.Euler(0, 0, rotation/Mathf.PI*180);
        cone1.transform.rotation = q;
        cone2.transform.rotation = q;
    }

    public void VisionUpdate(Boolean spotted) {
        enemyInSight = spotted;
    }
}

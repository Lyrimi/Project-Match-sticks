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
    public float maxSpeed;
    public float maxSpeedFriction;
    public float turnAccel;
    public float maxTurnSpeed;
    public float angulFriction;
    public float friction;
    public int lostSightChaseTime;
    public int animLostSightInitDelay;
    public int animLostSightTurnTime;
    public float animLostSightTurnFactor;
    public int animLostSightTurnDelay;
    public int animLostSightEndDelay;
    public GameObject DEBUG_FACING;
    public GameObject DEBUG_CHASE;

    bool enemyInSight = false;
    int enemyLastSeen = 0x7FFFFFFF;
    int animLostSightTimer = 0x7FFFFFFF;
    Vector2 enemyLastPosition;
    Vector2 enemyLastVel;
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
        }

        Color DEBUG_COLOR;

        Vector2 movement;
        float movMagnitude;
        float rotateMovement = 0;
        if (enemyInSight) {
            float angleDiff = Mathf.Atan2(enemy.transform.position.y-transform.position.y, enemy.transform.position.x-transform.position.x)-rotation;
            if (angleDiff <= -Mathf.PI) {
                angleDiff += Mathf.PI*2;
            } else if (angleDiff > Mathf.PI) {
                angleDiff -= Mathf.PI*2;
            }
            rotateMovement = (Mathf.Sign(angleDiff));
            movement = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized;
            movMagnitude = 1;
            enemyLastSeen = 0;
            animLostSightTimer = 0;
            DEBUG_COLOR = new Color(.5f, 0, 0);
        } else {
            if (enemyLastSeen < lostSightChaseTime) {
                if (enemyLastSeen == 0) {
                    enemyLastPosition = enemy.transform.position;
                    enemyLastVel = enemy.GetComponent<Rigidbody2D>().velocity;
                }
                enemyLastSeen++;
                
                Vector2 predictedPosition = enemyLastPosition+enemyLastVel*enemyLastSeen*Time.fixedDeltaTime;
                float angleDiff = Mathf.Atan2(predictedPosition.y-transform.position.y, predictedPosition.x-transform.position.x)-rotation;
                if (angleDiff <= -Mathf.PI) {
                    angleDiff += Mathf.PI*2;
                } else if (angleDiff > Mathf.PI) {
                    angleDiff -= Mathf.PI*2;
                }
                
                rotateMovement = (Mathf.Sign(angleDiff));
                movement = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized;
                movMagnitude = 1;
                DEBUG_COLOR = new Color(0, 0, .5f);

                DEBUG_CHASE.transform.position = predictedPosition;
            } else if (animLostSightTimer < animLostSightInitDelay+animLostSightTurnTime*4+animLostSightTurnDelay*2+animLostSightEndDelay) {
                if (animLostSightTimer < animLostSightInitDelay) {
                } else if (animLostSightTimer < animLostSightInitDelay+animLostSightTurnTime) {
                    rotateMovement = animLostSightTurnFactor;
                } else if (animLostSightTimer < animLostSightInitDelay + animLostSightTurnTime + animLostSightTurnDelay) {
                } else if (animLostSightTimer < animLostSightInitDelay + animLostSightTurnTime*3 + animLostSightTurnDelay) {
                    rotateMovement = -animLostSightTurnFactor;
                } else if (animLostSightTimer < animLostSightInitDelay + animLostSightTurnTime*3 + animLostSightTurnDelay*2) {
                } else if (animLostSightTimer < animLostSightInitDelay + animLostSightTurnTime*4 + animLostSightTurnDelay*2) {
                    rotateMovement = animLostSightTurnFactor;
                }
                animLostSightTimer++;
                
                movement = Vector2.zero;
                movMagnitude = 0;
                DEBUG_COLOR = new Color(.5f, .5f, 0);
            } else {
                movement = Vector2.zero;
                movMagnitude = 0;
                DEBUG_COLOR = new Color(.5f, .5f, .5f);
            }
        }

        GetComponentInChildren<SpriteRenderer>().color = DEBUG_COLOR;
        
        DEBUG_FACING.transform.position = transform.position+new Vector3(Mathf.Cos(rotation), Mathf.Sin(rotation), 0)*.3f;
        rb.AddForce(movement*accel);
        float prevAngulVel = angulVel;
        angulVel = rotateMovement*turnAccel;

        
        float angulVelChange = angulFriction*Time.fixedDeltaTime;
        if (Mathf.Abs(angulVel) <= angulVelChange) {
            angulVel = 0;
        } else {
            angulVel += angulVelChange * -Mathf.Sign(angulVel);
        }

        float magnitude = rb.velocity.magnitude;
        if (magnitude != 0) {
            float dot = (rb.velocity.x*movement.x+rb.velocity.y*movement.y)/(magnitude*movMagnitude);
            if (dot <= 0) {
                rb.AddForce(-rb.velocity * friction);
            } else {
                Vector2 vel = rb.velocity-movement*magnitude;
                rb.AddForce(-vel*friction);
                float speed = movMagnitude*magnitude;
                if (speed > maxSpeed) {
                    rb.AddForce(-movement/movMagnitude*(speed-maxSpeed)*maxSpeedFriction);
                }
            }
        }

        if (angulVel > maxTurnSpeed) {
            if (maxTurnSpeed < prevAngulVel) {
                angulVel = prevAngulVel;
            } else {
                angulVel = maxTurnSpeed;
            }
        }

        Quaternion q = Quaternion.Euler(0, 0, rotation/Mathf.PI*180);
        cone1.transform.rotation = q;
        cone2.transform.rotation = q;
    }

    public void VisionUpdate(Boolean spotted) {
        enemyInSight = spotted;
    }
}

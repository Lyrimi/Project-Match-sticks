using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal.Internal;

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
    public float smallTurnThreshold;
    public float angulFriction;
    public float friction;
    public int lostSightChaseTime;
    public int animLostSightInitDelay;
    public int animLostSightTurnTime;
    public float animLostSightTurnFactor;
    public int animLostSightTurnDelay;
    public int animLostSightEndDelay;
    public int wanderTimerMin;
    public int wanderTimerMax;
    public int wanderAdjustTimerMin;
    public int wanderAdjustTimerMax;
    public int wanderAdjustTimerMinRemaining;
    public int wanderDelayMin;
    public int wanderDelayMax;
    public float wanderAdjustMinAngle;
    public float wanderAdjustMaxAngle;
    public float wanderMoveFactor;
    public float wanderTurnFactor;
    public float headHomeDistance;
    public GameObject visionLight;
    public float visionLightDistance;

    Vector2 home;

    PlayerTest enemyScript;
    bool enemyHiding;

    bool enemyInSight = false;
    int enemyLastSeen = 0x7FFFFFFF;
    int animLostSightTimer = 0x7FFFFFFF;
    Vector2 enemyLastPosition;
    Vector2 enemyLastVel;
    int wanderTimer;
    int wanderAdjustTimer;
    int wanderDelay;
    float wanderAngle;
    const float WANDER_DEFLECT_NO_DEFLECT = -9999f;
    float wanderDeflectAngle = WANDER_DEFLECT_NO_DEFLECT;

    float rotation = 0;
    float angulVel = 0;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemyScript = enemy.GetComponent<PlayerTest>();

        home = transform.position;

        wanderTimer = UnityEngine.Random.Range(wanderTimerMin, wanderTimerMax+1);
        wanderAdjustTimer = UnityEngine.Random.Range(wanderAdjustTimerMin, wanderAdjustTimerMax+1);
        if (wanderTimer-wanderAdjustTimer < wanderAdjustTimerMinRemaining) {
            wanderAdjustTimer = 0x7FFFFFFF;
        }
        wanderAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
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

        Vector2 movement;
        float movMagnitude;
        float rotateMovement = 0;
        if (enemyScript.isHidden()) {
            if (enemyInSight) {
                enemyInSight = false;
                enemyHiding = true;
                enemyLastSeen = 0x7FFFFFFF;
            }
        } else {
            enemyHiding = false;
        }
        if (enemyInSight) {
            float angleDiff = Mathf.Atan2(enemy.transform.position.y-transform.position.y, enemy.transform.position.x-transform.position.x)-rotation;
            if (angleDiff <= -Mathf.PI) {
                angleDiff += Mathf.PI*2;
            } else if (angleDiff > Mathf.PI) {
                angleDiff -= Mathf.PI*2;
            }
            rotateMovement = Mathf.Abs(angleDiff)<smallTurnThreshold?angleDiff/smallTurnThreshold:Mathf.Sign(angleDiff);
            movement = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized;
            movMagnitude = 1;
            enemyLastSeen = 0;
            animLostSightTimer = 0;
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
                
                rotateMovement = Mathf.Abs(angleDiff)<smallTurnThreshold?angleDiff/smallTurnThreshold:Mathf.Sign(angleDiff);
                movement = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized;
                movMagnitude = 1;
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
                if (animLostSightTimer == animLostSightInitDelay + animLostSightTurnTime * 4 + animLostSightTurnDelay * 2 + animLostSightEndDelay) {
                    SetupWanderTimer();
                    wanderDelay = 0;
                    wanderDeflectAngle = WANDER_DEFLECT_NO_DEFLECT;
                    wanderAngle = GetWanderAngle();
                }
                
                movement = Vector2.zero;
                movMagnitude = 0;
            } else {
                if (wanderDeflectAngle != WANDER_DEFLECT_NO_DEFLECT) {
                    wanderAngle = wanderDeflectAngle;
                    wanderDeflectAngle = WANDER_DEFLECT_NO_DEFLECT;
                    SetupWanderTimer();
                    wanderDelay = 0;
                }
                if (wanderDelay > 0) {
                    wanderDelay--;
                    if (wanderDelay == 0) {
                        SetupWanderTimer();
                        wanderAngle = GetWanderAngle();
                    }
                    movement = Vector2.zero;
                    movMagnitude = 0;
                } else {
                    if (wanderAdjustTimer == 0) {
                        wanderAngle += UnityEngine.Random.Range(wanderAdjustMinAngle, wanderAdjustMaxAngle)*(UnityEngine.Random.Range(0, 2)*2-1);
                        wanderAdjustTimer = UnityEngine.Random.Range(wanderAdjustTimerMin, wanderAdjustTimerMax+1);
                        if (wanderTimer-1-wanderAdjustTimer < wanderAdjustTimerMinRemaining) {
                            wanderAdjustTimer = 0x7FFFFFFF;
                        }
                    }
                    movement = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized*wanderMoveFactor;
                    movMagnitude = wanderMoveFactor;
                    float angleDiff = wanderAngle-rotation;
                    if (angleDiff <= -Mathf.PI) {
                        angleDiff += Mathf.PI*2;
                    } else if (angleDiff > Mathf.PI) {
                        angleDiff -= Mathf.PI*2;
                    }
                    if (angleDiff != 0) {
                        rotateMovement = Mathf.Abs(angleDiff)*wanderTurnFactor<smallTurnThreshold?angleDiff*wanderTurnFactor/smallTurnThreshold:Mathf.Sign(angleDiff)*wanderTurnFactor;
                    }
                    wanderAdjustTimer--;
                    wanderTimer--;
                    if (wanderTimer == 0) {
                        wanderDelay = UnityEngine.Random.Range(wanderDelayMin, wanderDelayMax+1);
                    }
                }
            }
        }
        
        visionLight.transform.position = transform.position+new Vector3(Mathf.Cos(rotation), Mathf.Sin(rotation), 0)*visionLightDistance;
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

        Quaternion q = Quaternion.Euler(0, 0, rotation*Mathf.Rad2Deg);
        cone1.transform.rotation = q;
        cone2.transform.rotation = q;
        visionLight.transform.rotation = q;
    }

    public void SetupWanderTimer() {
        wanderTimer = UnityEngine.Random.Range(wanderTimerMin, wanderTimerMax+1);
        wanderAdjustTimer = UnityEngine.Random.Range(wanderAdjustTimerMin, wanderAdjustTimerMax+1);
        if (wanderTimer-wanderAdjustTimer < wanderAdjustTimerMinRemaining) {
            wanderAdjustTimer = 0x7FFFFFFF;
        }
    }

    public float GetWanderAngle() {
        Vector2 vec2Home = home-(Vector2) transform.position;
        if (vec2Home.magnitude >= headHomeDistance) {
            return Mathf.Atan2(vec2Home.y, vec2Home.x);
        } else {
            return UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        }
    }

    public bool IsEnemyHiding() {
        return enemyHiding;
    }

    public void VisionUpdate(Boolean spotted) {
        if (enemyScript.isHidden()) {
            enemyHiding = spotted;
            return;
        }
        enemyInSight = spotted;
    }

	private void OnCollisionEnter2D(Collision2D collision) {
        if (!enemyInSight && enemyLastSeen >= lostSightChaseTime && animLostSightTimer >= animLostSightInitDelay+animLostSightTurnTime*4+animLostSightTurnDelay*2+animLostSightEndDelay) {
		    wanderTimer = 0;
            Vector2 normal = collision.contacts[0].normal;
            wanderDeflectAngle = Mathf.Atan2(normal.y, normal.x);
        }
	}
}

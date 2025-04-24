using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float velX = Input.GetAxisRaw("Horizontal");
        float velY = Input.GetAxisRaw("Vertical");
        Vector2 vel = new Vector2(velX, velY);
        float length = vel.magnitude;
        if (length > 1) {
            vel /= length;
        }
        rb.velocity = vel*speed;
    }

    public Vector2 RandomPointInRingMath(float lowerRadius, float upperRadius, float minRadians, float maxRadians) {
        float angle = Random.Range(minRadians, maxRadians);
        float squareLength = Random.Range(Mathf.Pow(lowerRadius, 2), Mathf.Pow(upperRadius, 2));
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))*Mathf.Sqrt(squareLength);
    }
    
    public Vector2 RandomPointInRing(float lowerRadius, float upperRadius) {
        Random.InitState(10);
        return RandomPointInRingMath(lowerRadius, upperRadius, 0, Mathf.PI*2);
    }
}

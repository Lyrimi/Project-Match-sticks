using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeDetection : MonoBehaviour
{
    public GameObject enemy;
    public bool exit;
    
    Adversary owner;

	void Start() {
		owner = GetComponentInParent<Adversary>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
        if (!exit && collision.gameObject) {
            owner.VisionUpdate(true);
        }
	}

	private void OnTriggerExit2D(Collider2D collision) {
        if (exit && collision.gameObject) {
            owner.VisionUpdate(false);
        }
	}
}

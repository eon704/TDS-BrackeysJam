using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float speed;

#pragma warning disable CS0108, CS0114
    private Rigidbody rigidbody;
#pragma warning restore CS0108, CS0114

    void Awake() {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void FixedUpdate() {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xMovement, 0f, yMovement).normalized;
        movement *= this.speed * Time.deltaTime;
        this.rigidbody.MovePosition(this.transform.position + movement);
    }
}

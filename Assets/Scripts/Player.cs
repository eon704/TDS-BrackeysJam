using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {
    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private float attackDistance;

    private int health;
    private MeleeCombatController meleeCombatController;

#pragma warning disable CS0108, CS0114
    private Rigidbody rigidbody;
#pragma warning restore CS0108, CS0114

    void Awake() {
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.meleeCombatController = this.GetComponent<MeleeCombatController>();
    }

    void Start() {
        this.health = this.maxHealth;
    }

    void FixedUpdate() {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xMovement, 0f, yMovement).normalized;
        movement *= this.speed * Time.deltaTime;
        this.rigidbody.MovePosition(this.transform.position + movement);
    }

    void OnDrawGizmos() {
        Handles.DrawWireDisc(this.transform.position, Vector3.up, this.attackDistance);
    }

    public int GetHealth() {
        return this.health;
    }

    public void TakeDamage(int damage) {
        this.health -= damage;
        if (this.health <= 0) {
            Debug.Log("Player Died");
        }
    }
}

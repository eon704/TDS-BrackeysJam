using System;
using Interfaces;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float lifetime;
    [SerializeField] private GameObject hitEffect;

    private float currentLifetime;
    private LayerMask mask;
    private int damage;

    void Start() {
        this.currentLifetime = Time.time;
    }

    void Update() {
        if (Time.time - this.currentLifetime >= this.lifetime) {
            Destroy(this.gameObject);
        }
    }

    public void SetLayerMask(LayerMask newMask) {
        this.mask = newMask;
    }

    public void SetDamage(int newDamage) {
        this.damage = newDamage;
    }

    private void OnTriggerEnter(Collider collider) {
        if (this.mask == (this.mask | 1 << collider.gameObject.layer)) {
            GameObject hitEffectObj = Instantiate(this.hitEffect,
                                                  this.transform.position,
                                                  Quaternion.identity);

            IDamageable target = collider.gameObject.GetComponent<IDamageable>();
            target?.TakeDamage(this.damage);
            Destroy(hitEffectObj, 5f);
            Destroy(this.gameObject);
        }
    }
}

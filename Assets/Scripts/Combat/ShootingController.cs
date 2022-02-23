using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEditor;
using UnityEngine;

public class ShootingController : MonoBehaviour {
  [SerializeField] private int damage;
  [SerializeField] private int attackCooldown;
  [SerializeField] private LayerMask mask;

  private Coroutine attackingRoutine;
  // Connect to animator

  public void Attack(Vector3 origin, Vector3 direction) {
    bool onCooldown = this.attackingRoutine != null;

    if (!onCooldown) {
      this.attackingRoutine = this.StartCoroutine(this.StartAttack(origin, direction));
    }
  }

  private IEnumerator StartAttack(Vector3 origin, Vector3 direction) {
    bool hit = Physics.Raycast(origin, direction, out RaycastHit hitInfo, 500f, this.mask);
    if (hit) {
      IDamageable target = hitInfo.collider.GetComponent<IDamageable>();
      target?.TakeDamage(this.damage);
    }

    yield return new WaitForSeconds(this.attackCooldown);
    this.attackingRoutine = null;
  }
}

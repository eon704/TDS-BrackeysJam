using System.Collections;
using Interfaces;
using UnityEngine;

public class MeleeCombatController : MonoBehaviour {
  [SerializeField] private int damage;
  [SerializeField] private int attackCooldown;

  private Coroutine attackingRoutine;

  public void Attack(IDamageable target) {
    bool onCooldown = this.attackingRoutine != null;

    if (onCooldown) {
      this.StartCoroutine(this.StartAttack(target));
    }
  }

  private IEnumerator StartAttack(IDamageable target) {
    target.TakeDamage(this.damage);
    yield return new WaitForSeconds(this.attackCooldown);
    this.attackingRoutine = null;
  }
}

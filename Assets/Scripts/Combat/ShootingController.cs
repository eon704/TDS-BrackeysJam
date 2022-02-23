using System.Collections;
using UnityEngine;

public class ShootingController : MonoBehaviour {
  [SerializeField] private int damage;
  [SerializeField] private float projectileSpeed;
  [SerializeField] private int attackCooldown;
  [SerializeField] private LayerMask mask;
  [SerializeField] private GameObject projectilePrefab;
  [SerializeField] private Transform projectileSource;
  [SerializeField] private AudioSource audioSource;

  private Coroutine attackingRoutine;

  private static int _counter;
  // Connect to animator

  public void Attack() {
    _counter = 0;
    bool onCooldown = this.attackingRoutine != null;

    Debug.Log($"Attacking, onCooldown: {onCooldown}");
    if (!onCooldown) {
      this.attackingRoutine = this.StartCoroutine(this.StartAttack());
    }
  }

  private IEnumerator StartAttack() {
    this.audioSource.Play();
    GameObject projectileObj = Instantiate(this.projectilePrefab,
                                           this.projectileSource.position,
                                           this.projectileSource.rotation);

    projectileObj.name = $"Projectile {_counter}";
    Projectile projectile = projectileObj.GetComponent<Projectile>();
    projectile.SetLayerMask(this.mask);
    projectile.SetDamage(this.damage);
    Rigidbody rigidbody = projectileObj.GetComponent<Rigidbody>();
    rigidbody.AddForce(this.projectileSource.forward * this.projectileSpeed, ForceMode.Impulse);

    yield return new WaitForSeconds(this.attackCooldown);
    this.attackingRoutine = null;
  }
}

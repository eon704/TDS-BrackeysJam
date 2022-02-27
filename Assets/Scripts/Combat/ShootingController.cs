using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Combat {
  public class ShootingController : MonoBehaviour {
    public UnityAction<float> OnShot;

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

      if (!onCooldown) {
        this.attackingRoutine = this.StartCoroutine(this.StartAttack());
      }
    }

    private IEnumerator StartAttack() {
      this.audioSource.Play();
      GameObject projectileObj = Instantiate(this.projectilePrefab,
                                             this.projectileSource.position,
                                             this.projectileSource.rotation);

      projectileObj.name = $"Projectile {_counter++}";
      Projectile projectile = projectileObj.GetComponent<Projectile>();
      projectile.SetLayerMask(this.mask);
      projectile.SetDamage(this.damage);
      Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
      rb.AddForce(this.projectileSource.forward * this.projectileSpeed, ForceMode.Impulse);

      this.OnShot?.Invoke(this.attackCooldown);

      yield return new WaitForSeconds(this.attackCooldown);
      this.attackingRoutine = null;
    }
  }
}

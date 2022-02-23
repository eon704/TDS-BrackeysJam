using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable {
  public enum Type {
    Zombie,
    Ghost
  }

  [SerializeField] private Type type;
  [SerializeField] private int maxHealth;
  [SerializeField] private float attackDistance;

  private int health;
  private Player player;
  private MeleeController meleeController;
  private NavMeshAgent navMeshAgent;

#pragma warning disable CS0108, CS0114
  private Rigidbody rigidbody;
#pragma warning restore CS0108, CS0114

  void Awake() {
    this.player = FindObjectOfType<Player>();
    this.rigidbody = this.GetComponent<Rigidbody>();
    this.meleeController = this.GetComponent<MeleeController>();
    this.navMeshAgent = this.GetComponent<NavMeshAgent>();
  }

  void Start() {
    this.health = this.maxHealth;
  }

  void FixedUpdate() {
    if (this.type == Type.Zombie) {
      this.UpdateZombie();
    }
  }

  private void UpdateZombie() {
    Vector3 position = this.transform.position;
    Vector3 directionTowardsPlayer = this.player.transform.position - position;

    if (directionTowardsPlayer.magnitude <= this.attackDistance) {
      // Attack
      this.meleeController.Attack(this.player);
    } else {
      // Move towards Player
      this.navMeshAgent.SetDestination(this.player.transform.position);
    }
  }

  private void UpdateGhost() { }

  void OnDrawGizmos() {
    Handles.DrawWireDisc(this.transform.position, Vector3.up, this.attackDistance);
  }

  public int GetHealth() {
    return this.health;
  }

  public void TakeDamage(int damage) {
    Debug.Log("Taking damage");
    this.health -= damage;
    if (this.health <= 0) {
      Destroy(gameObject);
    }
  }
}

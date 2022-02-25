using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using View;

public class Enemy : MonoBehaviour, IDamageable {
  private enum Type {
    Zombie,
    Ghost
  }

  public UnityAction<int> OnHealthChanged { get; set; }
  public UnityAction OnDeath { get; set; }

  public int MaxHealth => this.maxHealth;

  [SerializeField] private Type type;
  [SerializeField] private int maxHealth;
  [SerializeField] private float attackDistance;

  private int health;
  private Player player;
  private MeleeController meleeController;
  private NavMeshAgent navMeshAgent;

  void Awake() {
    this.player = FindObjectOfType<Player>();
    this.meleeController = this.GetComponent<MeleeController>();
    this.navMeshAgent = this.GetComponent<NavMeshAgent>();
  }

  void Start() {
    this.health = this.maxHealth;
    GameUI.Instance.InstantiateNewHealthBar(this, this.transform);
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

  public void TakeDamage(int damage) {
    this.health -= damage;
    if (this.health <= 0) {
      this.OnDeath?.Invoke();
      Destroy(this.gameObject);
    }

    this.OnHealthChanged?.Invoke(this.health);
  }
}

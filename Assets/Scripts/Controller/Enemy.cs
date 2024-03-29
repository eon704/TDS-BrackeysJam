using Combat;
using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using View;

namespace Controller {
  public class Enemy : MonoBehaviour, IDamageable {
    public enum State {
      Zombie,
      Ghost
    }

    public UnityAction<int> OnHealthChanged { get; set; }
    public UnityAction<IDamageable> OnDeath { get; set; }
    public UnityAction OnReset { get; set; }

    public int MaxHealth => this.maxHealth;

    [SerializeField] private int maxHealth;
    [SerializeField] private float meleeAttackDistance;
    [SerializeField] private float rangedAttackDistance;
    [SerializeField] private Color ghostColor;
    [SerializeField] private Color zombieColor;

    private int health;
    private Player player;
    private MeleeController meleeController;
    private ShootingController shootingController;
    private NavMeshAgent navMeshAgent;
    private State state;
    private float attackDistance;
    private MeshRenderer meshRenderer;

    void Awake() {
      this.meleeController = this.GetComponent<MeleeController>();
      this.shootingController = this.GetComponent<ShootingController>();
      this.navMeshAgent = this.GetComponent<NavMeshAgent>();
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

    void Start() {
      this.health = this.maxHealth;
      GameUI.Instance.InstantiateNewHealthBar(this, this.transform);

      this.SetState(State.Ghost);
    }

    void Update() {
      if (this.player == null) {
        return;
      }

      this.transform.LookAt(this.player.transform, Vector3.up);
    }

    void FixedUpdate() {
      if (this.player == null) {
        return;
      }

      Vector3 position = this.transform.position;
      Vector3 directionTowardsPlayer = this.player.transform.position - position;

      if (directionTowardsPlayer.magnitude <= this.attackDistance) {
        // Attack
        this.Attack();
        this.navMeshAgent.SetDestination(this.transform.position);
      } else {
        // Move towards Player
        this.navMeshAgent.SetDestination(this.player.transform.position);
      }
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
      Handles.color = Color.red;
      Handles.DrawWireDisc(this.transform.position, Vector3.up, this.attackDistance);
    }
    #endif

    public void AssignPlayer(Player newPlayer) {
      this.player = newPlayer;
    }

    public void TakeDamage(int damage) {
      this.health -= damage;
      if (this.health <= 0) {
        this.OnDeath?.Invoke(this);
        Destroy(this.gameObject);
      }

      this.OnHealthChanged?.Invoke(this.health);
    }

    public void SetState(State newState) {
      this.state = newState;
      switch (newState) {
        case State.Zombie:
          this.attackDistance = this.meleeAttackDistance;
          this.meshRenderer.material.color = this.zombieColor;
          this.gameObject.layer = LayerMask.NameToLayer("Zombie");
          break;
        case State.Ghost:
          this.attackDistance = this.rangedAttackDistance;
          this.meshRenderer.material.color = this.ghostColor;
          this.gameObject.layer = LayerMask.NameToLayer("Ghost");
          break;
      }
    }

    public void Reset() {
      this.OnReset?.Invoke();
      Destroy(this.gameObject);
    }

    private void Attack() {
      if (this.state == State.Zombie) {
        this.meleeController.Attack(this.player);
      } else if (this.state == State.Ghost) {
        this.shootingController.Attack();
      }
    }
  }
}

using System.Security.Cryptography;
using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using View;

namespace Controller {
  public class Player : MonoBehaviour, IDamageable {
    public UnityAction<int> OnHealthChanged { get; set; }
    public UnityAction<IDamageable> OnDeath { get; set; }

    public int MaxHealth => this.maxHealth;

    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private float attackDistance;

    private int health;
    private ShootingController shootingController;
    private Camera mainCamera;

#pragma warning disable CS0108, CS0114
    private Rigidbody rigidbody;
#pragma warning restore CS0108, CS0114

    void Awake() {
      this.rigidbody = this.GetComponent<Rigidbody>();
      this.shootingController = this.GetComponent<ShootingController>();
      this.mainCamera = Camera.main;
    }

    void Start() {
      this.health = this.maxHealth;
      GameUI.Instance.InstantiateNewHealthBar(this, this.transform);
    }

    void Update() {
      Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);
      bool hit = Physics.Raycast(ray, out RaycastHit hitInfo,
                                 100f, LayerMask.GetMask("Floor"));

      if (hit) {
        Vector3 lookTarget = new Vector3(hitInfo.point.x, this.transform.position.y, hitInfo.point.z);
        this.transform.LookAt(lookTarget);
      }

      if (Input.GetMouseButtonDown(0)) {
        this.shootingController.Attack();
      }

      Debug.DrawRay(this.transform.position, this.transform.forward * 2, Color.red);
    }

    void FixedUpdate() {
      float xMovement = Input.GetAxis("Horizontal");
      float yMovement = Input.GetAxis("Vertical");

      Vector3 movement = new Vector3(xMovement, 0f, yMovement);
      if (movement.magnitude > 1f) {
        movement = movement.normalized;
      }

      movement *= this.speed * Time.deltaTime;
      this.rigidbody.MovePosition(this.transform.position + movement);
    }

    void OnDrawGizmos() {
      Handles.DrawWireDisc(this.transform.position, Vector3.up, this.attackDistance);
    }

    public void TakeDamage(int damage) {
      this.health -= damage;
      if (this.health <= 0) {
        this.Death();
      }

      this.OnHealthChanged?.Invoke(this.health);
    }

    public void GameWon() {
      this.Death();
    }

    private void Death() {
      this.OnDeath?.Invoke(this);
      Destroy(this.gameObject);
    }
  }
}

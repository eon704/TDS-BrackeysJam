using Interfaces;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {
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

    Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
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

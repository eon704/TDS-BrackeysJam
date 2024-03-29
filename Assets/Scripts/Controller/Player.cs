using System.Collections;
using Combat;
using Interfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using View;

namespace Controller {
  public class Player : MonoBehaviour, IDamageable {
    public UnityAction<int> OnHealthChanged { get; set; }
    public UnityAction<IDamageable> OnDeath { get; set; }

    public UnityAction OnReset { get; set; }

    public UnityAction<float> OnSpellCasted;

    public int MaxHealth => this.maxHealth;

    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private float attackDistance;
    [SerializeField] private float spellDistance;
    [SerializeField] private float spellCooldown;
    [SerializeField] private GameObject blastPrefab;

    private int health;
    private ShootingController shootingController;
    private Camera mainCamera;
    private bool lockInput;
    private new Rigidbody rigidbody;

    private Coroutine fireballRoutine;
    private Coroutine spellRoutine;

    void Awake() {
      this.rigidbody = this.GetComponent<Rigidbody>();
      this.shootingController = this.GetComponent<ShootingController>();
      this.mainCamera = Camera.main;
    }

    void Start() {
      this.health = this.maxHealth;
      this.lockInput = false;
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

      Debug.DrawRay(this.transform.position, this.transform.forward * 2, Color.red);

      if (this.lockInput) {
        return;
      }

      if (Input.GetMouseButtonDown(0)) {
        this.shootingController.Attack();
      }

      if (Input.GetMouseButtonDown(1) && this.spellRoutine == null) {
        this.spellRoutine = this.StartCoroutine(this.CastMaterializationSpell());
      }
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

    #if UNITY_EDITOR
    void OnDrawGizmos() {
      Handles.color = Color.blue;
      Handles.DrawWireDisc(this.transform.position, Vector3.up, this.attackDistance);
      Handles.color = Color.red;
      Handles.DrawWireDisc(this.transform.position, Vector3.up, this.spellDistance);
    }
    #endif

    public void TakeDamage(int damage) {
      this.health -= damage;
      if (this.health <= 0) {
        this.Death();
      }

      this.OnHealthChanged?.Invoke(this.health);
    }

    public void GameWon() {
      this.lockInput = true;
    }

    public void ListenToShots(UnityAction<float> action) {
      this.shootingController.OnShot += action;
    }

    public void Reset() {
      this.OnReset?.Invoke();
      Destroy(this.gameObject);
    }

    private void Death() {
      this.OnDeath?.Invoke(this);
      Destroy(this.gameObject);
    }

    private IEnumerator CastMaterializationSpell() {
      Collider[] hits = Physics.OverlapSphere(this.transform.position, this.spellDistance,
                                              LayerMask.GetMask("Ghost", "Zombie"));

      foreach (var hit in hits) {
        Enemy enemy = hit.GetComponent<Enemy>();
        if (enemy != null) {
          enemy.SetState(Enemy.State.Zombie);
        }
      }

      Quaternion rotation = this.transform.rotation * Quaternion.AngleAxis(90, Vector3.right);
      GameObject blastObject = Instantiate(this.blastPrefab, this.transform.position, rotation, this.transform);
      Blast blast = blastObject.GetComponent<Blast>();
      blast.AnimateBlast(this.spellDistance);

      this.OnSpellCasted?.Invoke(this.spellCooldown);

      yield return new WaitForSeconds(this.spellCooldown);
      this.spellRoutine = null;
    }
  }
}

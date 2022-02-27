using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using View;

namespace Controller {
  public class Game : MonoBehaviour {
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPosition;

    [Header("Enemies")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private List<Transform> enemySpawnPositions;

    [Header("UI")]
    [SerializeField] private GameUI gameUI;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private float spawnDelay;
    [SerializeField] private int startEnemiesPerWaveCount;
    [SerializeField] private int numberOfWaves;

    public UnityAction OnPlayerVictory;

    private Player player;
    private List<Enemy> enemies;
    private int currentWaveNumber;
    private int currentEnemiesPerWave;

    void Start() {
      this.enemies = new List<Enemy>();
      this.InitializeGame();
    }

    public void InitializeGame() {
      this.currentWaveNumber = 0;
      this.currentEnemiesPerWave = this.startEnemiesPerWaveCount;

      if (this.player != null) {
        this.player.Reset();
      }

      if (this.enemies.Count > 0) {
        foreach (var enemy in this.enemies) {
          enemy.Reset();
        }

        this.enemies.Clear();
      }

      var playerObject = Instantiate(this.playerPrefab,
                                     this.playerSpawnPosition.position,
                                     this.playerSpawnPosition.rotation);

      this.player = playerObject.GetComponent<Player>();
      this.gameUI.Initialize(this, this.player);
      this.StartCoroutine(this.StartEnemySpawning());
    }

    private IEnumerator StartEnemySpawning() {
      yield return new WaitForSeconds(this.spawnDelay);
      this.SpawnEnemiesWave();
    }

    private void SpawnEnemiesWave() {
      if (this.currentEnemiesPerWave > this.enemySpawnPositions.Count) {
        Debug.LogWarning("More enemies will be spawned than there are the spawn points");
      }

      List<int> availableSpawnIndexes = new List<int>();
      for (int i = 0; i < this.enemySpawnPositions.Count; i++) {
        availableSpawnIndexes.Add(i);
      }

      for (int i = 0; i < this.currentEnemiesPerWave; i++) {
        int randomIndex = Random.Range(0, availableSpawnIndexes.Count);
        int spawnPointIndex = availableSpawnIndexes[randomIndex];
        Transform spawnTransform = this.enemySpawnPositions[spawnPointIndex];

        var enemyObject = Instantiate(this.zombiePrefab, spawnTransform.position, spawnTransform.rotation);
        enemyObject.name = $"Enemy {i}";
        var enemy = enemyObject.GetComponent<Enemy>();
        this.enemies.Add(enemy);
        enemy.AssignPlayer(this.player);
        enemy.OnDeath += this.OnEnemyDeath;

        availableSpawnIndexes.Remove(spawnPointIndex);
      }

      this.currentWaveNumber++;
      this.currentEnemiesPerWave++;
    }

    private void OnEnemyDeath(IDamageable dyingEnemy) {
      this.enemies.Remove(dyingEnemy as Enemy);

      if (this.enemies.Count <= 0) {
        if (this.CheckWinCondition()) {
          this.OnPlayerVictory?.Invoke();
        } else {
          this.StartCoroutine(this.StartEnemySpawning());
        }
      }
    }

    private bool CheckWinCondition() {
      return this.currentWaveNumber >= this.numberOfWaves;
    }
  }
}

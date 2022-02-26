using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using View;

namespace Controller {
  public class Game : MonoBehaviour {
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPosition;

    [Header("Enemies")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private List<Transform> enemySpawnPositions;

    [Header("UI")]
    [SerializeField] private GameUI gameUI;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private float spawnDelay;
    [SerializeField] private int enemiesPerWave;
    [SerializeField] private int numberOfWaves;

    private Player player;
    private List<Enemy> enemies;
    private int currentWaveNumber;

    void Start() {
      if (this.enemiesPerWave > this.enemySpawnPositions.Count) {
        Debug.LogWarning("More enemies will be spawned than there are the spawn points");
      }

      this.enemies = new List<Enemy>();
      this.currentWaveNumber = 0;
      this.InitializeGame();
    }

    public void InitializeGame() {
      var playerObject = Instantiate(this.playerPrefab,
                                     this.playerSpawnPosition.position,
                                     this.playerSpawnPosition.rotation);

      this.player = playerObject.GetComponent<Player>();
      this.gameUI.SetPlayer(this.player);
      this.StartCoroutine(this.StartEnemySpawning());
    }

    private IEnumerator StartEnemySpawning() {
      yield return new WaitForSeconds(this.spawnDelay);
      this.SpawnEnemiesWave();
    }

    private void SpawnEnemiesWave() {
      if (this.currentWaveNumber >= this.numberOfWaves) {
        Debug.Log("All waves completed");
        // TODO: Probably will be moved to the Win Condition checker
        return;
      }

      List<int> availableSpawnIndexes = new List<int>();
      for (int i = 0; i < this.enemySpawnPositions.Count; i++) {
        availableSpawnIndexes.Add(i);
      }

      for (int i = 0; i < this.enemiesPerWave; i++) {
        int randomIndex = Random.Range(0, availableSpawnIndexes.Count);
        int spawnPointIndex = availableSpawnIndexes[randomIndex];
        Transform spawnTransform = this.enemySpawnPositions[spawnPointIndex];

        var enemyObject = Instantiate(this.zombiePrefab, spawnTransform.position, spawnTransform.rotation);
        var enemy = enemyObject.GetComponent<Enemy>();
        this.enemies.Add(enemy);
        enemy.AssignPlayer(this.player);
        enemy.OnDeath += this.OnEnemyDeath;

        availableSpawnIndexes.Remove(spawnPointIndex);
      }

      this.currentWaveNumber++;
    }

    private void OnEnemyDeath(IDamageable dyingEnemy) {
      this.enemies.Remove(dyingEnemy as Enemy);

      if (this.enemies.Count <= 0) {
        if (this.CheckWinCondition()) {
          Debug.Log("Player Won!");
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

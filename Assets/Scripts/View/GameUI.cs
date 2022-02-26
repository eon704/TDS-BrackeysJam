using Controller;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace View {
  public class GameUI : MonoBehaviour {
    public static GameUI Instance;

    [SerializeField] private Player player;
    [SerializeField] private Transform healthBarsParent;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject deathPanel;

    private RectTransform rectTransform;

    void Awake() {
      if (Instance == null) {
        Instance = this;
      } else {
        Debug.LogWarning($"Duplicate instance of the Singleton class: {this.name}");
        Destroy(this.gameObject);
      }

      this.rectTransform = this.GetComponent<RectTransform>();
    }

    public void InstantiateNewHealthBar(IDamageable target, Transform targetTransform) {
      GameObject healthBarObject = Instantiate(this.healthBarPrefab, this.healthBarsParent);
      HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
      healthBar.SetHealthBarData(target, targetTransform, this.rectTransform, target.MaxHealth);
    }

    public void SetPlayer(Player newPlayer) {
      this.player = newPlayer;
      this.player.OnDeath += this.OnPlayerDeath;
    }

    private void OnPlayerDeath(IDamageable damageable) {
      this.player = null;
      this.deathPanel.SetActive(true);
    }
  }
}

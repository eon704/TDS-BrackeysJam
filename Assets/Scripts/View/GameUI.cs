using Controller;
using Interfaces;
using UnityEngine;

namespace View {
  public class GameUI : MonoBehaviour {
    public static GameUI Instance;

    [SerializeField] private Transform healthBarsParent;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject deathPanel;

    [Header("Abilities")]
    [SerializeField] private AbilityCooldownUI fireballCooldownUI;
    [SerializeField] private AbilityCooldownUI spellCooldownUI;

    private Player player;
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

    public void Initialize(Game game, Player newPlayer) {
      this.player = newPlayer;
      this.player.OnDeath += this.OnPlayerDeath;
      game.OnPlayerVictory += this.OnPlayerVictory;

      this.fireballCooldownUI.Initialize(this.player);
      this.spellCooldownUI.Initialize(this.player);
    }

    public void InstantiateNewHealthBar(IDamageable target, Transform targetTransform) {
      GameObject healthBarObject = Instantiate(this.healthBarPrefab, this.healthBarsParent);
      HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
      healthBar.SetHealthBarData(target, targetTransform, this.rectTransform, target.MaxHealth);
    }

    private void OnPlayerDeath(IDamageable damageable) {
      this.player = null;
      this.deathPanel.SetActive(true);
    }

    private void OnPlayerVictory() {
      this.winPanel.SetActive(true);
      this.player.GameWon();
    }
  }
}

using Interfaces;
using UnityEngine;

namespace View {
  public class GameUI : MonoBehaviour {
    public static GameUI Instance;

    [SerializeField] private Transform healthBarsParent;
    [SerializeField] private GameObject healthBarPrefab;

    private RectTransform rectTransform;

    void Awake() {
      if (Instance == null) {
        Instance = this;
      } else {
        Destroy(this.gameObject);
      }

      this.rectTransform = this.GetComponent<RectTransform>();
    }

    public void InstantiateNewHealthBar(IDamageable target, Transform targetTransform) {
      GameObject healthBarObject = Instantiate(this.healthBarPrefab, this.healthBarsParent);
      HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
      healthBar.SetHealthBarData(target, targetTransform, this.rectTransform, target.MaxHealth);
    }
  }
}

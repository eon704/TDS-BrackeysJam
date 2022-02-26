using System;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace View {
  public class HealthBar : MonoBehaviour {
    [SerializeField] Vector2 positionOffset;
    private IDamageable target;
    private Camera mainCamera;
    private RectTransform targetCanvas;
    private RectTransform rectTransform;
    private Transform objectToFollow;
    private Slider healthBar;
    private int maxHealth;

    private bool ShouldHide => Math.Abs(this.healthBar.value - this.maxHealth) < 0.01f;

    void Awake() {
      this.mainCamera = Camera.main;
      this.healthBar = this.GetComponent<Slider>();
      this.rectTransform = this.GetComponent<RectTransform>();
    }

    void Update() {
      this.RepositionHealthBar();
    }

    public void SetHealthBarData(IDamageable newTarget, Transform targetTransform, RectTransform newTargetCanvas, int newMaxHealth) {
      this.targetCanvas = newTargetCanvas;
      this.healthBar.maxValue = newMaxHealth;
      this.healthBar.value = newMaxHealth;
      this.healthBar.minValue = 0;
      this.maxHealth = newMaxHealth;
      this.objectToFollow = targetTransform;

      newTarget.OnHealthChanged += this.OnHealthChanged;
      newTarget.OnDeath += this.OnDeath;
      this.target = newTarget;

      this.RepositionHealthBar();
      this.UpdateActiveState();
    }

    private void OnHealthChanged(int health) {
      this.healthBar.value = health;
      this.UpdateActiveState();
    }

    private void OnDeath(IDamageable source) {
      this.target.OnHealthChanged -= this.OnHealthChanged;
      this.target.OnDeath -= this.OnDeath;
      Destroy(this.gameObject);
    }

    private void UpdateActiveState() {
      this.gameObject.SetActive(!this.ShouldHide);
    }

    private void RepositionHealthBar() {
      Vector2 viewportPosition = this.mainCamera.WorldToViewportPoint(this.objectToFollow.position);
      Vector2 worldObjectScreenPosition =
        new Vector2(viewportPosition.x * this.targetCanvas.sizeDelta.x - this.targetCanvas.sizeDelta.x * 0.5f,
                    viewportPosition.y * this.targetCanvas.sizeDelta.y - this.targetCanvas.sizeDelta.y * 0.5f);
      worldObjectScreenPosition += this.positionOffset;

      //now you can set the position of the ui element
      this.rectTransform.anchoredPosition = worldObjectScreenPosition;
    }
  }
}

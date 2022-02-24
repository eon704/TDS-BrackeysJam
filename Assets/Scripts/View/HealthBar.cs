using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
  private Vector2 positionCorrection = new Vector2(0, 100);

  public RectTransform targetCanvas;
  public RectTransform healthBar;
  public Transform objectToFollow;

  public void SetHealthBarData(Transform targetTransform, RectTransform healthBarPanel) {
    this.targetCanvas = healthBarPanel;
    healthBar = GetComponent<RectTransform>();
    objectToFollow = targetTransform;
    RepositionHealthBar();
    healthBar.gameObject.SetActive(true);
  }

  public void OnHealthChanged(float healthFill) {
    healthBar.GetComponent<Image>().fillAmount = healthFill;
  }void Update() {
    RepositionHealthBar();
  }

  private void RepositionHealthBar() {
    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow.position);
    Vector2 WorldObject_ScreenPosition = new Vector2(
                                                     ((ViewportPosition.x * targetCanvas.sizeDelta.x) -
                                                      (targetCanvas.sizeDelta.x * 0.5f)),
                                                     ((ViewportPosition.y * targetCanvas.sizeDelta.y) -
                                                      (targetCanvas.sizeDelta.y * 0.5f)));
    //now you can set the position of the ui element
    healthBar.anchoredPosition = WorldObject_ScreenPosition;
  }
}

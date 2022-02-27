using System;
using UnityEngine;
using DG.Tweening;

namespace Controller {
  public class Blast : MonoBehaviour {
    [SerializeField] private float duration;

    public void AnimateBlast(float maxRadius) {
      Vector3 endScale = new Vector3(maxRadius * 2, maxRadius * 2, 1f);
      this.transform
          .DOScale(endScale, this.duration)
          .SetEase(Ease.OutExpo)
          .OnComplete(() => {
            Destroy(this.gameObject);
          });
    }
  }
}

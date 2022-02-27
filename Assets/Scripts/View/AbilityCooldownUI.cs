using Controller;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace View {
  public class AbilityCooldownUI : MonoBehaviour {
    private enum Type {
      Fireball,
      Spell
    }

    [SerializeField] private Type type;
    [SerializeField] private Image mask;

    private Player player;

    public void Initialize(Player newPlayer) {
      this.player = newPlayer;

      if (this.type == Type.Spell) {
        this.player.OnSpellCasted = this.OnAbilityUsed;
      } else if (this.type == Type.Fireball) {
        this.player.ListenToShots(this.OnAbilityUsed);
      }
    }

    private void OnAbilityUsed(float cooldown) {
      this.mask.fillAmount = 1;
      this.mask.DOFillAmount(0, cooldown);
    }
  }
}

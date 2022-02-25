using UnityEngine.Events;

namespace Interfaces {
  public interface IDamageable {
    public UnityAction<int> OnHealthChanged { get; set; }
    public UnityAction OnDeath { get; set; }
    public int MaxHealth { get; }
    public void TakeDamage(int damage);
  }
}

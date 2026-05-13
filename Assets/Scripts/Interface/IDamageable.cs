using UnityEngine;

public enum DamageType
{
    Player, Enemy, Environment
}

public interface IDamageable
{
    void TakeDamage(int amount, DamageType type);
    void TakeDamage(int amount, DamageType type, Vector3 sourcePosition);
}
using UnityEngine;

public enum DamageType
{
    Player, Enemy, Environment
}

public enum Element
{
    Fire, Water, Earth, Air, None
}

public interface IDamageable
{
    void TakeDamage(int amount, DamageType type);
    void TakeDamage(int amount, DamageType type, Vector3 sourcePosition);
}
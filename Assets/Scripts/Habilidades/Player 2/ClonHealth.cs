using UnityEngine;

public class ClonHealth : Health
{
    protected override void Die(DamageType type)
    {
        base.Die(type);
    }
}
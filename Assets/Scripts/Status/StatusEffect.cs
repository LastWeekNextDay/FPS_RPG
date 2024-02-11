using System.Collections.Generic;

public enum AffectedStat
{
    Strength,
    Agility,
    Endurance,
    Intelligence,

    Speed,
    JumpForce,
    Health,
    Mana,
    Energy,
    HealthRegen,
    ManaRegen,
    EnergyRegen
}

public struct StatAffector
{
    public AffectedStat AffectedStat;
    public bool Multiplicative;
    public int Amount;
}

public class StatusEffect
{
    public List<StatAffector> StatAffectors;
    public Dictionary<StatAffector, float> RepeatedStatAffectors;
    public float Duration;
    public bool IsStackable;
}

public class AppliedStatusEffect
{
    public StatusEffect StatusEffect;
    public float TimeLeft;
}

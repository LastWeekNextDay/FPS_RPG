using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack
{
    public InventoryItem[] items;

    public Backpack()
    {
        items = new InventoryItem[28];
    }

    public bool TryAddItem(InventoryItem item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return true;
            }
        }
        return false;
    }

    public bool TryChangeItemPosition(InventoryItem item, int toPosition)
    {
        InventoryItem itemToSwap;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                itemToSwap = items[i];
                items[toPosition] = itemToSwap;
                items[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryRemoveItem(InventoryItem item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                items[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryGetItemIndex(InventoryItem item, out int index)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }
}

public struct DamageArgs 
{
    public float Damage;
    public List<StatusEffect> StatusEffects;
}

public class Character : MonoBehaviour
{
    [Header("Unity Components")]
    public Collider col;
    public Rigidbody rigidBody;
    public AudioSource audioSource;

    [Header("Character Stats")]
    public int baseStrength;
    public int Strength { get; private set; }
    public int baseAgility;
    public int Agility { get; private set; }
    public int baseEndurance;
    public int Endurance { get; private set; }
    public int baseIntelligence;
    public int Intelligence { get; private set; }

    [Header("Character Equipment")]
    public Weapon weapon;

    public Backpack backpack;

    [NonSerialized] public float Health;
    [NonSerialized] public float MaxHealth;
    [NonSerialized] public float Mana;
    [NonSerialized] public float MaxMana;
    [NonSerialized] public float Energy;
    [NonSerialized] public float MaxEnergy;
    [NonSerialized] public float Speed;
    [NonSerialized] public float JumpForce;
    [NonSerialized] public float SightRange;
    [NonSerialized] public float HealthRegen;
    [NonSerialized] public float ManaRegen;
    [NonSerialized] public float EnergyRegen;

    [NonSerialized] public bool IsGrounded;
    [NonSerialized] public bool InCombatMode;

    private float _previousDistanceToGround;
    private float _distanceFallen;
    private float _jumpEnergy;
    private bool _justJumped;
    private bool _reachedJumpApex;
    private List<AppliedStatusEffect> _appliedStatusEffects;
    public Action<DamageArgs> OnDamageTaken;

    void Awake()
    {
        _appliedStatusEffects = new();
        backpack = new();
    }

    void Start()
    {   
        OnDamageTaken += (args) => ApplyDamage(args.Damage, args.StatusEffects);
        OnDamageTaken += (args) => RecalculateStats();
        OnDamageTaken += (args) => AudioManager.Instance.PlayHurtSound(audioSource);
        RecalculateStats();
        Health = MaxHealth;
        Mana = MaxMana;
        Energy = MaxEnergy;
    }

    void Update()
    {
        Debugging();
        CalculateGrounded();
        CalculateStatusEffectsTimings();
        ConstantlyCalculatingStats();
    }

    void FixedUpdate()
    {
        CalculateFall();
    }

    void CalculateGrounded()
    {
        var ray = new Ray(transform.position, Vector3.down);
        IsGrounded = Physics.Raycast(ray, col.bounds.extents.y);
    }

    void Debugging()
    {
        var ray = new Ray(transform.position, Vector3.down);
        var distance = 100000f;
        if (Physics.Raycast(ray, out var hit, distance))
        {
            distance = hit.distance;
        }
        Color color = IsGrounded ? Color.green : Color.red;
        Debug.DrawRay(ray.origin, ray.direction * distance, color);
    }

    public void Jump()
    {
        if (IsGrounded && Energy > _jumpEnergy && _justJumped == false)
        {
            AudioManager.Instance.PlayJumpSound(audioSource);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
            rigidBody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            Energy -= _jumpEnergy;
            _justJumped = true;
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        bool contains = false;
        int index = -1;
        foreach (var appliedStatusEffect in _appliedStatusEffects)
        {
            if (appliedStatusEffect.StatusEffect == statusEffect)
            {
                contains = true;
                index = _appliedStatusEffects.IndexOf(appliedStatusEffect);
                break;
            }
        }
        if (contains)
        {
            if (statusEffect.IsStackable == false)
            {
                _appliedStatusEffects[index].TimeLeft = statusEffect.Duration;
            } else {
                var newStatusEffect = new AppliedStatusEffect
                {
                    StatusEffect = statusEffect,
                    TimeLeft = statusEffect.Duration
                };
                _appliedStatusEffects.Add(newStatusEffect);
            }
        }
        else
        {
            var newStatusEffect = new AppliedStatusEffect
                {
                    StatusEffect = statusEffect,
                    TimeLeft = statusEffect.Duration
                };
            _appliedStatusEffects.Add(newStatusEffect);
        }
        RecalculateStats();
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        bool contains = false;
        int index = -1;
        foreach (var appliedStatusEffect in _appliedStatusEffects)
        {
            if (appliedStatusEffect.StatusEffect == statusEffect)
            {
                contains = true;
                index = _appliedStatusEffects.IndexOf(appliedStatusEffect);
                break;
            }
        }
        if (contains)
        {
            _appliedStatusEffects.RemoveAt(index);
        }
        RecalculateStats();
    }

    public void RemoveStatusEffect(AppliedStatusEffect appliedStatusEffect)
    {
        bool contains = false;
        int index = -1;
        foreach (var statusEffect in _appliedStatusEffects)
        {
            if (statusEffect == appliedStatusEffect)
            {
                contains = true;
                index = _appliedStatusEffects.IndexOf(statusEffect);
                break;
            }
        }
        if (contains)
        {
            _appliedStatusEffects.RemoveAt(index);
        }
        RecalculateStats();
    }

    void CalculateStatusEffectsTimings()
    {
        foreach (var appliedStatusEffect in _appliedStatusEffects)
        {
            foreach (var repeatedStatAffector in appliedStatusEffect.StatusEffect.RepeatedStatAffectors)
            {
                appliedStatusEffect.StatusEffect.RepeatedStatAffectors[repeatedStatAffector.Key] -= Time.deltaTime;
                if (appliedStatusEffect.StatusEffect.RepeatedStatAffectors[repeatedStatAffector.Key] <= 0f)
                {
                    RecalculateStats();
                }
            }
            
            if (appliedStatusEffect.TimeLeft == -1f) continue;
            appliedStatusEffect.TimeLeft -= Time.deltaTime;
            if (appliedStatusEffect.TimeLeft <= 0f)
            {
                RemoveStatusEffect(appliedStatusEffect);
            }
        }
    }

    void ConstantlyCalculatingStats()
    {
        Health += HealthRegen * Time.deltaTime;
        Mana += ManaRegen * Time.deltaTime;
        Energy += EnergyRegen * Time.deltaTime;

        if (Health > MaxHealth){
            Health = MaxHealth;
        }
        if (Mana > MaxMana){
            Mana = MaxMana;
        }
        if (Energy > MaxEnergy){
            Energy = MaxEnergy;
        }
    }

    public void RecalculateStats()
    {
        Strength = baseStrength;
        Agility = baseAgility;
        Endurance = baseEndurance;
        Intelligence = baseIntelligence;

        foreach (var appliedStatusEffects in _appliedStatusEffects)
        {
            foreach (var statAffector in appliedStatusEffects.StatusEffect.StatAffectors)
            {
                switch (statAffector.AffectedStat)
                {
                    case AffectedStat.Strength:
                    case AffectedStat.Agility:
                    case AffectedStat.Endurance:
                    case AffectedStat.Intelligence:
                        ApplyAffector(statAffector);
                        break;
                }
            }
            foreach (var repeatedStatAffector in appliedStatusEffects.StatusEffect.RepeatedStatAffectors)
            {
                switch (repeatedStatAffector.Key.AffectedStat)
                {
                    case AffectedStat.Strength:
                    case AffectedStat.Agility:
                    case AffectedStat.Endurance:
                    case AffectedStat.Intelligence:
                        if (repeatedStatAffector.Value <= 0f)
                        {
                            ApplyAffector(repeatedStatAffector.Key);
                            appliedStatusEffects.StatusEffect.RepeatedStatAffectors[repeatedStatAffector.Key] = repeatedStatAffector.Key.Amount;
                        }
                        break;
                }
            }
        }
        
        if (Strength < 1) Strength = 1;
        if (Agility < 1) Agility = 1;
        if (Endurance < 1) Endurance = 1;
        if (Intelligence < 1) Intelligence = 1;

        Speed = Agility * 0.33f;
        JumpForce = Agility * 10f + Strength;
        SightRange = 20f;
        MaxHealth = Strength * 2f + Endurance * 10f;
        MaxMana = Intelligence * 10f;
        MaxEnergy = Endurance * 10f;
        HealthRegen = Endurance * 0.1f;
        ManaRegen = Intelligence * 0.05f + Endurance * 0.05f;
        EnergyRegen = Endurance * 0.1f;
        _jumpEnergy = JumpForce * 0.1f / Endurance;

        foreach (var appliedStatusEffects in _appliedStatusEffects)
        {
            foreach (var statAffector in appliedStatusEffects.StatusEffect.StatAffectors)
            {
                switch (statAffector.AffectedStat)
                {
                    case AffectedStat.Speed:
                    case AffectedStat.JumpForce:
                    case AffectedStat.Health:
                    case AffectedStat.Mana:
                    case AffectedStat.Energy:
                    case AffectedStat.HealthRegen:
                    case AffectedStat.ManaRegen:
                    case AffectedStat.EnergyRegen:
                        ApplyAffector(statAffector);
                        break;
                }
            }
            foreach (var repeatedStatAffector in appliedStatusEffects.StatusEffect.RepeatedStatAffectors)
            {
                switch (repeatedStatAffector.Key.AffectedStat)
                {
                    case AffectedStat.Speed:
                    case AffectedStat.JumpForce:
                    case AffectedStat.Health:
                    case AffectedStat.Mana:
                    case AffectedStat.Energy:
                    case AffectedStat.HealthRegen:
                    case AffectedStat.ManaRegen:
                    case AffectedStat.EnergyRegen:
                        if (repeatedStatAffector.Value <= 0f)
                        {
                            ApplyAffector(repeatedStatAffector.Key);
                            appliedStatusEffects.StatusEffect.RepeatedStatAffectors[repeatedStatAffector.Key] = repeatedStatAffector.Key.Amount;
                        }
                        break;
                }
            }
        }
    }

    void ApplyAffector(StatAffector stat)
    {
        switch (stat.AffectedStat)
        {
            case AffectedStat.Strength:
                if (stat.Multiplicative){
                    Strength *= stat.Amount;
                } else {
                    Strength += stat.Amount;
                }
                break;
            case AffectedStat.Agility:
                if (stat.Multiplicative){
                    Agility *= stat.Amount;
                } else {
                    Agility += stat.Amount;
                }
                break;
            case AffectedStat.Endurance:
                if (stat.Multiplicative){
                    Endurance *= stat.Amount;
                } else {
                    Endurance += stat.Amount;
                }
                break;
            case AffectedStat.Intelligence:
                if (stat.Multiplicative){
                    Intelligence *= stat.Amount;
                } else {
                    Intelligence += stat.Amount;
                }
                break;
            case AffectedStat.Speed:
                if (stat.Multiplicative){
                    Speed *= stat.Amount;
                } else {
                    Speed += stat.Amount;
                }
                break;
            case AffectedStat.JumpForce:
                if (stat.Multiplicative){
                    JumpForce *= stat.Amount;
                } else {
                    JumpForce += stat.Amount;
                }
                break;
            case AffectedStat.Health:
                RestoreHealth(stat.Amount, stat.Multiplicative);
                break;
            case AffectedStat.Mana:
                RestoreMana(stat.Amount, stat.Multiplicative);
                break;
            case AffectedStat.Energy:
                RestoreEnergy(stat.Amount, stat.Multiplicative);
                break;
            case AffectedStat.HealthRegen:
                if (stat.Multiplicative){
                    HealthRegen *= stat.Amount;
                } else {
                    HealthRegen += stat.Amount;
                }
                break;
            case AffectedStat.ManaRegen:
                if (stat.Multiplicative){
                    ManaRegen *= stat.Amount;
                } else {
                    ManaRegen += stat.Amount;
                }
                break;
            case AffectedStat.EnergyRegen:
                if (stat.Multiplicative){
                    EnergyRegen *= stat.Amount;
                } else {
                    EnergyRegen += stat.Amount;
                }
                break;
        }
    }

    void ApplyDamage(float damage, List<StatusEffect> statusEffects = null)
    {
        if (statusEffects != null)
        {
            foreach (var statusEffect in statusEffects)
            {
                AddStatusEffect(statusEffect);
            }
        }
        Health -= damage;
    }

    void CalculateFall()
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (IsGrounded == false){
            Physics.Raycast(ray, out var hit, 100000f);
            var currentDistanceToGround = hit.distance;
            if (_previousDistanceToGround == 0){
                _previousDistanceToGround = currentDistanceToGround;
            }
    
            if (((currentDistanceToGround < _previousDistanceToGround) || (currentDistanceToGround == _previousDistanceToGround)) 
            && _reachedJumpApex == false)
            {
                _reachedJumpApex = true;
            }
            var differenceBetweenDistances = Mathf.Abs(_previousDistanceToGround - currentDistanceToGround);
            _previousDistanceToGround = currentDistanceToGround;
            if (_reachedJumpApex)
            {
                _distanceFallen += Mathf.Abs(differenceBetweenDistances); 
            }     
        } else {
            var damageDistanceLimit = 3f + (Agility * 0.1f);
            if (_distanceFallen > damageDistanceLimit)
            {
                var difference = _distanceFallen - damageDistanceLimit;
                var damage = difference;
                var args = new DamageArgs
                {
                    Damage = damage
                };
                TakeDamage(args);
            }
            _distanceFallen = 0f;
            if (_reachedJumpApex)
            {
                _reachedJumpApex = false;
                _justJumped = false;   
            }
        }
    }

    public void TakeDamage(DamageArgs args)
    {
        OnDamageTaken?.Invoke(args);
    }

    public void RestoreHealth(float amount, bool mult = false)
    {
        if (mult)
        {
            Health *= amount;
        }
        else
        {
            Health += amount;
        }
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void RestoreMana(float amount, bool mult = false)
    {
        if (mult)
        {
            Mana *= amount;
        }
        else
        {
            Mana += amount;
        }
        if (Mana > MaxMana)
        {
            Mana = MaxMana;
        }
    }

    public void RestoreEnergy(float amount, bool mult = false)
    {
        if (mult)
        {
            Energy *= amount;
        }
        else
        {
            Energy += amount;
        }
        if (Energy > MaxEnergy)
        {
            Energy = MaxEnergy;
        }
    }
}
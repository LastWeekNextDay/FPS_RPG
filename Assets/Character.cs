using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Backpack Backpack;
    public Equipment Equipment;

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

    public static Action<DamageArgs> OnDamageTaken;
    public static Action<JumpArgs> OnJump;
    public static Action<StatusEffectArgs> OnAddStatusEffect;
    public static Action<StatusEffectArgs> OnRemoveStatusEffect;
    public static Action<AppliedStatusEffectArgs> OnRemoveAppliedStatusEffect;
    public static Action<WeaponPulloutArgs> OnWeaponPullout;
    public static Action<WeaponPutawayArgs> OnWeaponPutaway;
    public static Action<PickupItemArgs> OnPickupItem;

    [NonSerialized] public Vector3 OriginalScale;

    void Awake()
    {
        _appliedStatusEffects = new();
        Backpack = new(this);
        Equipment = new(this);
    }

    void Start()
    {   
        OriginalScale = transform.localScale;
        RecalculateStats();
        Health = MaxHealth;
        Mana = MaxMana;
        Energy = MaxEnergy;
    }

    void Update()
    {
        Debugging();
        CalculateGrounded();
        if (IsDead() == false)
        {
            CalculateStatusEffectsTimings();
            ConstantlyCalculatingStats();
        }
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

    public bool IsDead()
    {
        return Health <= 0f;
    }

    public void Jump()
    {
        if (IsGrounded && Energy > _jumpEnergy && _justJumped == false)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
            rigidBody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
            Energy -= _jumpEnergy;
            _justJumped = true;
            var args = new JumpArgs
            {
                Source = this
            };
            OnJump?.Invoke(args);   
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
        var args = new StatusEffectArgs
        {
            Target = this,
            StatusEffect = statusEffect
        };
        OnAddStatusEffect?.Invoke(args);
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
        var args = new StatusEffectArgs
        {
            Target = this,
            StatusEffect = statusEffect
        };
        OnRemoveStatusEffect?.Invoke(args);
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
        var args = new AppliedStatusEffectArgs
        {
            Target = this,
            StatusEffect = appliedStatusEffect
        };
        OnRemoveAppliedStatusEffect?.Invoke(args);
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

        if (IsGrounded == false)
            {
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
                    Target = this,
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
        ApplyDamage(args.Damage, args.StatusEffects);
        RecalculateStats(); 
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

    public void ToggleCombatMode(bool b)
    {
        InCombatMode = b;
    }

    public void ReadyWeapon()
    {
        if (Equipment.WeaponItem == null)
        {
            if (PrefabContainer.Instance.TryGetPrefab("Fists", out var fistsPrefab))
            {
                var fists = Instantiate(fistsPrefab, transform.position, Quaternion.identity);
                Equipment.TryEquipWeapon(fists.GetComponent<Weapon>());
            }
        }
        if (Equipment.WeaponItem.IsReady == false)
        {
            PullOutWeapon();
        }
        else
        {
            PutAwayWeapon();
        }
    }

    public void PullOutWeapon()
    {
        if (Equipment.WeaponItem == null)
        {
            if (PrefabContainer.Instance.TryGetPrefab("Fists", out var fistsPrefab))
            {
                var fists = Instantiate(fistsPrefab, transform.position, Quaternion.identity);
                Equipment.TryEquipWeapon(fists.GetComponent<Weapon>());
            }
        }
        StartCoroutine(nameof(PullOutWeaponCoroutine));
        var args = new WeaponPulloutArgs
        {
            Source = this,
            Weapon = Equipment.WeaponItem
        };
        OnWeaponPullout?.Invoke(args);
    }

    public void PutAwayWeapon()
    {
        StartCoroutine(nameof(PutAwayWeaponCoroutine));
        var args = new WeaponPutawayArgs
        {
            Source = this,
            Weapon = Equipment.WeaponItem
        };
        OnWeaponPutaway?.Invoke(args);
    }

    public void PickupItem(Item item)
    {
        if (Backpack.TryAddItem(item))
        {
            item.SetActiveInWorld(false, parent: transform);
        }
        var args = new PickupItemArgs
        {
            Source = this,
            Item = item
        };
        OnPickupItem?.Invoke(args); 
    }

    public void AttackPrimary(Vector3 dir)
    {
        if (Equipment.WeaponItem == null) return;
        var weapon = Equipment.WeaponItem;
        if (weapon.IsReady == false) return;
        if (weapon.IsAttacking) return;
        StartCoroutine(nameof(AttackPrimaryCoroutine), dir);
    }

    IEnumerator AttackPrimaryCoroutine(Vector3 dir)
    {
        var weapon = Equipment.WeaponItem;
        weapon.IsAttacking = true;
        var middle_of_character = transform.position + col.bounds.extents.y * transform.up;
        var ray = new Ray(middle_of_character, dir);
        var distance = weapon.baseRange;
        var hits = Physics.RaycastAll(ray, distance);
        List<RaycastHit> objectsHit = new();
        List<RaycastHit> charactersHit = new();
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                switch (hit.collider.tag)
                {
                    case "Object":
                        objectsHit.Add(hit);
                        break;
                    case "Character":
                        charactersHit.Add(hit);
                        break;
                }
            }
        }
        GameObject closestObjectHit = null;
        GameObject closestCharacterHit = null;
        foreach (var objHit in objectsHit)
        {
            if (closestObjectHit == null)
            {
                closestObjectHit = objHit.collider.gameObject;
            }
            else
            {
                var distanceToObjectHit = Vector3.Distance(middle_of_character, objHit.collider.gameObject.transform.position);
                var distanceToClosestObjectHit = Vector3.Distance(middle_of_character, closestObjectHit.transform.position);
                if (distanceToObjectHit < distanceToClosestObjectHit)
                {
                    closestObjectHit = objHit.collider.gameObject;
                }
            }
        }
        foreach (var characterHit in charactersHit)
        {
            if (closestCharacterHit == null)
            {
                closestCharacterHit = characterHit.collider.gameObject;
            }
            else
            {
                var distanceToCharacterHit = Vector3.Distance(middle_of_character, characterHit.collider.gameObject.transform.position);
                var distanceToClosestCharacterHit = Vector3.Distance(middle_of_character, closestCharacterHit.transform.position);
                if (distanceToCharacterHit < distanceToClosestCharacterHit)
                {
                    closestCharacterHit = characterHit.collider.gameObject;
                }
            }
        }
        GameObject objectHit = null;
        if (closestCharacterHit == null){
            if (closestObjectHit != null){
                objectHit = closestObjectHit;
            }
        } else if (closestObjectHit == null){
            if (closestCharacterHit != null){
                objectHit = closestCharacterHit;
            }
        } else {
            var distanceToObjectHit = Vector3.Distance(middle_of_character, closestObjectHit.transform.position);
            var distanceToCharacterHit = Vector3.Distance(middle_of_character, closestCharacterHit.transform.position);
            if (distanceToObjectHit < distanceToCharacterHit)
            {
                objectHit = closestObjectHit;
            }
            else
            {
                objectHit = closestCharacterHit;
            }
        }
        RaycastHit finalhit = new();
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == objectHit)
            {
                finalhit = hit;
                break;
            }
        }
        if (objectHit != null)
        {
            if (objectHit.TryGetComponent(out Object obj))
            {
                obj.GetHit(dir, finalhit.point);
            } else if (objectHit.TryGetComponent(out Character character))
            {
                character.TakeDamage(new DamageArgs
                {
                    Source = this,
                    Target = character,
                    Damage = weapon.baseDamage,
                });
            }
        }
        yield return new WaitForSeconds(weapon.baseAttackTime);
        weapon.IsAttacking = false;
    }

    IEnumerator PullOutWeaponCoroutine()
    {
        var weapon = Equipment.WeaponItem;
        yield return new WaitForSeconds(weapon.basePullOutTime);
        weapon.IsReady = true;
        // TODO: Add animation
        // TODO: This is a temporary solution
        var thisy = transform.position.y + col.bounds.extents.y / 2;
        var pos = new Vector3(transform.position.x, thisy, transform.position.z);
        var infront = pos + transform.forward;
        Equipment.WeaponItem.SetActiveInWorld(true, infront, transform);
    }

    IEnumerator PutAwayWeaponCoroutine()
    {
        var weapon = Equipment.WeaponItem;
        yield return new WaitForSeconds(weapon.basePullOutTime);
        weapon.IsReady = false;
        // TODO: Add animation
        // TODO: This is a temporary solution
        Equipment.WeaponItem.SetActiveInWorld(false);
    }
}
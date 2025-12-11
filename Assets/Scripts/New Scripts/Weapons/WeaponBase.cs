using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public bool isEquipped;  

    public virtual void OnEquip() { }

    public virtual void OnUnequip() { }

    public virtual void UpdateWeapon() { }
}
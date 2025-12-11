using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponBase flamethrower;            // Reference to the Flamethrower WeaponBase
    public WeaponBase bombLauncher;           // Reference to the Bomb Launcher WeaponBase
    public Transform weaponSocket;            // The socket/bone to attach weapons to (e.g., RightHand)

    private WeaponBase currentWeapon;

    void Start()
    {
        // Equip the flamethrower by default and attach it to the correct socket
        EquipWeapon(flamethrower);
    }

    void Update()
    {
        // Switch between weapons using the scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            EquipWeapon(flamethrower);  // Scroll Up: Equip Flamethrower
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            EquipWeapon(bombLauncher); // Scroll Down: Equip Bomb Launcher
        }

        // Update the currently equipped weapon
        currentWeapon?.UpdateWeapon();
    }

    void EquipWeapon(WeaponBase newWeapon)
    {
        // If the new weapon is different from the current one, switch
        if (currentWeapon != newWeapon)
        {
            if (currentWeapon != null)
            {
                currentWeapon.isEquipped = false;  
                currentWeapon.OnUnequip();         
            }

          
            currentWeapon = newWeapon;
            currentWeapon.isEquipped = true;      
            currentWeapon.OnEquip();              

           
            if (weaponSocket != null && currentWeapon != null)
            {
                currentWeapon.transform.SetParent(weaponSocket);  
                currentWeapon.transform.localPosition = Vector3.zero;  
                currentWeapon.transform.localRotation = Quaternion.identity; 
            }

            ToggleWeaponVisibility();
        }
    }

    void ToggleWeaponVisibility()
    {
        flamethrower.gameObject.SetActive(currentWeapon == flamethrower);
        bombLauncher.gameObject.SetActive(currentWeapon == bombLauncher);
    }
}

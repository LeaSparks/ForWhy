using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.InputSystem;  

public class Flamethrower : WeaponBase
{
    [Header("Flamethrower Settings")]
    public ParticleSystem flameParticles;     
    public AudioSource flameSound;             
    public float fuelDrainRate = 10f;         
    [Range(0f, 100f)]
    [SerializeField] private float currentFuel = 100f;
    private bool isFiring = false;           

    public Slider fuelSlider; 

    private InputAction fireWeaponAction;

    public override void OnEquip()
    {
        base.OnEquip();
        if (flameParticles != null) flameParticles.Stop(); 
        if (flameSound != null) flameSound.Stop();        

        if (fuelSlider != null)
        {
            fuelSlider.maxValue = 100f;  
            fuelSlider.value = currentFuel; 
        }
    }

    public override void OnUnequip()
    {
        base.OnUnequip();
        if (flameParticles != null) flameParticles.Stop();
        if (flameSound != null) flameSound.Stop();        
    }

    public override void UpdateWeapon()
    {
        if (isEquipped)
        {
            if (fireWeaponAction.ReadValue<float>() > 0 && currentFuel > 0f)
            {
                isFiring = true;
                StartFlame();
                DrainFuel();
            }
            else
            {
                isFiring = false;
                StopFlame();
            }

            if (fuelSlider != null)
            {
                fuelSlider.value = currentFuel;
            }
        }
    }

    private void StartFlame()
    {
        if (flameParticles != null && !flameParticles.isPlaying)
        {
            flameParticles.Play();  
        }

        if (flameSound != null && !flameSound.isPlaying)
        {
            flameSound.Play(); 
        }
    }

    private void StopFlame()
    {
        if (flameParticles != null && flameParticles.isPlaying)
        {
            flameParticles.Stop(); 
        }

        if (flameSound != null && flameSound.isPlaying)
        {
            flameSound.Stop();  
        }
    }

    private void DrainFuel()
    {
        currentFuel -= fuelDrainRate * Time.deltaTime;
        if (currentFuel < 0) currentFuel = 0; 
    }

    public void RefillFuel(float amount)
    {
        currentFuel += amount;
        if (currentFuel > 100f) currentFuel = 100f;
    }

    private void OnEnable()
    {
        fireWeaponAction = new InputAction("FireWeapon", binding: "<Keyboard>/space");
        fireWeaponAction.Enable();
    }

    private void OnDisable()
    {
        fireWeaponAction.Disable();
    }
}

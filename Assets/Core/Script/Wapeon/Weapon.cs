using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wapeon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cam;
    [SerializeField] Transform attackPoint;
    [SerializeField] GameObject objectToThrow;
    BuildingMod buildingMod;

    [Header("Setting")]
    public int TotalThrow;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;

    [Header("Mode")]
    public bool IsChained;
    public float TimerChain;
    float CurrentTimerChain;
    public bool IsHasten;
    bool HastApplied = false;
    public float TimerHast;
    float CurrentTimerHast;

    [Header("Hast")]
    [SerializeField] float ThrowHastenModificator;

    [Header("Vfx")]
    [SerializeField] ParticleSystem bangVfx;

    [Header("UI")]
    [SerializeField] GameObject LayoutUi;
    [SerializeField] GameObject UiChained;
    GameObject CurrentUiCHained;
    bool UiIsActivChained = false;
    [SerializeField] GameObject UiHast;
    GameObject CurrentUiHast;
    bool UiIsActivHast = false;

    bool readyToThrow;


    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        CurrentTimerChain = TimerChain;
        CurrentTimerHast = TimerHast;

        buildingMod = GetComponent<BuildingMod>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!buildingMod.bIsBuilding)
        {
            if (Input.GetKey(throwKey) && readyToThrow && TotalThrow > 0)
            {
                Throw();
                bangVfx.Play();
            }
        }
        if(IsChained == true)
        {
            Chained();
        }
        if(IsHasten == true)
        {
            Haste();
        }
       

    }

    void Chained()
    {
        if (IsChained == true && CurrentTimerChain > 0)
        {
            CurrentTimerChain -= Time.deltaTime;
        }
        else if (CurrentTimerChain <= 0)
        {
            CurrentTimerChain = TimerChain;
            IsChained = false;
            Destroy(CurrentUiCHained);
            UiIsActivChained = false;
        }
    }

    void Haste()
    {
        if (IsHasten == true && CurrentTimerHast > 0)
        {
            CurrentTimerHast -= Time.deltaTime;
            if(HastApplied == false)
            {
                throwCooldown = throwCooldown / ThrowHastenModificator;
                HastApplied = true;
            }
        }
        else if (CurrentTimerHast <= 0)
        {
            CurrentTimerHast = TimerHast;
            IsHasten = false;
            UiIsActivHast = false;
            Destroy(CurrentUiHast);
            HastApplied = false;
            throwCooldown = throwCooldown * ThrowHastenModificator;

        }
    }

    public void ActivateUI()
    {
        if (UiIsActivChained == false && IsChained == true)
        {
            CurrentUiCHained = Instantiate(UiChained, LayoutUi.transform);
            UiIsActivChained = true;
        }
        if(UiIsActivHast == false && IsHasten == true)
        {
            CurrentUiHast = Instantiate(UiHast, LayoutUi.transform);
            UiIsActivHast = true;
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        //instantiate object
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        //get rigidbody
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        //Direction
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward,out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;

        }

        // add Force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        TotalThrow--;

        //mods
        if (IsChained == true)
        {
            projectile.GetComponent<WeaponAddon>().Chained = true;
        }

        //throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}

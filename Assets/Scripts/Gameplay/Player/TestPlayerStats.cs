using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerStats : MonoBehaviour
{
    public static float PlayerMaxSpeed = 6f;
    public static float PlayerSprintMultiplier = 2f;
    public static float PlayerFreezingTime = 4;
    public static float PlayerHealthMax = 100f;
    public static float PlayerHealth = 100f;

    public static float WeaponFirePower = 21;
    public static float WeaponDamage = 50;
    public static float WeaponFireRate = 1f;
    public static float WeaponFireRange = 10f;

    [Header("Player")]
    [SerializeField]
    public InputField playerMaxSpeedField;
    [SerializeField]
    public InputField playerSprintMultiplierField;
    [SerializeField]
    public InputField playerFreezingTimeField;
    [SerializeField]
    public InputField playerHealthMaxField;
    [SerializeField]
    public InputField playerHealthField;

    [Header("Weapon")]
    [SerializeField]
    public InputField weaponFirePowerField;

    [SerializeField]
    public InputField weaponDamageField;

    [SerializeField]
    public InputField weaponFireRateField;

    [SerializeField]
    public InputField weaponFireRangeField;

    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);        
#else
        playerMaxSpeedField.text = PlayerMaxSpeed.ToString();
        playerSprintMultiplierField.text = PlayerSprintMultiplier.ToString();
        playerFreezingTimeField.text = PlayerFreezingTime.ToString();
        playerHealthMaxField.text = PlayerHealthMax.ToString();
        playerHealthField.text = PlayerHealth.ToString();

        weaponDamageField.text = WeaponDamage.ToString();
        weaponFirePowerField.text = WeaponFirePower.ToString();
        weaponFireRangeField.text = WeaponFireRange.ToString();
        weaponFireRateField.text = WeaponFireRate.ToString();

        playerFreezingTimeField.onValueChanged.AddListener(OnPlayerFreezingTimeChanged);
        playerHealthField.onValueChanged.AddListener(OnPlayerHealthChanged);
        playerHealthMaxField.onValueChanged.AddListener(OnPlayerHealthMaxChanged);
        playerMaxSpeedField.onValueChanged.AddListener(OnPlayerMaxSpeedChanged);
        playerSprintMultiplierField.onValueChanged.AddListener(OnPlayerSprintMultiplyerChanged);
        weaponDamageField.onValueChanged.AddListener(OnWeaponDamageChanged);
        weaponFirePowerField.onValueChanged.AddListener(OnWeaponFirePowerChanged);
        weaponFireRangeField.onValueChanged.AddListener(OnWeaponFireRangeChanged);
        weaponFireRateField.onValueChanged.AddListener(OnWeaponFireRateChanged);
#endif

    }

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerFreezingTimeChanged(string value)
    {
        PlayerFreezingTime = float.Parse(value);
    }

    void OnPlayerHealthChanged(string value)
    {
        PlayerHealth = float.Parse(value);
    }

    void OnPlayerHealthMaxChanged(string value)
    {
        PlayerHealthMax = float.Parse(value);
    }

    void OnPlayerMaxSpeedChanged(string value)
    {
        PlayerMaxSpeed = float.Parse(value);
        Debug.Log("SpeedChanged:" + PlayerMaxSpeed);
    }

    void OnPlayerSprintMultiplyerChanged(string value)
    {
        PlayerSprintMultiplier = float.Parse(value);
    }

    void OnWeaponDamageChanged(string value)
    {
        WeaponDamage = float.Parse(value);
    }

    void OnWeaponFirePowerChanged(string value)
    {
        WeaponFirePower = float.Parse(value);
    }

    void OnWeaponFireRangeChanged(string value)
    {
        WeaponFireRange = float.Parse(value);
    }

    void OnWeaponFireRateChanged(string value)
    {
        WeaponFireRate = float.Parse(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoca;

public class TestPowerUpUI : MonoBehaviour
{
    [SerializeField]
    Text speedText;

    [SerializeField]
    Text staminaText;

    [SerializeField]
    Text freezingTimeText;

    [SerializeField]
    Text firePowerText;

    [SerializeField]
    Text fireRateText;

    [SerializeField]
    Text fireRangeText;

    [SerializeField]
    Text speedPowerUpText;

    [SerializeField]
    Text staminaPowerUpText;

    [SerializeField]
    Text freezingTimePowerUpText;

    [SerializeField]
    Text firePowerPowerUpText;

    [SerializeField]
    Text fireRatePowerUpText;

    [SerializeField]
    Text fireRangePowerUpText;

    private void Awake()
    {
#if !UNITY_EDITORW
    Destroy(gameObject);
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

    private void LateUpdate()
    {
        speedText.text = PlayerController.Local.MaxSpeed.ToString();
        staminaText.text = PlayerController.Local.StaminaMax.ToString();
        freezingTimeText.text = PlayerController.Local.FreezingCooldown.ToString();
        firePowerText.text = PlayerController.Local.FireWeapon.Power.ToString();
        fireRateText.text = PlayerController.Local.FireWeapon.FireRate.ToString();
        fireRangeText.text = PlayerController.Local.FireWeapon.FireRange.ToString();

        //PowerUpManager pum = PlayerController.Local.GetComponent<PowerUpManager>();
        //if (pum.IsPowerUpActive(Skill.Speed))
        //    speedPowerUpText.text = pum.GetPowerUpRemainingTime(Skill.Speed).ToString();
        //else
        //    speedPowerUpText.text = "0";

        //if (pum.IsPowerUpActive(Skill.Stamina))
        //    staminaPowerUpText.text = pum.GetPowerUpRemainingTime(Skill.Stamina).ToString();
        //else
        //    staminaPowerUpText.text = "0";

        //if (pum.IsPowerUpActive(Skill.Resistance))
        //    freezingTimePowerUpText.text = pum.GetPowerUpRemainingTime(Skill.Resistance).ToString();
        //else
        //    freezingTimePowerUpText.text = "0";

        //if (pum.IsPowerUpActive(Skill.FirePower))
        //    firePowerPowerUpText.text = pum.GetPowerUpRemainingTime(Skill.FirePower).ToString();
        //else
        //    firePowerPowerUpText.text = "0";

        //if (pum.IsPowerUpActive(Skill.FireRate))
        //    fireRatePowerUpText.text = pum.GetPowerUpRemainingTime(Skill.FireRate).ToString();
        //else
        //    fireRatePowerUpText.text = "0";

        //if (pum.IsPowerUpActive(Skill.FireRange))
        //    fireRangePowerUpText.text = pum.GetPowerUpRemainingTime(Skill.FireRange).ToString();
        //else
        //    fireRangePowerUpText.text = "0";
    }
}

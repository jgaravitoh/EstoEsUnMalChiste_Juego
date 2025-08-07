using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public float damagePhysical;
    public float damageOffense;

    public float maxHP;
    public float currentHP;
    public int iniciative; // Determines the turn the unit will get to act
    public bool boolIsEnemy;
    public Slider healthBar;
    public GameObject selectedTriangle;
    public GameObject ActionButtons;
    public TMP_Text textName;
    public CinemachineCamera camera;

    private Coroutine healthBarCoroutine;

    private void Start()
    {
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
        UpdateHealthBarColor(currentHP);
        selectedTriangle.SetActive(false);
        textName.text = unitName;
        camera = GetComponentInChildren<CinemachineCamera>();
    }

    public void Action_ReceiveDamage(float _incomingDamage, bool _isPhysical, bool _isOffensive)
    {
        currentHP -= _incomingDamage;

        if (_isPhysical && !_isOffensive)
        {
            // TODO: set animation to hurt   
        }
        else if (!_isPhysical && _isOffensive)
        {
            // TODO: set animation to offended
        }
        else if (_isPhysical && _isOffensive)
        {
            // TODO: create a mixed event
        }

        selectedTriangle.SetActive(false);

        // Animate health bar change
        StartHealthBarAnimation(currentHP);
    }

    public void Action_PhysicalAttack(Unit _unit)
    {
        bool isPhysical = true;
        bool isOffensive = false;
        _unit.Action_ReceiveDamage(damagePhysical, isPhysical, isOffensive);
    }

    public void Action_OffenseAttack(Unit _unit)
    {
        bool isPhysical = false;
        bool isOffensive = true;
        _unit.Action_ReceiveDamage(damageOffense, isPhysical, isOffensive);
    }

    private void StartHealthBarAnimation(float targetHP)
    {
        if (healthBarCoroutine != null)
            StopCoroutine(healthBarCoroutine);

        healthBarCoroutine = StartCoroutine(SmoothHealth(healthBar.value, targetHP));
    }

    private IEnumerator SmoothHealth(float from, float to)
    {
        float duration = 0.4f;
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            float value = Mathf.Lerp(from, to, t);
            healthBar.value = value;
            UpdateHealthBarColor(value);
            yield return null;
        }

        healthBar.value = to;
        UpdateHealthBarColor(to);
    }

    private void UpdateHealthBarColor(float value)
    {
        float normalized = value / maxHP;
        healthBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, normalized);
    }

    public IEnumerator Die()
    {
        Debug.Log($"{unitName} is going to die");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject, 1);
        Debug.Log($"{unitName} has died");
    }
}

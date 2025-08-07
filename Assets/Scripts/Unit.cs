using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.Cinemachine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public float damagePhysical;
    public float damageOffense;

    public float maxHP;
    public float currentHP;
    public int iniciative; // Determines the turn the will get to act
    public bool boolIsEnemy;
    public Slider healthBar;
    public GameObject selectedTriangle;
    public GameObject ActionButtons;
    public TMP_Text textName;
    public CinemachineCamera camera;


    private void Start()
    {
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
        selectedTriangle.SetActive(false);
        textName.text = unitName;
        camera = GetComponentInChildren<CinemachineCamera>();
    }

    public void Action_ReceiveDamage(float _incomingDamage, bool _isPhysical, bool _isOffensive)
    {
        if (_isPhysical && !_isOffensive)
        {
            currentHP -= damagePhysical; // TODO: set animation to hurt   
        }
        else if (!_isPhysical && _isOffensive)
        {
            currentHP -= damageOffense; // TODO: set animation to offended
        }
        if (_isPhysical && !_isOffensive)
        {
            //TODO: CREATE A MIXED EVENT
        }
        selectedTriangle.SetActive(false);
        healthBar.value = currentHP; // animate in the future

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

    public IEnumerator Die()
    {
        Debug.Log($"{unitName} is going to die");
        yield return new WaitForSeconds(2f);

        Destroy(gameObject,1);
        Debug.Log($"{unitName} is going to died");
    }
}
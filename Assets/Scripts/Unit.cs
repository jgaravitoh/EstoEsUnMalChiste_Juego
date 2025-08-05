using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;

    public int damagePhysical;
    public int damageOffense;

    public int maxHP;
    public int currentHP;
    public int iniciative;
    public bool boolIsEnemy;
    public Slider healthBar;
    public GameObject selectedTriangle;


    private void Start()
    {
        currentHP = maxHP;
    }
}



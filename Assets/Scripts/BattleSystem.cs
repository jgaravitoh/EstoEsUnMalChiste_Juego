using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    private List<GameObject> list_GameObjectsOnly = new List<GameObject>();
    private List<Unit> list_Units = new List<Unit>();
    int enemyCount = 0, allyCount = 0;

    // Lists Flags
    private bool bool_IsGameObjectListValidated = false;
    private bool bool_IsUnitListValidated = false;
    void Start()
    {
        state = BattleState.START;
        SetupBattle(list_GameObjectsOnly);
    }
    
    public void SetupBattle(List<GameObject> _list_GameObjectsOnly)
    {
        SetUnitList(_list_GameObjectsOnly);
        
        for (int i = 0; i < list_GameObjectsOnly.Count; i++) //Setup each unit UI & assing elements to Unit list
        {
            Unit unit = list_GameObjectsOnly[i].GetComponent<Unit>();
            list_Units.Add(unit);
            unit.healthBar.maxValue = unit.maxHP;
            unit.healthBar.value = unit.currentHP;
            unit.selectedTriangle.SetActive(false);            
        }
        ValidateUnitList(); // TODO: CHANGE THIS TO A BETTER APPROACH FOR UNIT LIST (only check enemy count and ally count)
        if (!bool_IsUnitListValidated) { return; }
        list_Units.Sort((x, y) => x.iniciative.CompareTo(y.iniciative)); //sorts the list by iniciative
        list_GameObjectsOnly.Clear();

    }

    public void ClearListsAndCounters()
    {
        list_GameObjectsOnly.Clear();
        list_Units.Clear();
        enemyCount = 0; allyCount = 0;
    }

    private bool ValidateGameObjectList()
    {
        bool_IsGameObjectListValidated = true;
        if (list_GameObjectsOnly.Count < 1) { Debug.LogError($"{nameof(list_GameObjectsOnly)} is empty, did you forget to fill it from another script?"); bool_IsGameObjectListValidated = false; }
        if (list_GameObjectsOnly.Count < 2)  { Debug.LogError($"Cannot set up a battle if { nameof(list_GameObjectsOnly) } doesn't have at least two unit game objects | did you forget to fill the list correctly?"); bool_IsGameObjectListValidated = false; }
        return bool_IsGameObjectListValidated;
    }
    public bool ValidateUnitList()
    {
        foreach (Unit unit in list_Units)
        {
            if (unit.boolIsEnemy) { enemyCount++; } else { allyCount++; }
        }
        if (enemyCount < 1 || allyCount < 1)
        {
            Debug.LogWarning($"Cannot set up a battle if there are no Allies or Enemies | { nameof(list_Units) } has not enough objects with boolIsEnemy set to true or false");
            bool_IsUnitListValidated = false;
            return bool_IsUnitListValidated;
        }
        bool_IsUnitListValidated = true;
        return bool_IsUnitListValidated;
    }
    public void SetUnitList(List<GameObject> _list_GameObjectsOnly)
    {
        ClearListsAndCounters();
        ValidateGameObjectList();
        if (!bool_IsGameObjectListValidated) { return; }
        list_GameObjectsOnly = _list_GameObjectsOnly;
        ValidateUnitList();
    }
    public List<Unit> GetUnitList() 
    {
        ValidateUnitList();
        return list_Units; 
    }

    public void AddUnitToList(GameObject _unitGameObject)
    {
        list_Units.Add(_unitGameObject.GetComponent<Unit>());
        list_Units.Sort((x, y) => x.iniciative.CompareTo(y.iniciative)); //sorts the list by iniciative
        ValidateUnitList();
    }

    public void AddUnitsToList(List<GameObject> _unitGameObjectList)
    {
        foreach (GameObject _unitGameObject in _unitGameObjectList)
        {
            list_Units.Add(_unitGameObject.GetComponent<Unit>());
        }
        ValidateUnitList();
        list_Units.Sort((x, y) => x.iniciative.CompareTo(y.iniciative)); //sorts the list by iniciative
    }
}
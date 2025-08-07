using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


// Enum representing the current state of the battle
public enum BattleState { DEFAULT, START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    #region VARIABLES
    // === SINGLETON INSTANCE ===
    // Static reference to the one and only instance of BattleSystem
    public static BattleSystem Instance { get; private set; }

    // === BATTLE STATE ===
    public BattleState state;

    // === UNIT DATA ===
    
    public List<GameObject> list_GameObjectsOnly = new List<GameObject>(); // GameObjects that have Units
    public List<Unit> list_Units = new List<Unit>();                       // Unit components from those GameObjects
    public int enemyCount = 0, allyCount = 0;                                      // Used for validation
    public int indexCurrentUnit = 0;                                               // Used for state changes and turn based actions
    private GameObject hoveredEnemy;
    private GameObject lastHoveredEnemy = null;

    // === VALIDATION FLAGS ===
    private bool bool_IsGameObjectListValidated = false;
    private bool bool_IsUnitListValidated = false;

    // === PLAYER FIGHTING FLAGS ===
    private bool bool_PlayerIsPhysicalAttacking = false;
    private bool bool_PlayerIsOffensiveAttacking = false;
    private bool bool_PlayerPickedTarget = false;

    // === BAR CAMERAS ===
    private BarRoomCameraHolder barRoomCameraHolder;

    // === WIN/LOSE SCREENS ===
    [SerializeField]private GameObject screenWin;
    [SerializeField]private GameObject screenLose;
    #endregion

    #region  CORE SETUP AND LOOP FUNCTIONS (Awake, Start, Update, Setup)

    // === SINGLETON ENFORCEMENT ===
    private void Awake()
    {
        // Enforce the singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // If another instance exists, destroy this one
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this.gameObject); // Optional: makes the instance persist across scenes
    }

    // === INITIALIZATION ===
    void Start()
    {
        //SetupBattle(list_GameObjectsOnly); // Begins setup using the provided GameObject list
        SetState(BattleState.DEFAULT);
        // GETTING THE CAMERAS
        try
        {
            barRoomCameraHolder = FindAnyObjectByType<BarRoomCameraHolder>();
            if (barRoomCameraHolder == null)
            {
                Debug.LogWarning("BarRoomCameraHolder not found in the scene.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error while trying to find BarRoomCameraHolder: " + ex.Message);
        }
    }

    private void Update()
    {
        if (state == BattleState.PLAYERTURN)
        {
            if (!bool_PlayerIsOffensiveAttacking && !bool_PlayerIsPhysicalAttacking) { list_Units[indexCurrentUnit].ActionButtons.SetActive(true); }
            if ((bool_PlayerIsOffensiveAttacking || bool_PlayerIsPhysicalAttacking) && !bool_PlayerPickedTarget)
            {
                DetectEnemyWithRaycast();
                list_Units[indexCurrentUnit].ActionButtons.SetActive(false);
            }
        }
    }

    // === MAIN BATTLE SETUP ===
    public void SetupBattle(List<GameObject> _list_GameObjectsOnly)
    {
        SetUnitList(_list_GameObjectsOnly); // Clears old data and sets new unit list

        if (!bool_IsUnitListValidated) return;

        // Sort by initiative in descentding order (turn order) 
        list_Units.Sort((x, y) => y.iniciative.CompareTo(x.iniciative));

        // Clear temp GameObject list
        list_GameObjectsOnly.Clear();

        // Set battle state based on the unit with the most iniciative
        SetStateByCurrentUnitDefinition();
        if (state == BattleState.ENEMYTURN) { StartCoroutine(EnemyAttack()); }
    }
    #endregion

    #region LISTS FUNCTIONS
    // === RESET ALL STATE ===
    public void ClearListsAndCounters()
    {
        list_GameObjectsOnly.Clear();
        list_Units.Clear();
        enemyCount = 0;
        allyCount = 0;
    }

    // === VALIDATE GAMEOBJECT LIST ===
    private bool ValidateGameObjectList()
    {
        bool_IsGameObjectListValidated = true;

        if (list_GameObjectsOnly.Count < 1)
        {
            Debug.LogError($"{nameof(list_GameObjectsOnly)} is empty. Did you forget to assign it?");
            bool_IsGameObjectListValidated = false;
        }

        if (list_GameObjectsOnly.Count < 2)
        {
            Debug.LogError($"{nameof(list_GameObjectsOnly)} must contain at least two units.");
            bool_IsGameObjectListValidated = false;
        }

        return bool_IsGameObjectListValidated;
    }

    // === VALIDATE UNIT LIST (ALLY/ENEMY COUNT) ===
    public bool ValidateUnitList()
    {
        enemyCount = 0;
        allyCount = 0;

        foreach (Unit unit in list_Units)
        {
            if (unit.boolIsEnemy) enemyCount++;
            else allyCount++;
        }

        if (enemyCount < 1 || allyCount < 1)
        {
            Debug.LogWarning("Not enough allies or enemies to start battle.");
            bool_IsUnitListValidated = false;
            return false;
        }

        bool_IsUnitListValidated = true;
        return true;
    }

    // === SET NEW GAMEOBJECT LIST ===
    public void SetUnitList(List<GameObject> _list_GameObjectsOnly)
    {
        ClearListsAndCounters(); // Reset
        list_GameObjectsOnly = _list_GameObjectsOnly;

        if (!ValidateGameObjectList()) return;
        // Initialize unit data
        for (int i = 0; i < list_GameObjectsOnly.Count; i++)
        {
            Unit unit = list_GameObjectsOnly[i].GetComponent<Unit>();
            list_Units.Add(unit);
        }


        ValidateUnitList();
    }

    // === ACCESSOR: GET CURRENT UNIT LIST ===
    public List<Unit> GetUnitList()
    {
        ValidateUnitList(); // Ensure list is up-to-date
        return list_Units;
    }

    // === ADD A SINGLE UNIT (e.g., reinforcement) ===
    public void AddUnitToList(GameObject _unitGameObject)
    {
        Unit _unit = _unitGameObject.GetComponent<Unit>();
        list_Units.Add(_unit);

        _unit.healthBar.maxValue = _unit.maxHP;
        _unit.healthBar.value = _unit.currentHP;
        _unit.selectedTriangle.SetActive(false);

        list_Units.Sort((x, y) => x.iniciative.CompareTo(y.iniciative)); // Keep initiative order
        ValidateUnitList();
    }

    // === ADD MULTIPLE UNITS AT ONCE ===
    public void AddUnitsToList(List<GameObject> _unitGameObjectList)
    {
        Unit _unit;
        foreach (GameObject _unitGameObject in _unitGameObjectList)
        {
            list_Units.Add(_unitGameObject.GetComponent<Unit>());

            _unit = _unitGameObject.GetComponent<Unit>();
            _unit.healthBar.maxValue = _unit.maxHP;
            _unit.healthBar.value = _unit.currentHP;
            _unit.selectedTriangle.SetActive(false);
        }
        list_Units.Sort((x, y) => x.iniciative.CompareTo(y.iniciative));
        ValidateUnitList();
    }

    public void RemoveUnitFromList(int _indexToRemove) 
    {
        Unit unit = list_Units[_indexToRemove];
        if (unit.boolIsEnemy) { enemyCount--; }
        else { allyCount--; }
        list_Units.RemoveAt(_indexToRemove);
    }
    #endregion

    #region SET STATE FUNCTIONS
    public void SetState(BattleState _state)
    {
        state = _state;
    }

    public void SetStateByCurrentUnitDefinition()
    {
        bool_PlayerIsOffensiveAttacking = false;
        bool_PlayerIsPhysicalAttacking = false;
        bool_PlayerPickedTarget = false;

        if (list_Units[indexCurrentUnit].boolIsEnemy)
        {
            SetState(BattleState.ENEMYTURN);
        }
        else
        {
            SetState(BattleState.PLAYERTURN);
        }
        list_Units[indexCurrentUnit].camera.transform.gameObject.SetActive(true);
        CameraSwitcher.SwitchCamera(list_Units[indexCurrentUnit].camera);
        
    }
    #endregion

    #region UNIT TURN FUNCTIONS  ( Raycast selection, Buttons, Player and Enemy Attack, List of Alive Oponents and Next Turn Function )
    void DetectEnemyWithRaycast()
    {
        hoveredEnemy = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hoveredEnemy = hit.collider.gameObject;

                // If we changed enemy, disable the triangle on the previous one
                if (hoveredEnemy != lastHoveredEnemy)
                {
                    if (lastHoveredEnemy != null)
                    {
                        lastHoveredEnemy.GetComponent<Unit>().selectedTriangle.SetActive(false);
                    }

                    // Enable the triangle on the new one
                    hoveredEnemy.GetComponent<Unit>().selectedTriangle.SetActive(true);
                    lastHoveredEnemy = hoveredEnemy;
                }

                // If left click, perform attack
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(PlayerAttack());
                }

                return; // Early return so we don't disable the triangle right after
            }
        }
        // If we're here, we're not hovering any enemy — turn off triangle if one was previously on
        if (lastHoveredEnemy != null)
        {
            lastHoveredEnemy.GetComponent<Unit>().selectedTriangle.SetActive(false);
            lastHoveredEnemy = null;
        }
    }

    public void OnPhysicalAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        bool_PlayerIsPhysicalAttacking = true;
    }
    public void OnOffenseAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        bool_PlayerIsOffensiveAttacking = true;
    }

    IEnumerator PlayerAttack()
    {
                   
        CameraSwitcher.SwitchCamera(barRoomCameraHolder.mainCamera); // If we are in the bar change to its main camera
        
        bool_PlayerPickedTarget = true;

        Unit targetUnit = hoveredEnemy.GetComponent<Unit>();
        if (bool_PlayerIsPhysicalAttacking) { list_Units[indexCurrentUnit].Action_PhysicalAttack(targetUnit); }
        if (bool_PlayerIsOffensiveAttacking) { list_Units[indexCurrentUnit].Action_OffenseAttack(targetUnit); }
        
        StartCoroutine(NextTurn());

        yield return null;
    }


    IEnumerator EnemyAttack()
    {        
        List<Unit> allyList = GetAliveOponents();
        int attackType = UnityEngine.Random.Range(0,1);
        Unit victim = allyList[UnityEngine.Random.Range(0,allyList.Count)];
        victim.selectedTriangle.SetActive(true);
        yield return new WaitForSeconds(2f);

        switch (attackType)
        {
            case 0:
                list_Units[indexCurrentUnit].Action_PhysicalAttack(victim);
                break;
            case 1:
                list_Units[indexCurrentUnit].Action_OffenseAttack(victim);
                break;
            default:
                Debug.LogError($"Something went wrong, enemy unit could not decide it's attack type | {nameof(attackType)}: {attackType}");
                break;
        }
        CameraSwitcher.SwitchCamera(barRoomCameraHolder.mainCamera);

        victim.selectedTriangle.SetActive(false);

        
        StartCoroutine(NextTurn());
        yield return null;
    }

    private List<Unit> GetAliveOponents()
    {
        List<Unit> listAlive = new List<Unit>();
        if (list_Units[indexCurrentUnit].boolIsEnemy) // if unit is enemy get players
        {
            foreach (Unit unit in list_Units)
            {
                if (!unit.boolIsEnemy) listAlive.Add(unit);
            }
        }
        else // else get enemies
        {
            foreach (Unit unit in list_Units)
            {
                if (unit.boolIsEnemy) listAlive.Add(unit);
            }
        }

        return listAlive;
    }
    IEnumerator NextTurn()
    {
        bool turnInterrupted = false;

        int index = 0;
        foreach (Unit unit in list_Units.ToArray())
        {
            if (unit.currentHP <= 0)
            {
                if (index <= indexCurrentUnit) { indexCurrentUnit--; } // Readjust index
                StartCoroutine(unit.Die());
                RemoveUnitFromList(index);

                // If we detect win/lose condition inside loop later, we can use this flag
            }
            index++;
        }

        if (enemyCount <= 0)
        {
            enemyCount = 0;
            SetState(BattleState.WON);
            screenWin.SetActive(true);
            turnInterrupted = true;
        }
        else if (allyCount <= 0)
        {
            allyCount = 0;
            SetState(BattleState.LOST);
            screenLose.SetActive(true);
            turnInterrupted = true;
        }

        if (turnInterrupted) yield break;

        indexCurrentUnit++;
        indexCurrentUnit = indexCurrentUnit % list_Units.Count;

        yield return new WaitForSeconds(2f);

        SetStateByCurrentUnitDefinition();

        if (state == BattleState.ENEMYTURN)
        {
            StartCoroutine(EnemyAttack());
        }
    }




    #endregion

}
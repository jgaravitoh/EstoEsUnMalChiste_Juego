using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetUpExampleBattle : MonoBehaviour
{
    [SerializeField] private List<GameObject> battleParticipants = new List<GameObject>();
    private void Start()
    {
        Invoke(nameof(SetStartState), 2f);
        Invoke(nameof(CallSetUpBattleSystem), 5f);
    }
    public void SetStartState() 
    {
        BattleSystem.Instance.SetState(BattleState.START);
    }
    public void CallSetUpBattleSystem()
    {
        BattleSystem.Instance.SetupBattle(battleParticipants);
    }
}

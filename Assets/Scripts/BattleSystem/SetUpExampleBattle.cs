using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetUpExampleBattle : MonoBehaviour
{
    [SerializeField] private BarRoomCameraHolder barRoomCameraHolder;
    [SerializeField] private List<GameObject> battleParticipants = new List<GameObject>();
    private void Start()
    {
        barRoomCameraHolder = FindAnyObjectByType<BarRoomCameraHolder>();
        CameraSwitcher.SwitchCamera(barRoomCameraHolder.topDownSillyCamera);
        Invoke(nameof(SetStartState), 2f);
        
        Invoke(nameof(CallSetUpBattleSystem), 5f);

    }
    public void SetStartState() 
    {
        BattleSystem.Instance.SetState(BattleState.START);
        CameraSwitcher.SwitchCamera(barRoomCameraHolder.mainCamera);
    }
    public void CallSetUpBattleSystem()
    { 
        BattleSystem.Instance.SetupBattle(battleParticipants);
    }
}

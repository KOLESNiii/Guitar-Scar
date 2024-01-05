using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUILoader : MonoBehaviour
{
    //Needed to load the rest of the battle scene after the battle UI is loaded
    void Start()
    {
        BattleUIManager.loadRestOfBattleScene();
    }
}

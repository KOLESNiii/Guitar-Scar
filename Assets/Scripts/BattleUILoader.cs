using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUILoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BattleUIManager.loadRestOfBattleScene();
    }
}

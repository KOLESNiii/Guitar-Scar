using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Interface for data saving
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}

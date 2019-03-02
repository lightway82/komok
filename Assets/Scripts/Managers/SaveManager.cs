using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Осуществляет загрузку и сохранение игровых достижений и продвижений игрока
/// </summary>
public class SaveManager: BaseGameManager
{
//TODO возможно стоит сохраняться при изменениях значений, чтобы при выходе изер не потерял свои изменения
    public int CurrentLevel { get; set; }

    public override void Initializations()
    {
        
        Debug.Log("Loading Save Manager");

        CurrentLevel = 5;//заглушка
    }
}

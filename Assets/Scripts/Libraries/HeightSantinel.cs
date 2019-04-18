using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Передается флаг на земле или нет и текущая позиция по оси y
/// </summary>
public class GroundEvent : UnityEvent<bool, float>
{
}

/// <summary>
/// Отслеживает точку отрыва от земли
/// нужно посылать объекту события каждый кадр, считывать можно
/// </summary>
public class HeightSantinel
{
    /// <summary>
    /// В полете или на земле.
    /// 
    /// </summary>
    public bool IsInFly { get; private set; }

    /// <summary>
    /// Верное значение только после приземления тела <see cref="IsInFly"/>
    /// </summary> 
    public float EndFlyHeight { get; private set; }

    /// <summary>
    /// Высота с которой упало тело(от точки перегиба траектории падения)
    /// Максимальная высота - это относительная высота, не абсолютная.
    /// Можно считывать в любой момент, но наиболее точное значение после  события о приземлении
    /// </summary>
    public float MaxHeight { get; private set; }

    public float StartupHeight { get; private set; }


    

    public HeightSantinel(GroundEvent groundEvent)
    {
        groundEvent.AddListener(OnGroundedListener);
    }

   

    private void OnGroundedListener(bool isOnGround, float height)
    {
        if (IsInFly && isOnGround)
        {//конец полета 
            IsInFly = false;
            EndFlyHeight = height;

        }else  if (!IsInFly && !isOnGround)
        {    //начало полета
            IsInFly = true;
            StartupHeight =height;
            MaxHeight = height;
        }
        else
        { 
            if (height > MaxHeight) MaxHeight = height;
            
        }
        
    }

    
}

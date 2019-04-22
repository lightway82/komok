using System;
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
    /// Можно считывать в любой момент, но это всегда высота последнего падения
    /// </summary>
    public float MaxHeight
    {
        get
        {
            if (!IsInFly) return _maxHeight;
            throw new Exception("Читать высоту нужно после окончания полета");
        } 
        private set => _maxHeight = value;
    }

    public float StartupHeight { get; private set; }


    

    public HeightSantinel(GroundEvent groundEvent)
    {
        groundEvent.AddListener(OnGroundedListener);
    }


    private float _maxHeightCalc;
    private float _maxHeight;

    private void OnGroundedListener(bool isOnGround, float height)
    {
        if (IsInFly && isOnGround)
        {//конец полета 
            IsInFly = false;
            EndFlyHeight = height;

            var h  = _maxHeightCalc - EndFlyHeight;//если меньше нуля, то небы ло падение с высоты, был полет и мы уткнулись на что-то в подъеме или был взлет с падением на верхатуру
            if (h < 0) MaxHeight = 0;
            else MaxHeight = h;

        }else  if (!IsInFly && !isOnGround)
        {    //начало полета
            IsInFly = true;
            StartupHeight =height;
            _maxHeightCalc = height;
        }
        else
        { 
            if (height > _maxHeightCalc) _maxHeightCalc = height;
            
        }
        
    }

    
}

using System.Collections;
using System;

using UnityEngine;
using Action = System.Action;


public abstract class CoroutineObjectBase
{
    public MonoBehaviour Owner { get; protected set; }
    public Coroutine Coroutine { get; protected set; }

    public bool IsProcessing => Coroutine != null;

    public abstract event Action Finished;
    
    
}

public abstract class CoroutineObjectBase<U>
{
    public MonoBehaviour Owner { get; protected set; }
    public Coroutine Coroutine { get; protected set; }

    public bool IsProcessing => Coroutine != null;

    public abstract event Action<U> Finished;

    protected Result<U> result = new Result<U>(default(U));

    public class Result<U>
    {
        public Result(U value)
        {
            Value = value;
        }

        public U Value;
    }
}

/// <summary>
/// Позволяет выполнить функцию routine, как корутину. Можно установить эвент на окончание выполнения
/// В функцию будет передано Result<U>, для возврата результата. Результат вернется в эвент листенер.
/// </summary>
/// <typeparam name="T">Тип, который юуде передан функции для исполнения в корутине. Значение передается в Start</typeparam>
/// <typeparam name="U">Тип возвращаемого значения</typeparam>
public sealed class CoroutineObject<T, U> : CoroutineObjectBase<U>
{
    private Func<T, Result<U>, IEnumerator> Routine { get; }

    public override event Action<U> Finished;

    public CoroutineObject(MonoBehaviour owner, Func<T, Result<U>, IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process(T arg)
    {
        yield return Routine.Invoke(arg, result);
        Coroutine = null;
        Finished?.Invoke(result.Value);
    }

    public void Start(T arg)
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process(arg));
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}

/// <summary>
/// Выполняет функции в корутине. Возвращает через эвент значение
/// </summary>
/// <typeparam name="U">Возвращаемый тип</typeparam>
public sealed class CoroutineObjectProvider<U> : CoroutineObjectBase<U>
{
    private Func<Result<U>, IEnumerator> Routine { get; }

    public override event Action<U> Finished;

    public CoroutineObjectProvider(MonoBehaviour owner, Func< Result<U>, IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process()
    {
        yield return Routine.Invoke(result);
        Coroutine = null;
        Finished?.Invoke(result.Value);
    }

    public void Start()
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}


/// <summary>
/// Выполняет функцию в корутине, принимает при старте значение
/// </summary>
/// <typeparam name="T">Тип принимаемого значения</typeparam>
public sealed class CoroutineObjectSupplier<T> : CoroutineObjectBase
{
    private Func<T,  IEnumerator> Routine { get; }

    public override event Action Finished;

    public CoroutineObjectSupplier(MonoBehaviour owner, Func<T, IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process(T arg)
    {
        yield return Routine.Invoke(arg);
        Coroutine = null;
        Finished?.Invoke();
    }

    public void Start(T arg)
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process(arg));
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}

/// <summary>
/// Выполняет функцию в корутине
/// </summary>
public sealed class CoroutineObject :CoroutineObjectBase
{
    private Func<IEnumerator> Routine { get; }

    public override event Action Finished;

    public CoroutineObject(MonoBehaviour owner, Func<IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process()
    {
        yield return Routine.Invoke();
        Coroutine = null;
        Finished?.Invoke();
    }

    public void Start()
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}

/// <summary>
/// Выполняет функцию в корутине после истечени таймера
/// Можно установить эвент на окончание
/// </summary>
public sealed class CoroutineObjectTimer :CoroutineObjectBase
{
    private readonly float _time;
    private Action action { get; }

    public override event Action Finished;

    public CoroutineObjectTimer(MonoBehaviour owner, Action action, float time)
    {
        _time = time;
        Owner = owner;
        this.action = action;
    }

    private IEnumerator Process()
    {
        yield return Owner.InvokeDelegateOnce(action, _time);
        Coroutine = null;
        Finished?.Invoke();
    }

    public void Start()
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}

/// <summary>
/// Выполняет функцию в корутине каждый кадр, завершит после истечени таймера
/// Можно установить эвент на окончание
/// </summary>
public sealed class CoroutineObjectInterval :CoroutineObjectBase
{
    private readonly float _time;
    private readonly Action<float, float> action;

    public override event Action Finished;

    public CoroutineObjectInterval(MonoBehaviour owner, Action<float, float> action, float time)
    {
        _time = time;
        Owner = owner;
        this.action = action;
    }

    private IEnumerator Process()
    {
        yield return Owner.InvokeDelegateEveryFrameBeforeTimeOff(action, _time, null);
        Coroutine = null;
        Finished?.Invoke();
    }

    public void Start()
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }
}


public static class CoroutineHelper
{
    /// <summary>
    /// Запустить делегат один раз по истечении указанного времени
    /// </summary>
    /// <param name="mn">класс на котором будет вызываться корутина</param>
    /// <param name="func"> делегат для выполнения</param>
    /// <param name="time">время задержки в секундах</param>
    /// <returns></returns>
    public static Coroutine InvokeDelegateOnce(this MonoBehaviour mn, Action func, float time)
    {
        return mn.StartCoroutine(InvokeDelegateCor(func, time));
    }

    private static IEnumerator InvokeDelegateCor(Action func, float time)
    {
        yield return new WaitForSeconds(time);
        func();
    }

    /// <summary>
    /// Запускает делегат каждый кадр, пока таймер не выйдет.
    /// Время будет передано в функцию
    /// </summary>
    /// <param name="mn">класс на котором будет вызываться корутина</param>
    /// <param name="func"> делегат для выполнения. В аргументах будет передано текущее время отсчета и время конечное(общее время таймера) </param>
    /// <param name="time">время задержки в секундах</param>
    /// <param name="endFunc">Делегат, который ваполниться вконце</param>
    /// <returns></returns>
    public static  Coroutine InvokeDelegateEveryFrameBeforeTimeOff(this MonoBehaviour mn, Action<float, float> func, float time, Action endFunc = null)
    {
        return mn.StartCoroutine(InvokeDelegateCor(func, time, endFunc));
    }

    private static IEnumerator InvokeDelegateCor(Action<float,float> func, float time, Action endFunc)
    {
        var timer = 0f;
        while (timer <= time)
        {
            func(timer, time);
            yield return null;
            timer += Time.deltaTime;
        }

        func(timer,time);
        endFunc?.Invoke();
    }
}

/// <summary>
///
/// Кастомный waiter на который не влияет Time.timeScale
/// </summary>
public class WaitForSecondsRealtime : CustomYieldInstruction
{
    private float waitTime;
    
    public override bool keepWaiting => Time.realtimeSinceStartup < waitTime;

    public WaitForSecondsRealtime(float waitTime)
    {
        this.waitTime = Time.realtimeSinceStartup + waitTime;
    }
}
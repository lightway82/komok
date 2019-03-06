using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO возврат ошибок, возвращаемые значения, передаваемые значение в таску.
/// <summary>
/// Позволяет выполнять таски в очереди по приоритетам, работает с корутинами
/// Довольно простом, можно совершенствовать.
/// Требует экземпляр MonoBehaviour для запуска
/// </summary>
public class TaskManager
{
    public ITask CurrentTask => _currentTask;

    private ITask _currentTask;
    private List<ITask> _tasks = new List<ITask>();


    private MonoBehaviour coroutineHost;

    public TaskManager(MonoBehaviour coroutineHost)
    {
        this.coroutineHost = coroutineHost;
    }

    public void AddTask(IEnumerator taskAction, Action callback, 
        TaskPriorityEnum taskPriority = TaskPriorityEnum.Default)
    {
        var task = Task.Create(taskAction, coroutineHost, taskPriority).Subscribe(callback);

        ProcessingAddedTask(task, taskPriority);
    }

    public void Break()
    {
        _currentTask?.Stop();
    }

    public void Restore()
    {
        TaskQueueProcessing();
    }

    public void Clear()
    {
        Break();

        _tasks.Clear();
    }

    private void ProcessingAddedTask(ITask task, TaskPriorityEnum taskPriority)
    {
        switch (taskPriority)
        {
            case TaskPriorityEnum.Default:
                _tasks.Add(task);
            break;
            case TaskPriorityEnum.High:
                _tasks.Insert(0, task);
            break;
            case TaskPriorityEnum.Interrupt:
                if (_currentTask != null && _currentTask.Priority != TaskPriorityEnum.Interrupt)
                {
                    _currentTask.Stop();
                }
                _currentTask = task;
                task.Subscribe(TaskQueueProcessing).Start();
            break;
        }

        if (_currentTask != null) return;
        _currentTask = GetNextTask();
        _currentTask?.Subscribe(TaskQueueProcessing).Start();
    }

    private void TaskQueueProcessing()
    {
        _currentTask = GetNextTask();
        _currentTask?.Subscribe(TaskQueueProcessing).Start();
    }

    private ITask GetNextTask()
    {
        if (_tasks.Count <= 0) return null;
        var returnValue = _tasks[0];
        _tasks.RemoveAt(0);

        return returnValue;
    }
}
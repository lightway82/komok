using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Управляет состояниями приложения, загрузкой уровней, переходами сцен итп
/// представляет ряд состояний приложения - загрузка, гла меню, загр.экран, уровень
/// </summary>

public class AppManager: BaseGameManager
{
    public enum ApplicationState
    {
        GameLoading,//игра стартует, показываетс явступительный ролик и грузится главное меню
        MainMenu,//главное меню
        LoadingScene,//грузится уровень, показывается экран загрузки
        LevelInProcess,//осуществляется активная игра на уровне(от назружен и нет паузы)
        ApplicationPause,//только на уровне может быть включена, при этом показывается обычно меню паузы
        ApplicationExited//готовится выход из приложения
    }
    
    private ApplicationState _appState;

    private float timeScale;//для временного созранения значения при операциях со временем
    
    
    /// <summary>
    ///
    /// Жизненный цикл приложения. Этапы устанавливаются из вне
    /// НА состояния можно подписаться
    /// По умолчанию стартует с AppState.GameLoading
    /// </summary>
    public ApplicationState AppState
    {
        get => _appState;

        set
        {
            _appState = value;
            LifeCycleNotification(_appState);
        }
    }

    public void AddAppStateListener(OnLifeCycleEvent e)
    {
        AddLifeCycleListener(e);
    }

    public void RemoveAppStateListener(OnLifeCycleEvent e)
    {
        RemoveLifeCycleListener(e);
    }
    
    

    public override void Initializations()
    {
        AppState = ApplicationState.GameLoading;
        Debug.Log("Loading AppStateManager...");
    }

    /// <summary>
    /// Загружает сцену меню. После загрузки сцены поменяет стэйт на ApplicationState.MainMenu
    /// </summary>
    /// <returns>Вернет асинхронную операцию, можно показать прогресс итп</returns>
    public AsyncOperation LoadMainMenu()
    {
        var operation = SceneManager.LoadSceneAsync(1);
        operation.completed += OnMainMenuLoaded;
        return operation;
    }
    
    
    /// <summary>
    /// Загрузка главного меню 
    /// </summary>
    /// <param name="withLoadingScreen"> Через загрузочный экран или напрямую</param>
    /// <returns></returns>
    public IEnumerator LoadMainMenu(bool withLoadingScreen)
    {

       if(withLoadingScreen) yield return LoadLoadingScreenSceneWithCoroutine();
        
        var operation = SceneManager.LoadSceneAsync(1);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Main menu  is loaded.");
        AppState = ApplicationState.MainMenu;
    }

    /// <summary>
    /// Загрузка главного меню 
    /// </summary>
    /// <param name="withLoadingScreen"> Через загрузочный экран или напрямую</param>
    /// <returns></returns>
    public Coroutine LoadMainMenuWithCoroutine(bool withLoadingScreen)
    {
        return StartCoroutine(LoadMainMenu(withLoadingScreen));
    }
    
    private void OnMainMenuLoaded(AsyncOperation opp)
    {
        Debug.Log("Main menu is loaded.");
        AppState = ApplicationState.MainMenu;
        opp.completed -= OnMainMenuLoaded;
    }

    public void TogglePause()
    {
        switch (AppState)
        {
            case ApplicationState.ApplicationPause:
                //сюда можно попасть только при установке паузы и с уровня, не из меню итп. Пауза поставиться только на уровне.
                Time.timeScale = timeScale;
                AppState = ApplicationState.LevelInProcess;
                break;
            case ApplicationState.LevelInProcess:
                timeScale = Time.timeScale;//сохраним, тк может отличаться от 1 для эффектов
                Time.timeScale = 0;
                break;
        }
    }

    /// <summary>
    /// Осуществляет загрузку уровня через экран загрузки.
    /// Требуется запуск через корутину.
    /// Не передает экрану прогресс(нужно без прогресс бара, но с прогресс индикатором)
    /// </summary>
    /// <param name="index"> Индекс уровня. Уровни начинаются от 4 индекса</param>
    /// <returns></returns>
    public IEnumerator LoadLevel(int index)
    {
        if (index < 4) index = 4;
        
        var operation = SceneManager.LoadSceneAsync(3);//грузим уровень с экраном загрузки

        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Load  screen  is loaded.");
        AppState = ApplicationState.LoadingScene;
        
        
        operation = SceneManager.LoadSceneAsync(index);//грузим уровень игровой

        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Load  game level  is done.");
        AppState = ApplicationState.LevelInProcess;
        
    }

    
    /// <summary>
    /// Грузит уровень асинхронно, сам вызывает корутину. Переход через экран(сцену) загрузки
    /// </summary>
    /// <param name="index"> индекс уровня, от 4</param>
    /// <returns> Созданную корутину</returns>
    public Coroutine LoadLevelWithCoroutine(int index)
    {
       return StartCoroutine(LoadLevel(index));
    }


    /// <summary>
    /// Осуществляет загрузку уровня напрямую, без экрана загрузки.
    /// Требуется запуск через корутину.
    /// Не передает экрану прогресс(нужно без прогресс бара, но с прогресс индикатором)
    /// </summary>
    /// <param name="index"> Индекс уровня. Уровни начинаются от 4 индекса</param>
    /// <returns></returns>
    public IEnumerator LoadLevelForce(int index)
    {
        if (index < 4) index = 4;
        
        var operation = SceneManager.LoadSceneAsync(index);//грузим уровень игровой

        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Load  game level  is done.");
        AppState = ApplicationState.LevelInProcess;
        
    }
    
    /// <summary>
    /// Загружает сцену экрана заргузки
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadLoadingScreenScene()
    {
        var operation = SceneManager.LoadSceneAsync(3);//грузим уровень с экраном загрузки

        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("Load  screen  is loaded.");
        AppState = ApplicationState.LoadingScene;
        
    }

    public Coroutine LoadLoadingScreenSceneWithCoroutine()
    {
        return StartCoroutine(LoadLoadingScreenScene());
    }
    

    /// <summary>
    /// Загрузка новой игры
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadNewGame()
    {
        if (AppState != ApplicationState.MainMenu)
        {
            Debug.LogError("Вызывать LoadNewGame() можно только из главного меню!");
            yield break;
        }
        yield return StartCoroutine(LoadLoadingScreenScene());
        yield return StartCoroutine(PrepareGameStateForNewsGame());
        yield return StartCoroutine(LoadLevel(4));
    }

    
    /// <summary>
    /// Загрузка новой игры через корутину
    /// </summary>
    /// <returns></returns>
    public Coroutine LoadNewGameWithCoroutine()
    {
        return StartCoroutine(LoadNewGame());
    }


    /// <summary>
    /// Очищает сохраненный прогресс игрока для начала новой игры
    /// </summary>
    /// <returns></returns>
    public IEnumerator PrepareGameStateForNewsGame()
    {
        Debug.LogWarning("Необходимо реализовать логику обнуления стэйта игры перед началом новой игры! AppManager");
        yield return null;
    }

    /// <summary>
    /// Продолжает игру с ранее остановленного места
    /// </summary>
    /// <returns></returns>
    public IEnumerator ContinueGame()
    {
        yield return StartCoroutine(LoadLoadingScreenScene());
        yield return StartCoroutine(PrepareGameStateForContinueGame());
        //уровен загружается в уже готовый стэйт, который ранее подготовлен PrepareGameStateForContinueGame()
        yield return StartCoroutine(LoadLevel(Managers.Save.CurrentLevel));
    }

    /// <summary>
    /// Продолжает игру с ранее остановленного места
    /// </summary>
    /// <returns></returns>
    public Coroutine ContinueGameWithCoroutine()
    {
        return StartCoroutine(ContinueGame());
    }

    private IEnumerator PrepareGameStateForContinueGame()
    {
        Debug.LogWarning("Необходимо реализовать логику загрузки стэйта игры перед продолжением игры! AppManager");
        yield return null;
    }


    /// <summary>
    /// Выход из приложения
    /// Уведомит перед выходом обработчики жизненного цикла  приложения. Укажет статус ApplicationState.ApplicationExited
    /// </summary>
    public void ExitApp()
    {
        Debug.Log("Prepare Exit");
        AppState = ApplicationState.ApplicationExited;
        Debug.Log("Exit");
        Application.Quit();
    }

    
    /// <summary>
    /// Установит текужий стэйт жизненного цикла приложения.
    /// Чит. Используется только для разработки, в целях тестирования. В скриптах загрузки managers для запускауровней без требования запускать всю игру!!!
    /// </summary>
    /// <param name="state"></param>
    public void SetStateManual_DO_NOT_USE_IN_YOUR_CODE(ApplicationState state)
    {
        AppState = state;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.U2D;
using UnityEngine;



public enum SurfaceType
{
    SOFT_SNOW, MIDDLE_SNOW, HARD_SNOW, STONE, GRAVEL, ICE, GROUND, GRASS
}

[Serializable]
public class Surfaces
{
   
    [Tooltip("Мягкий снег")] public Surface softSnow;
    [Tooltip("Средний снег")] public Surface middleSnow;
    [Tooltip("Твердый  снег")] public Surface hardSnow;
    [Tooltip("Камень")] public Surface stone;
    [Tooltip("Гравий")]public Surface gravel;
    [Tooltip("Лед")] public Surface ice;
    [Tooltip("Земля")] public Surface ground;
    [Tooltip("Трава")] public Surface grass;
    [HideInInspector] public Surface[] list;
}

[Serializable]
public class SurfaceDescribe
{
    /// <summary>
    /// Вес поверхности в смешанной поверхности. Вычисляемый и обновляемый
    /// </summary>
    [HideInInspector] 
    public float weight;
    
    [Tooltip("Трение для начала движения. Ноль - это лед")]
    [Range(0,1)]
    public float staticFriction;//трение
    
    [Tooltip("Трение для движения.  Ноль - это лед")]
    [Range(0,1)]
    public float dynamicFriction;//трение
    
    [Tooltip("Упругость(также твердость). Ноль - абсолютно не упруго!")]
    [Range(0,1)]
    public float bounciness;//трение
    
    [Tooltip("Влажность")]
    public float humidity;//влажность
  

}

[Serializable]
public class Surface
{
    [Tooltip("Индексы текстурных слоев  террайна")] public int[] indexes;
    public SurfaceDescribe describe;
    [HideInInspector] public SurfaceType surfaceType;


}

public class LevelController : MonoBehaviour
{
    
    [SerializeField]
    private GameObject MenuContainerPrefab;
    private GameObject MenuContainer;
    
    [SerializeField]
    [Tooltip("Список поверхностей с индексами слоев текстур для терайна. Индексы начинаются от нуля! Если терайнов несколько, то  на всехтерайнах индексы должны соответствовать друг другу")]
    private Surfaces Surfaces;

    private PhysicMaterial _physicMaterial;

    private PlayerController _playerController;

    private Terrain _currentTerrain;

    private SurfaceDescribe[] _curentSurfaces;

    public SurfaceDescribe[] CurentSurfaces => _curentSurfaces;
    private Dictionary<int, SurfaceDescribe> _surfacesMap = new Dictionary<int, SurfaceDescribe>();
  


    public Terrain CurrentTerrain
    {
        get => _currentTerrain;
        private set => _currentTerrain = value;
    }

    private void Awake()
    {
        _physicMaterial= new PhysicMaterial();
        _playerController = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if(_playerController==null) Debug.LogError("Отстутствует Player");
       
        SetupSurfacesMap();
    }

    private void Start()
    {
        
        Managers.App.AddAppStateListener(AppStateListener);
        MenuContainer =  Instantiate(MenuContainerPrefab);
        MenuContainer.SetActive(false);
         //изменение терайна прилетает от игрока, тк именно он по терайну катается
        _playerController.OnChangeTerrainEvent.AddListener(terrain =>
        {
            _currentTerrain = terrain;
             _currentTerrain.GetComponent<TerrainCollider>().material = _physicMaterial;
            
        });
    }


    private void SetupSurfacesMap()
    {
        Surfaces.ice.surfaceType = SurfaceType.ICE;
        Surfaces.grass.surfaceType = SurfaceType.GRASS;
        Surfaces.gravel.surfaceType = SurfaceType.GRAVEL;
        Surfaces.stone.surfaceType = SurfaceType.STONE;
        Surfaces.ground.surfaceType = SurfaceType.GROUND;
        Surfaces.softSnow.surfaceType = SurfaceType.SOFT_SNOW;
        Surfaces.middleSnow.surfaceType = SurfaceType.MIDDLE_SNOW;
        Surfaces.hardSnow.surfaceType = SurfaceType.HARD_SNOW;
        
     List<Surface> _surfacesList = new List<Surface>();
     
        _surfacesList.Add(Surfaces.ice);
        _surfacesList.Add(Surfaces.grass);
        _surfacesList.Add(Surfaces.stone);
        _surfacesList.Add(Surfaces.gravel);
        _surfacesList.Add(Surfaces.ground);
        _surfacesList.Add(Surfaces.hardSnow);
        _surfacesList.Add(Surfaces.softSnow);
        _surfacesList.Add(Surfaces.middleSnow);
        Surfaces.list = _surfacesList.ToArray();
        foreach (var surface in _surfacesList)
        {
            foreach (var index in surface.indexes)
            {
                if(_surfacesMap.ContainsKey(index)) throw new Exception("Недопустимо указывать одинаковые индексы слоев для разных типов поверхности!!!");
                _surfacesMap.Add(index, surface.describe);
            } 
        }

       
       
    }

    private void calcCurrentSurfaces()
    {
        if(_currentTerrain == null) return;
        float[] layers = TerrainHelper.GetTexturesMix(_playerController.transform.position, _currentTerrain);
        SurfaceDescribe sd;

        float dynamicFriction = 0;
        float staticFriction = 0;
        float bounciness = 0;

      
        for (var i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            if (_surfacesMap.TryGetValue(i, out sd))
            {
                
                sd.weight = layer;
                dynamicFriction+=sd.dynamicFriction*layer;
                staticFriction+=sd.staticFriction*layer;
                if(sd.bounciness > bounciness)bounciness=sd.bounciness;
               
            }
             //если нет в нашем словаре нужного, то это не наш "клиент" или кто-то забыл указать индекс в скрипте. 
            
        }

        _physicMaterial.dynamicFriction = dynamicFriction;
        _physicMaterial.staticFriction = staticFriction;
        _physicMaterial.bounciness = bounciness;
        
    }




    private void AppStateListener(Enum eventtype)
    {
        
        switch ((AppManager.ApplicationState)eventtype)
        {
                case AppManager.ApplicationState.ApplicationPause:
                    PauseActions();
                break;
                case AppManager.ApplicationState.LevelInProcess:
                    OnContinuePlayActions();
                break;
        }
    }

    private void PauseActions()
    {
        Debug.Log("PAUSED");
    }

    private void OnContinuePlayActions()
    {
        Debug.Log("PLAY");
    }

    private void Update()
    {
        if (Input.GetKeyDown(Application.isEditor?KeyCode.Q:KeyCode.Escape))
        {
            ToggleMenu();
        }

        calcCurrentSurfaces();
    }

    private void ToggleMenu()
    {
        Managers.App.TogglePause();
        MenuContainer.SetActive(!MenuContainer.activeSelf);
        
    }
    
    
}

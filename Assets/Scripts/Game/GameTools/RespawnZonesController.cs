using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RespawnZonesController : MonoBehaviour
{
    private List<RespawnZone> _respawnZones=new List<RespawnZone>();
    public List<RespawnZone> RespawnZones => _respawnZones;
    
    
    private List<RespawnZone> _startZones = new List<RespawnZone>();

    private RespawnZone _lastZone;

    public RespawnZone LastZone
    {
        get
        {
            if (_lastZone == null)
            {
                var rnd = new Random();
                return _startZones[rnd.Next(_startZones.Count)];
            }
            return _lastZone;
        }

        private set { _lastZone = value; }
    }

    private void Awake()
    {
       
        RespawnZone[] resps = FindObjectsOfType<RespawnZone>();
        if (resps!=null) _respawnZones.AddRange(resps);
        foreach (var zone in RespawnZones)
        {
            if (zone.IsStartZone)
            {
                _startZones.Add(zone);
            }
            zone.RespawnZoneEvent.AddListener(OnActivateZone);
        }

        if (_startZones.Count==0)
        {
            Debug.LogError("Отсутствует стартовая зона респавна. Необходимо пометить зону галочкой Start Zone"); 
        }
        
        
    }

    private void OnActivateZone(RespawnZone zone)
    {
        LastZone = zone;
    }

    /// <summary>
    /// Ближайшая кактивная зона для респавна. Стартовые зоны всегда активны!
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public RespawnZone GetNearestActiveZone(Vector3 point)
    {
        if (RespawnZones.Count==0)
        {
            throw new Exception("Отсутствуют зоны респавна игрока. Должна быть хотя бы одна");
        }
        float minDist = float.MaxValue;
        int index = -1;
        for (int i=0; i<RespawnZones.Count; i++)
        {
            if(!RespawnZones[i].IsActivated) continue;
            var d = Vector3.Distance(RespawnZones[i].SpawnPoint, point);
            if (d < minDist)
            {
                minDist = d;
                index = i;
            }
        }

        if(index!=-1)return RespawnZones[index];
        var rnd = new  Random();
        return _startZones[rnd.Next(_startZones.Count)];
    }
    
    
    public Vector3 GetNearestActiveRespawnPoint(Vector3 point) => GetNearestActiveZone(point).SpawnPoint;
}

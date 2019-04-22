
using UnityEngine;


/// <summary>
/// Триггер, который включает указанные объекты
/// 
/// </summary>

[RequireComponent(typeof(BoxCollider))]
public class ActivateObjectZone : MonoBehaviour
{
    [Tooltip("Гейм-обьекты которыe надо активировать при проходе данного триггера")]
    [SerializeField]
    private GameObject[] targets;

    [Tooltip("Отключить объекты после прохождения зоны")]
    [SerializeField]
    private bool switchOf;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var target in targets) target.SetActive(true);
    }

  
    private void OnTriggerExit(Collider other)
    {
        if (switchOf)
        {
            foreach (var target in targets)target.SetActive(false);
        }
    }

  


#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var matrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;//уже содержит инф о повороте месе и масштабе
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = matrix;



    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
        
    }
    
#endif
}

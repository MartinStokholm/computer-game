using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    #region Tooltip
    [Tooltip("Populate this array with prefabs that you want to add to the pool, and specify the number of gameobjects to be created for each.")]
    #endregion
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private readonly Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        // This singleton gameobject will be the object pool parent
        objectPoolTransform = this.gameObject.transform;

        // Create object pools on start
        for (var i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }

    }

    /// <summary>
    /// Create the object pool with the specified prefabs and the specified pool size for each
    /// </summary>
    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        var poolKey = prefab.GetInstanceID();

        var prefabName = prefab.name; // get prefab name

        var parentGameObject = new GameObject(prefabName + "Anchor"); // create parent gameobject to parent the child objects to

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (poolDictionary.ContainsKey(poolKey)) return;
        
        poolDictionary.Add(poolKey, new Queue<Component>());

        for (var i = 0; i < poolSize; i++)
        {
            var newObject = Instantiate(prefab, parentGameObject.transform);

            newObject.SetActive(false);

            poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
        }
    }

    /// <summary>
    /// Reuse a gameobject component in the pool.  'prefab' is the prefab gameobject containing the component. 'position' is the world position for the gameobject where it should appear when enabled. 'rotation' should be set if the gameobject needs to be rotated.
    /// </summary>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var poolKey = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.Log("No object pool for " + prefab);
            return null;
        }

        // Get object from pool queue
        var componentToReuse = GetComponentFromPool(poolKey);

        ResetObject(position, rotation, componentToReuse, prefab);

        return componentToReuse;
    }

    /// <summary>
    /// Get a gameobject component from the pool using the 'poolKey'
    /// </summary>
    private Component GetComponentFromPool(int poolKey)
    {
        var componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf)
        {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    /// <summary>
    /// Reset the gameobject.
    /// </summary>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}

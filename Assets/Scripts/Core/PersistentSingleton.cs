using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public bool UnparentOnAwake = true;

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    GameObject obj = new()
                    {
                        name = typeof(T).Name + "-AutoCreated"
                    };
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake() => InitializeSingleton();

    protected virtual void OnDestroy() => instance = null;

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (UnparentOnAwake)
        {
            transform.SetParent(null);
        }

        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
            enabled = true;
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
}


using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                {
                    return null;
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (!Application.isPlaying) return;

        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[{typeof(T).Name}] Duplicate singleton instance detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}


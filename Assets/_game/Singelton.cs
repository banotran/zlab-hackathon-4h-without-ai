using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public bool DestroyonLoad = false; // Optional: Don't destroy on load (default: false)

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    Debug.LogError($"Singleton of type {typeof(T)} not found!");
                }

            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
         

            // Optional: Set _persistOnLoad to true explicitly here if needed
        }
        if (!DestroyonLoad)
            DontDestroyOnLoad(this.gameObject);

    }
}
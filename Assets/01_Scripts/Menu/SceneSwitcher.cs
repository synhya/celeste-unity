
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher I => instance;
    private static SceneSwitcher instance;
    
    public const int SceneMenu = 0;
    public const int SceneLevel1 = 1;
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }
    
    public void LoadScene(int idx)
    {
        SceneManager.LoadScene(idx); // -> level.start()
    }
}



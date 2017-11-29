using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static TempSceneRefs scene = new TempSceneRefs();
    public static float map_bound_radius = Mathf.Infinity;
    public static Texture default_texture { get { return instance.default_texture_; } }

    private static GameManager instance;

    [SerializeField] float map_bounds_radius = 500;
    [SerializeField] Texture default_texture_;



    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        map_bound_radius = map_bounds_radius;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            int scene_index = SceneManager.GetActiveScene().buildIndex;
            if (scene_index == 0)
            {
                // In menu .. Quit.
                Application.Quit();
            }
            else
            {
                // Return to menu.
                LoadScene(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    void LoadScene(int _index)
    {
        AudioManager.StopAllSFX();
        SceneManager.LoadScene(_index);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, map_bounds_radius);
    }
}

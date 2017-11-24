using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static TempSceneRefs scene = new TempSceneRefs();
    public static float map_bound_radius = Mathf.Infinity;

    private static GameManager instance;

    [SerializeField] float map_bounds_radius = 500;



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
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.StopAllSFX();
            SceneManager.LoadScene(0);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, map_bounds_radius);
    }
}

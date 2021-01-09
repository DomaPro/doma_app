using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomaContainer : MonoBehaviour
{
    private static DomaContainer _instance;
    public static DomaContainer Instance { get { return _instance; } }

    // ************************************************************************************************************

    public int TargetFrameRate = 30;




    // ************************************************************************************************************

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

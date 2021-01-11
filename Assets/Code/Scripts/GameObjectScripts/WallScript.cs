using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    DomaManager domaManager;
    PolygonCollider2D _collider;

    WallDoma wallDoma;

    bool activeOverlap = false;
    bool endOverlap = true; // pozwala kontrolować tylko jednokrotne wykonanie metody po ucieczce myszki z pola ściany

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;

        wallDoma = domaManager.GetWallDomaByGameObject(gameObject);

        _collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_collider.OverlapPoint(domaManager.mousePosition2D))
        {
            print("Point is inside collider : " + gameObject.name);

            activeOverlap = true; // Aktywacja śledzenia pozycji na ścianie
            endOverlap = false; // Wyłączenie zakończenia śledzenia
        }
        else
        {
            print("Point is NOT inside collider : " + gameObject.name);

            activeOverlap = false;
        }

        if (activeOverlap == false && endOverlap == false)
        {
            endOverlap = true;
            domaManager.HoleInWall2D = null;
        }

        if (activeOverlap == true && endOverlap == false)
        {
            endOverlap = false;
        }

        if (activeOverlap)
        {
            if (domaManager.HoleInWall2D == null)
            {
                domaManager.HoleInWall2D = new HoleInWall2D()
                {
                    objectWall2D = gameObject,
                    mousePosition = domaManager.mousePosition2D,
                    Wall2D = wallDoma.Wall2D,
                    Wall3D = wallDoma.Wall3D
                };
            }
            else
            {
                domaManager.HoleInWall2D.objectWall2D = gameObject;
                domaManager.HoleInWall2D.mousePosition = domaManager.mousePosition2D;
                domaManager.HoleInWall2D.Wall2D = wallDoma.Wall2D;
                domaManager.HoleInWall2D.Wall3D = wallDoma.Wall3D;
            }
        }
    }
}

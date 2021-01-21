using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    DomaManager domaManager;
    CurrentStatusDoma currentStatusDoma;
    PolygonCollider2D _collider;

    DWall wall;

    bool activeOverlap = false;
    bool endOverlap = true; // pozwala kontrolować tylko jednokrotne wykonanie metody po ucieczce myszki z pola ściany

    // Start is called before the first frame update
    void Start()
    {
        domaManager = DomaManager.Instance;
        currentStatusDoma = domaManager.currentStatusDoma;

        wall = currentStatusDoma.GetWallByGameObject2D(gameObject);

        _collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_collider.OverlapPoint(domaManager.mousePosition2D))
        {
            activeOverlap = true; // Aktywacja śledzenia pozycji na ścianie
            endOverlap = false; // Wyłączenie zakończenia śledzenia
        }
        else
        {
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
                    Wall = wall
                };
            }
            else
            {
                domaManager.HoleInWall2D.objectWall2D = gameObject;
                domaManager.HoleInWall2D.mousePosition = domaManager.mousePosition2D;
                domaManager.HoleInWall2D.Wall = wall;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// States a tile can be in.
public enum TileType { Normal, Obstacle, Start, End };

/* Display and store data about tiles */
public class Tile : MonoBehaviour
{
    
    private Map map;

    void Awake()
    {

    }

    void Start()
    {
        // Ensure we get an active map if some in scene may be inactive.
        Map [] maps = GetComponentsInParent<Map>();

        foreach (Map m in maps)
        {
            if (m.enabled)
            {
                map = m;
            }
        }

        RouteOverlay = false;
    }
   
    void Update()
    {
        
    }

    // Position in grid
    private Vector2Int _pos; 

    public Vector2Int Pos
    {
        get
        {
            return _pos;
        }

        set
        {
            _pos = value;
        }
    }

    // Tile state
    private TileType _state;
    public TileType State
    {
        get
        {
            return _state;
        }

        set
        {
            _state = value;
            
        }
    }

    // Overlay tile with route marker so we can see the route that has been calculated.
    public bool RouteOverlay
    {
        set
        {
            
        }
    }
    
}


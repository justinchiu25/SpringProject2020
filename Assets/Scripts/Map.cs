using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* A class to contain the tile grid and serve as a base class for the path algorithms.*/
public abstract class Map : MonoBehaviour
{

    public GameObject player;
    public GameObject enemy;
    
    [HeaderAttribute("Prefab to create individual tiles")]
    [SerializeField]
    private Tile tilePrefab = null;

    [HeaderAttribute("Grid size")]
    [SerializeField]
    protected int sizeX = 10;
    [SerializeField]
    protected int sizeY = 10;

    // Store tiles in grid.
    protected Tile[,] tileGrid;

    // Start and end of path.
    protected Vector2Int startPos;
    protected Vector2Int destPos;

    // The path to display.
    protected List<Tile> pathTiles;

    // All possible directions. 
    protected readonly Vector2Int[] allDirections =
                                           {Vector2Int.up,
                                            Vector2Int.up + Vector2Int.left,
                                            Vector2Int.up + Vector2Int.right,
                                            Vector2Int.down,
                                            Vector2Int.down + Vector2Int.left,
                                            Vector2Int.down + Vector2Int.right,
                                            Vector2Int.left,
                                            Vector2Int.right };

    // Drawing state.
    private TileType _drawState;

    public TileType DrawState
    {
        get
        {
            return _drawState;
        }
        set
        {
            _drawState = value;
        }
    }

    // Virtual so that it can be overriden in derived class.
    protected virtual void Start()
    {
        // Create simple grid test layout.
        InitGrid();
        PlaceObstacle(Random.Range(1,6),Random.Range(1,6),1,1);
        PlaceObstacle(Random.Range(1,6),Random.Range(1,6),1,1);
        PlaceObstacle(Random.Range(1,6),Random.Range(1,6),1,1);
        PlaceObstacle(Random.Range(1,6),Random.Range(1,6),1,1);

        SetStartPos(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.x));
        SetDestPos(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
    }

    // Virtual so that it can be overriden in derived class.
    protected virtual void Update()
    {
        
    }

    // Initialize empty grid.
    private void InitGrid()
    {
        SpriteRenderer spriteRenderer = tilePrefab.GetComponent<SpriteRenderer>();
        float gridW = spriteRenderer.bounds.size.x; // Get size of sprite.
            float gridH = spriteRenderer.bounds.size.y;
        Vector3 tilePos = transform.position;

        // Destroy old grid.
        if (tileGrid != null)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Destroy(tileGrid[x,y].gameObject);
                }
            }
        }

        // Create new grid.
        tileGrid = new Tile[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {            
            for (int y = 0; y < sizeY; y++)
            {
                Tile newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity);

                newTile.transform.parent = transform;
                newTile.Pos = new Vector2Int(x, y);
                tileGrid[x, y] = newTile;

                tilePos.y += gridH;
            }

            tilePos.x += gridW;
            tilePos.y = transform.position.y;
        }

        
        Vector3 boardSize = new Vector3(gridW * sizeX, gridH * (sizeY+1), 0.0f);
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = boardSize.x / boardSize.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = boardSize.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = boardSize.y / 2 * differenceInSize;
        }
        
        Camera.main.transform.position = new Vector3((boardSize.x * 0.5f) - (gridW / 2.0f), (boardSize.y / 2.0f) - (gridH / 2.0f), Camera.main.transform.position.z);
    }

    // Place an obstacle tile.
    private void PlaceObstacle(int posX, int posY, int sizeX, int sizeY)
    {
        for (int x = posX; x < posX + sizeX; x++)
        {
            for (int y = posY; y < posY + sizeY; y++)
            {
                tileGrid[x, y].State = TileType.Obstacle;
            }
        }
    }

    // Set starting position.
    private void SetStartPos(int posX, int posY)
    {
        startPos = new Vector2Int(posX, posY);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (tileGrid[x, y].State == TileType.Start) // If Start is already set, clear it.
                {
                    tileGrid[x, y].State = TileType.Normal;
                }
            }
        }

        tileGrid[posX, posY].State = TileType.Start;
    }

    // Set Destination position.
    private void SetDestPos(int posX, int posY)
    {
        //destPos = new Vector2Int(posX, posY);
        destPos = new Vector2Int(posX, posY);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (tileGrid[x, y].State == TileType.End) // If Destination is already set, clear it.
                {
                    tileGrid[x, y].State = TileType.Normal;
                }
            }
        }

        tileGrid[posX, posY].State = TileType.End;
    }


    // Is the position on the board, and not blocked?
    public bool IsValidTile(Vector2Int pos)
    {
        if ((pos.x >= 0) &&
            (pos.y >= 0) &&
            (pos.x < sizeX) &&
            (pos.y < sizeY))
        {
            if ((tileGrid[pos.x, pos.y].State == TileType.Normal) ||
                (tileGrid[pos.x, pos.y].State == TileType.End))
            {
                return true;
            }
        }

        return false;
    }

    // Remove path overlay display.
    protected void ClearDisplay()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                tileGrid[x, y].RouteOverlay = false;
            }
        }
    }

    // Display the the path overlay.
    public void DisplayRoute()
    {
        if (pathTiles != null)
        {
            for (int i = 0; i < pathTiles.Count; i++)
            {
                tileGrid[pathTiles[i].Pos.x, pathTiles[i].Pos.y].RouteOverlay = true;
            }
        }
    }

    

    // Reset grid.
    private void ClearGrid()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
               tileGrid[x, y].State = TileType.Normal;
               tileGrid[x, y].RouteOverlay = false;
            }
        }

        SetStartPos(0, 0);
        SetDestPos(sizeX-1, sizeY-1);
    }
    
    public virtual void RunPathfindingAlgorithm()
    {
        
    }


}


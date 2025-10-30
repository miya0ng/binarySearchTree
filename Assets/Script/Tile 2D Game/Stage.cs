using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private GameObject player;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIteration = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    //public Texture2D islandTexture;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;

    public Map Map
    {
        get { return map; }
    }

    private Vector3 firstTilePos;

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var pivot = firstTilePos;
        pivot.x -= tileSize.x * 0.5f;
        pivot.y += tileSize.y * 0.5f;

        var diff = worldPos - pivot;
        int x = Mathf.FloorToInt(diff.x / tileSize.x);
        int y = -Mathf.CeilToInt(diff.y / tileSize.y);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }
    public Vector3 GetTilePos(int y, int x)
    {
        var pos = firstTilePos;
        pos.x += tileSize.x * x;
        pos.y -= tileSize.y * y;
        return pos;
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }
    private void ResetStage()
    {
        bool succeed = false;
        int tryCount = 0;
        while (!succeed)
        {
            tryCount++;
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(erodePercent, erodeIteration, lakePercent,
                treePercent, hillPercent, moutainPercent, townPercent, monsterPercent);

            if (succeed)
            {
                bool castlePlaced = TryPlaceCastle();
                if (!castlePlaced)
                {
                    succeed = false;
                    Debug.LogWarning($"Castle placement failed on attempt {tryCount}");
                }
            }

            if (tryCount > 50)
            {
                Debug.LogError("Failed castle after 50 attempts.");
                break;
            }
        }

        CreateGrid();
        CreatePath();
        CreatePlayer();
    }
    private bool TryPlaceCastle()
    {
        var towns = map.tiles.Where(t => t.autoTileId == (int)TileTypes.Towns).ToArray();
        if (towns == null || towns.Length == 0)
            return false;

        //map.ShuffleTiles(towns);

        foreach (var candidate in towns)
        {
            if (candidate == null || !candidate.CanMove || candidate == map.startTile)
                continue;

            if (AstarTile(map.startTile, candidate))
            {
                candidate.autoTileId = (int)TileTypes.Castle;
                map.castleTile = candidate;
                Debug.Log($"Castle placed at tile {candidate.id}");
                return true;
            }
        }
        return false;
    }

    private void CreatePath()
    {
        foreach (var tile in path)
        {
            if (tile == null) continue;
            var go = tileObjs[tile.id];
            if (go == null) continue;

            if (go.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.color = Color.red;
            }
        }
    }
    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
    }
    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapHeight * mapWidth];

        firstTilePos = Vector3.zero;
        firstTilePos.x -= mapWidth * tileSize.x * 0.5f;
        firstTilePos.y += mapHeight * tileSize.y * 0.5f;
        var pos = firstTilePos;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.localPosition = pos;
                pos.x += tileSize.x;
                newGo.name = $"Tile ({i} , {j})";
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            pos.x = firstTilePos.x;
            pos.y -= tileSize.y;
        }
    }
    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }

        // if (tile.isVisited)
        // {
        //     if (tile.autoTileId != (int)TileTypes.Empty)
        //     {
        //         ren.sprite = islandSprites[tile.autoTileId];
        //     }
        //     else
        //     {
        //         ren.sprite = null;
        //     }
        // }
        // else
        // {
        //     ren.sprite = fowSprites[tile.autoFowId];
        // }
    }

    public int visiteRadius = 1;
    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        int radius = visiteRadius;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    continue;

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }
        radius += 1;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
 
                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                        continue;

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateAuotoFowId();
                    DecorateTile(id);
                }
            }
        }
    }

    protected int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % map.cols;
        int ay = a.id / map.cols;

        int bx = b.id % map.cols;
        int by = b.id / map.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public List<Tile> path = new List<Tile>();
    public bool AstarTile(Tile start, Tile goal)
    {
        path.Clear();
        map.ResetNodePrevious();

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, int>();
        var distances = new int[map.tiles.Length];
        var scores = new int[map.tiles.Length];

        for (int i = 0; i < distances.Length; ++i)
        {
            scores[i] = distances[i] = int.MaxValue;
        }

        distances[start.id] = start.Weight;
        scores[goal.id] = distances[start.id] + Heuristic(start, goal);
        pQueue.Enqueue(start, scores[start.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
            {
                continue;
            }

            if (currentNode == goal)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (adjacent == null) continue;

                if (!adjacent.CanMove|| visited.Contains(adjacent))
                {
                    continue;
                }

                var newDistance = distances[currentNode.id] + adjacent.Weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    scores[adjacent.id] = distances[adjacent.id] + Heuristic(adjacent, goal);
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, scores[adjacent.id]);
                }
            }
        }


        if (!success)
        {
            return false;
        }

        Tile step = goal;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StopAllCoroutines();
            ResetStage();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));
            if (map == null) return;
            var target = ScreenPosToTileId(Input.mousePosition);
            if (AstarTile(map.tiles[WorldPosToTileId(player.transform.position)], map.tiles[target]))
            {
                //StopAllCoroutines();
                //StartCoroutine(PlayerMove());

                cts = new CancellationTokenSource();
                PlayerMove(cts.Token).Forget();
            }
        }
    }

    private CancellationTokenSource cts;
    public async UniTask PlayerMove(CancellationToken token)
    {
        var clonePath = path;

        foreach ( var nextTile in clonePath)
        {
            //var temp = map.startTile;
            //map.startTile = nextTile;
            //map.tiles[nextTile.id] = temp;

            player.transform.position = GetTilePos(nextTile.id);

            await UniTask.Delay(1, cancellationToken: token);
        }
        //OnTileVisited(clonePath[clonePath.Count - 1]);
    }

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}

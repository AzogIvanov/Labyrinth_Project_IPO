using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject doorPrefab;
    public GameObject wallPrefab;

    public GameObject entrancePrefab;
    public GameObject exitPrefab;

    Vector2Int entrancePos;
    Vector2Int exitPos;

    public int maxRooms = 20;
    public Vector2Int startPosition = Vector2Int.zero;

    [Range(0f, 1f)]
    public float extraDoorChance = 0.15f;

    [Tooltip("Posición de inicio")]
    [Range(0, 100)]
    public int entranceIndex = 0;

    Dictionary<Vector2Int, Room> rooms = new();
    HashSet<(Vector2Int, Vector2Int)> connections = new();
    List<Vector2Int> roomOrder = new List<Vector2Int>();

    void Start()
    {
        maxRooms = MazeSettings.maxRooms;
        entranceIndex = Mathf.Clamp(MazeSettings.entranceIndex, 0, maxRooms - 1);

        Generate();
    }


    public void Generate()
    {
        rooms.Clear();
        connections.Clear();
        roomOrder.Clear();

        int i = 0;
        int maxAttempts = maxRooms * 10;
        int attempts = 0;

        while (i < maxRooms && attempts < maxAttempts)
        {
            attempts++;
            Vector2Int next;

            if (i == 0)
                next = startPosition;
            else
                next = roomOrder[i - 1] + GetRandomDirection();

            if (!rooms.ContainsKey(next))
            {
                CreateRoom(next);
                roomOrder.Add(next);

                if (i == 0)
                    entrancePos = next;

                if (i > 0)
                    connections.Add(Normalize(roomOrder[i - 1], next));

                i++;
            }
            else
            {
                continue;
            }
        }


        entrancePos = roomOrder[Mathf.Clamp(entranceIndex, 0, roomOrder.Count - 1)];
        exitPos = roomOrder[roomOrder.Count - 1];

        foreach (var pos in rooms.Keys)
        {
            foreach (var n in GetNeighbors(pos))
            {
                var key = Normalize(pos, n);
                if (!connections.Contains(key) && Random.value < extraDoorChance)
                    connections.Add(key);
            }
        }

        SetupRooms();
        SpawnEntranceAndExit();
    }

    void CreateRoom(Vector2Int pos)
    {
        Vector3 worldPos = new Vector3(pos.x * 3, 0, pos.y * 3);
        GameObject obj = Instantiate(roomPrefab, worldPos, Quaternion.identity);
        rooms.Add(pos, obj.GetComponent<Room>());
    }

    void SetupRooms()
    {
        foreach (var pair in rooms)
        {
            Vector2Int pos = pair.Key;
            Room room = pair.Value;

            room.Setup(
                HasConnection(pos, Vector2Int.up),
                HasConnection(pos, Vector2Int.down),
                HasConnection(pos, Vector2Int.right),
                HasConnection(pos, Vector2Int.left),
                doorPrefab, wallPrefab
            );
        }
    }

    bool HasConnection(Vector2Int a, Vector2Int dir)
    {
        return rooms.ContainsKey(a + dir) &&
               connections.Contains(Normalize(a, a + dir));
    }

    (Vector2Int, Vector2Int) Normalize(Vector2Int a, Vector2Int b)
    {
        return a.x < b.x || (a.x == b.x && a.y < b.y) ? (a, b) : (b, a);
    }

    Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    Vector2Int GetRandomDirection()
    {
        return dirs[Random.Range(0, dirs.Length)];
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> result = new();
        foreach (var d in dirs)
            if (rooms.ContainsKey(pos + d))
                result.Add(pos + d);
        return result;
    }

    void SpawnEntranceAndExit()
    {
        Vector3 entrancePos3D = rooms[entrancePos].transform.position + Vector3.up * 0.1f;
        Vector3 exitPos3D = rooms[exitPos].transform.position + Vector3.up * 0.1f;

        Instantiate(entrancePrefab, entrancePos3D, Quaternion.identity);
        Instantiate(exitPrefab, exitPos3D, Quaternion.identity);
    }
}

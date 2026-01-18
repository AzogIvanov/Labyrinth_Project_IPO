using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform north;
    public Transform south;
    public Transform east;
    public Transform west;

    public void Setup(bool n, bool s, bool e, bool w,
                      GameObject door, GameObject wall)
    {
        Create(north, n, door, wall, Vector3.forward);
        Create(south, s, door, wall, Vector3.back);
        Create(east, e, door, wall, Vector3.right);
        Create(west, w, door, wall, Vector3.left);
    }

    void Create(Transform point, bool hasDoor,
                GameObject door, GameObject wall, Vector3 dir)
    {
        GameObject obj = Instantiate(
            hasDoor ? door : wall,
            point.position,
            Quaternion.LookRotation(dir),
            transform
        );
    }
}

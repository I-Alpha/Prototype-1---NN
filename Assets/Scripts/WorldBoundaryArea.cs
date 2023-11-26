using UnityEngine;


public class WorldBoundariesArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        Globals.ConfigureWorldBoundaries(bounds);
    }

}

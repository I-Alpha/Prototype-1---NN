using UnityEngine;


public class BoundariesArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        Globals.ConfigurePlayPenBoundaries(bounds);
    }

}

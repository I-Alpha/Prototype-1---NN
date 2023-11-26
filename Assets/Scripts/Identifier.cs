using UnityEngine;

public class Identifier : MonoBehaviour
{
    public string Id { get; private set; }

    public void SetId(string id)
    {
        Id = id;
    }
}

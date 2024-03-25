using UnityEngine;

public class DestroyWithDelay : MonoBehaviour
{
    [SerializeField] private float delay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, delay);
    }
}

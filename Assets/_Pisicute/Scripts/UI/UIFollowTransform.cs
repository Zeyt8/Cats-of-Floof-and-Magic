using UnityEngine;

public class UIFollowTransform : MonoBehaviour
{
    public bool active = true;
    public FollowMode followMode;
    public enum FollowMode
    {
        Transform,
        Vector,
        None
    }
    [Tooltip("Transform to follow if Follow Mode == Transform")]
    public Transform followTransform;
    [Tooltip("Vector to follow if Follow Mode == Vector")]
    public Vector3 followVector;
    [Tooltip("Offset in the position of the Transform/Vector in world space")]
    public Vector3 offset;
    [Tooltip("Offset of the position of this RectTransform on the Canvas")]
    public Vector2 canvasOffset;
    [Tooltip("Delay before it starts being active")]
    public float beginDelay;
    [HideInInspector] public bool isOutOfScreen = false;
    [SerializeField] private OutOfScreenBehaviour outOfScreenBehaviour;
    private enum OutOfScreenBehaviour
    {
        None,
        Clamp,
    }
    [SerializeField, Tooltip("Destroy self if active, follow transform is destroyed and FollowMode is set to Transform")]
    private bool selfDestroy;

    private RectTransform rectTransform;

    private float elapsedTime = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active || followMode == FollowMode.None)
        {
            return;
        }
        if (selfDestroy && followMode == FollowMode.Transform && followTransform == null)
        {
            Destroy(gameObject);
        }

        if (elapsedTime < beginDelay)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        // follow logic
        Vector3 screenPos = Vector3.zero;
        if (followMode == FollowMode.Transform)
        {
            screenPos = Camera.main.WorldToScreenPoint(followTransform.position + offset);
        }
        else if (followMode == FollowMode.Vector)
        {
            screenPos = Camera.main.WorldToScreenPoint(followVector + offset);
        }
        // out of screen logic
        isOutOfScreen = false;
        if (outOfScreenBehaviour == OutOfScreenBehaviour.Clamp)
        {
            screenPos = Clamp(screenPos);
        }
        rectTransform.position = new Vector2(screenPos.x + canvasOffset.x, screenPos.y + canvasOffset.y);
    }

    private Vector2 Clamp(Vector2 screenPos)
    {
        if (screenPos.x < 50)
        {
            screenPos.x = 50;
            isOutOfScreen = true;
        }
        else if (screenPos.y < 50)
        {
            screenPos.y = 50;
            isOutOfScreen = true;
        }
        else if (screenPos.x > Screen.width - 50)
        {
            screenPos.x = Screen.width - 50;
            isOutOfScreen = true;
        }
        else if (screenPos.y > Screen.height - 50)
        {
            screenPos.y = Screen.height - 50;
            isOutOfScreen = true;
        }
        return screenPos;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
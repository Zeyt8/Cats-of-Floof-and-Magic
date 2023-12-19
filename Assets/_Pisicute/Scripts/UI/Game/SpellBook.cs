using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler inputHandler;

    private void OnEnable()
    {
        inputHandler.OnCancel.AddListener(Close);
    }

    private void OnDisable()
    {
        inputHandler.OnCancel.RemoveListener(Close);
    }

    public void Open(Leader leader)
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}

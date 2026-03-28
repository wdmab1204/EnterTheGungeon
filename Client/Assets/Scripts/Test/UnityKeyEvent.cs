using UnityEngine;
using UnityEngine.Events;

public class UnityKeyEvent : MonoBehaviour
{
    public KeyCode keyCode;
    public UnityEvent evt;

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
            evt.Invoke();
    }
}

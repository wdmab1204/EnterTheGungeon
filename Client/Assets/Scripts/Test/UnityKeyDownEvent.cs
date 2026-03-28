using UnityEngine;
using UnityEngine.Events;

public class UnityKeyDownEvent : MonoBehaviour
{
    public KeyCode keyCode;
    public UnityEvent evt;

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
            evt.Invoke();
    }
}

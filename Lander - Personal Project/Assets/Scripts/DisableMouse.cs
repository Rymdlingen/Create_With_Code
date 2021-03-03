using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Hide the cursor if it is inside the game window.
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

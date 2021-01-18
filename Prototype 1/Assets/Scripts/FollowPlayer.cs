using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;

    // Private variables.
    [SerializeField] Vector3 offset = new Vector3(0, 5, -8);

    // Update is called once per frame
    void LateUpdate()
    {
        // Offset the camera behind the player by adding to the player's position.
        transform.position = player.transform.position + offset;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public bool moving = false;

    void Update()
    {
      if (moving) {
        Vector3 pos = transform.position;
        pos.y -= speed;
        transform.position = pos;
      }

      if (transform.position.y <= -6f) {
        Destroy(transform.gameObject);
      }
    }
}

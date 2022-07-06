using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    Rigidbody rb;

        [SerializeField]
    WalkManager manager;

    [SerializeField]
    MovementRecognizer recognizer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // rb.transform.Translate(Camera.main.transform.forward*0.001f, Space.World);
        recognizer.isWalking = true;
        manager.movePlayer(2f);
    }
}

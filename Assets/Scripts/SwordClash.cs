using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordClash : MonoBehaviour
{
[SerializeField] Transform sword;
    private Vector2 position;

    private Vector2 swordPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        swordPosition = sword.position;
        float bob1 = Input.GetAxis("Horizontal");
        swordPosition.x += bob1*Time.deltaTime;
        //Mathf.Abs(transform.position.x - swordPosition.x);
        position = transform.position;
        float distance = Vector2.Distance(position, swordPosition);
        distance =Mathf.Clamp(distance, -10, 10);
        sword.Translate(swordPosition.x+distance*Time.deltaTime,swordPosition.y,0f);
        



    }
}

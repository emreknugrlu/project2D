using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 25;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HealthAndPosture target = collider.GetComponent<HealthAndPosture>();

        if (target != null)
        {
            // Check if the target is not blocking or parrying
            if (!target.IsBlockingOrParrying())
            {
                target.Damage(damage);
            }
            else
            {
                Debug.Log("Attack blocked or parried!");
            }
        }
    }
}

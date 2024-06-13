using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HealthAndPosture target = collider.GetComponent<HealthAndPosture>();

        if (target != null)
        {
            // Check if the target is parrying
            if (target.parrying)
            {
                Debug.Log("Player parried the attack!");
                // Get the boss's HealthAndPosture component
                HealthAndPosture bossHealthAndPosture = GetComponentInParent<HealthAndPosture>();

                if (bossHealthAndPosture != null)
                {
                    bossHealthAndPosture.GetParried(); // Apply parry effect to the boss
                }
            }
            else if (!target.IsBlockingOrParrying())
            {
                target.Damage(damage); // Apply damage if not blocking or parrying
            }
            else
            {
                Debug.Log("Attack blocked!");
            }
        }
    }
}

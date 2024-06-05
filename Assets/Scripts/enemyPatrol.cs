using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public GameObject player;
    private Rigidbody2D rb;
    private Transform currentPoint;
    public float speed = 2f;
    private bool playerInPatrolArea = false;
    public float upperYAxisTolerance = 4.0f; // Tolerance for upper y-axis
    public float lowerYAxisTolerance = 0.5f; // Tolerance for lower y-axis

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    void Update()
    {
        Vector2 point = currentPoint.position - transform.position;

        // Check if player is between pointA and pointB
        playerInPatrolArea = IsPlayerInPatrolArea();

        if (playerInPatrolArea)
        {
            // Move towards the player on the x-axis only
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
        else
        {
            // Patrolling between pointA and pointB
            if (currentPoint == pointB.transform)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
            {
                flip();
                currentPoint = pointA.transform;
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
            {
                flip();
                currentPoint = pointB.transform;
            }
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool IsPlayerInPatrolArea()
    {
        // Check if the player's x position is between pointA and pointB's x positions
        float minX = Mathf.Min(pointA.transform.position.x, pointB.transform.position.x);
        float maxX = Mathf.Max(pointA.transform.position.x, pointB.transform.position.x);
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        float enemyY = transform.position.y;

        return playerX > minX && playerX < maxX && playerY <= enemyY + upperYAxisTolerance && playerY >= enemyY - lowerYAxisTolerance;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        }
    }
}

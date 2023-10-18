using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    [Header("Parameters")]
    public LayerMask friendMask;
    public float boundryOffset;
    public float speed;
    public float rotationSpeed;
    public float sightDistance;
    public float blindSpotAngle;
    public int maxFriends;

    [Header("Collision detection")]
    public LayerMask mask;
    public float wallAngle;
    public float wallDistance;
    public float wallDistance2;
    public float wallDistance3;

    [Header("Rule priority")]
    [Range(0f, 1f)]
    public float separationPriority;
    [Range(0f, 1f)]
    public float alignmentPriority;
    [Range(0f, 1f)]
    public float cohesionPriority;
    [Range(0f, 1f)]
    public float randomPriority;

    Collider2D[] inArea;
    Collider2D self;

    List<Transform> friends = new List<Transform>();

    Vector2 currentVelocity;
    Vector2 desiredDirection;
    Vector2 separationDirection;
    Vector2 alignmentDirection;
    Vector2 cohesionDirection;
    Vector2 randomDirection;

    float interval;
    bool seeWall;

    private void Start()
    {
        self = GetComponent<Collider2D>();
        desiredDirection = degreeToVector(Random.Range(0f, 360f));
        currentVelocity = desiredDirection;
        interval = 0f;
        seeWall = false;
    }
    private void Update()
    {
        detectWall();
        detectEdge();
        Move();
        if (!seeWall)
        {
            FindDirection();
            calculateDesiredDirection();
            Look();
            RandomD();
        }
    }

    void detectWall()
    {
        float myAngle = transform.localEulerAngles.z >= 0f ? transform.localEulerAngles.z : transform.localEulerAngles.z + 360f;
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, degreeToVector(wallAngle + myAngle), wallDistance, mask);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, degreeToVector(-wallAngle + 360f + myAngle), wallDistance, mask);
        RaycastHit2D hitU = Physics2D.Raycast(transform.position, transform.up, wallDistance3, mask);
        RaycastHit2D hitLL = Physics2D.Raycast(transform.position, degreeToVector(90 + myAngle), wallDistance2, mask);
        RaycastHit2D hitRR = Physics2D.Raycast(transform.position, degreeToVector(-90 + 360f + myAngle), wallDistance2, mask);

        if (hitL.collider != null && hitR.collider == null)
        {
            seeWall = true;
            desiredDirection = transform.right;
        }
        else if (hitL.collider == null && hitR.collider != null)
        {
            seeWall = true;
            desiredDirection = -transform.right;
        }
        else if (hitL.collider != null && hitR.collider != null)
        {
            seeWall = true;
            float lDis = ((Vector2)transform.position - hitL.point).magnitude;
            float rDis = ((Vector2)transform.position - hitR.point).magnitude;

            if (lDis != rDis)
                desiredDirection = lDis > rDis ? -transform.right : transform.right;
            else
                desiredDirection = Random.Range(0f, 1f) > 0.5f ? transform.right : -transform.right;
        }
        else if(hitU.collider != null)
        {
            seeWall = true;
            desiredDirection = Random.Range(0f, 1f) > 0.5f ? transform.right : -transform.right;
        }
        else if(hitLL.collider != null && hitRR.collider == null)
        {
            seeWall = true;
            desiredDirection = transform.right;
        }
        else if (hitLL.collider == null && hitRR.collider != null)
        {
            seeWall = true;
            desiredDirection = -transform.right;
        }
        else
            seeWall = false;
    }
    void detectEdge()
    {
        Vector3 pixelPos = Camera.main.WorldToScreenPoint(transform.position);

        if (pixelPos.x < -boundryOffset)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + boundryOffset, pixelPos.y, pixelPos.z));
        else if (pixelPos.x > Camera.main.pixelWidth + boundryOffset)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f - boundryOffset, pixelPos.y, pixelPos.z));
        else if (pixelPos.y < -boundryOffset)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pixelPos.x, Camera.main.pixelHeight + boundryOffset, pixelPos.z));
        else if (pixelPos.y > Camera.main.pixelHeight + boundryOffset)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pixelPos.x, 0f - boundryOffset, pixelPos.z));
    }
    void calculateDesiredDirection()
    {
        Vector2 separationPer = separationPriority * separationDirection;
        Vector2 alignmentPer = alignmentPriority * alignmentDirection;
        Vector2 cohesionPer = cohesionPriority * cohesionDirection;
        Vector2 randomPer = randomPriority * randomDirection;
        desiredDirection = (separationPer + alignmentPer + cohesionPer + randomPer) / 4f;
    }
    void Move()
    {
        currentVelocity = Vector2.Lerp(currentVelocity.normalized, desiredDirection.normalized, rotationSpeed * Time.deltaTime);
        transform.Translate(currentVelocity.normalized * speed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg) - 90f);
    }
    void Look()
    {
        inArea = Physics2D.OverlapCircleAll(transform.position, sightDistance, friendMask);
        friends.Clear();
        if(inArea.Length > maxFriends)
        {
            for (int i = 0; i < 
                maxFriends; i++)
            {
                if (inArea[i] != self)
                {
                    Transform t = inArea[i].transform;
                    float angle = Mathf.Atan2(transform.position.y - t.position.y, transform.position.x - t.position.x);

                    if (angle > 360f)
                        angle -= 360f;
                    else if (angle < 0f)
                        angle += 360f;

                    if (!(angle > blindSpotAngle && angle < (-blindSpotAngle + 360f)))
                        friends.Add(t);
                }
            }
        }
        else
        {
            for (int i = 0; i < inArea.Length; i++)
            {
                if (inArea[i] != self)
                {
                    Transform t = inArea[i].transform;
                    float angle = Mathf.Atan2(transform.position.y - t.position.y, transform.position.x - t.position.x);

                    if (angle > 360f)
                        angle -= 360f;
                    else if (angle < 0f)
                        angle += 360f;

                    if (!(angle > blindSpotAngle && angle < (-blindSpotAngle + 360f)))
                        friends.Add(t);
                }
            }
        }
    }
    void FindDirection()
    {
        if (friends.Count != 0)
        {
            Vector2 seperationDir = Vector2.zero;
            Vector2 alignmentDir = Vector2.zero;
            Vector2 cohesionDir = Vector2.zero;

            for (int i = 0; i < friends.Count; i++)
            {
                seperationDir += (Vector2)transform.position - (Vector2)friends[i].position;
                alignmentDir += degreeToVector(friends[i].rotation.eulerAngles.z).normalized;
                cohesionDir += (Vector2)friends[i].position - (Vector2)transform.position;
            }

            seperationDir /= friends.Count;
            separationDirection = seperationDir.normalized;
            alignmentDir /= friends.Count;
            alignmentDirection = alignmentDir.normalized;
            cohesionDir /= friends.Count;
            cohesionDirection = cohesionDir.normalized;
        }
        else
        {
            separationDirection = transform.up;
            alignmentDirection = transform.up;
            cohesionDirection = transform.up;
        }
    }
    void RandomD()
    {
        interval += Time.deltaTime;

        if (interval > 0.001f)
        {
            interval = 0f;
            randomDirection = degreeToVector(Random.Range(0f, 360f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        float myAngle = transform.localEulerAngles.z >= 0f ? transform.localEulerAngles.z : transform.localEulerAngles.z + 360f;
        Debug.DrawRay(transform.position, degreeToVector(wallAngle + myAngle) * wallDistance, Color.green);
        Debug.DrawRay(transform.position, degreeToVector(-wallAngle + 360f + myAngle) * wallDistance, Color.green);
        Debug.DrawRay(transform.position, transform.up * wallDistance3, Color.green);
        Debug.DrawRay(transform.position, degreeToVector(90f + myAngle) * wallDistance2, Color.green);
        Debug.DrawRay(transform.position, degreeToVector(-90f + 360f + myAngle) * wallDistance2, Color.green);
        Debug.DrawRay(transform.position, desiredDirection.normalized * wallDistance2, Color.red);
        /*
        float myAngle = transform.localEulerAngles.z >= 0f ? transform.localEulerAngles.z : transform.localEulerAngles.z + 360f;
        Debug.DrawRay(transform.position, degreeToVector(blindSpotAngle + myAngle) * sightDistance, Color.red);
        Debug.DrawRay(transform.position, degreeToVector(-blindSpotAngle + 360f + myAngle) * sightDistance, Color.red);
        */
    }
    Vector2 degreeToVector(float degree)
    {
        return new Vector2(Mathf.Cos((degree + 90f) * Mathf.Deg2Rad), Mathf.Sin((degree + 90f) * Mathf.Deg2Rad)).normalized;
    }
}

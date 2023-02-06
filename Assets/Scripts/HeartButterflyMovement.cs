using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeartButterflyMovement : MonoBehaviour
{
    public Transform player;
    public Vector2 rangeFromPlayer;
    public float butterflyHeight;

    private Vector3 currentWaypoint;
    //public GameObject testCube;
    private GameObject cube;
    public float speed;

    public float distanceFromWaypoint;

    private Rigidbody rb;

    public float flapStrength;
    public float floatDown;
    public float adjustedStrength;
    public float adjustedGravity;
    public Renderer butterflyRender;
    [ColorUsageAttribute(true,true)]
    public Color shiningColor;

    public float timeTilShine;

    private float timer = 0;

    private bool shining;

    public GameObject particles;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        adjustedStrength = Mathf.InverseLerp( 25,20, transform.position.y);

        FindWaypoint();
    }

    void FindWaypoint()
    {
        Destroy(cube);
        Vector3 RandomPos = new Vector3(Random.Range(-rangeFromPlayer.x, rangeFromPlayer.x),
            Random.Range(-butterflyHeight, butterflyHeight),
            Random.Range(-rangeFromPlayer.y, rangeFromPlayer.y));

        currentWaypoint = player.position + RandomPos;
       //cube = Instantiate(testCube, currentWaypoint, Quaternion.identity);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
         adjustedGravity = Mathf.InverseLerp( 20  + Random.Range(-1,3),33 + Random.Range(0,3), transform.position.y);

        rb.velocity -= Vector3.up * floatDown * adjustedGravity;

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        transform.rotation = Quaternion.LookRotation(flatVelocity);

        Vector3 movementDir =   currentWaypoint - transform.position;
        
        rb.AddForce(movementDir.normalized * speed * Time.deltaTime);
        
        if (Vector3.Distance(currentWaypoint, transform.position) < distanceFromWaypoint)
        {
            FindWaypoint();
        }

        timer += Time.deltaTime;
        if (timer > timeTilShine && !shining)
        {
            shining = true;
            StartCoroutine("ShiningLerp");
            particles.SetActive(true);
        }
    }

    IEnumerator ShiningLerp()
    {
        float lerpTime = 0;
        while (lerpTime<= 1)
        {
            Color currentColor = butterflyRender.material.GetColor("_EmissionColor");
            lerpTime += Time.deltaTime*.01f;
            butterflyRender.material.SetColor("_EmissionColor", Color.Lerp(currentColor,shiningColor, lerpTime));

            yield return new WaitForEndOfFrame();
        }  
    }
    public void HeartBeatFly()
    {
        transform.GetChild(0).GetComponent<Animation>().Play();
        adjustedStrength = Mathf.InverseLerp(38+ Random.Range(-2,3),21, transform.position.y);
        rb.AddForce(Vector3.up * flapStrength * adjustedStrength* adjustedStrength);
        
    }

    IEnumerator OffsetFlap()
    {
        yield return new WaitForSeconds(Random.Range(0, .04f));

    }
}

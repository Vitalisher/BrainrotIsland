using UnityEngine;

public class WaterRising : MonoBehaviour
{

    float stayTimer;
    public float riseAmount = 6f;
    Vector3 startPos;
    Vector3 targetPos;
    public float speed = 1f;
    public float stayTime = 3f;
    public WaterRisingEvent eventWater; 

    private void Start()
    {
        startPos = gameObject.transform.position;
        targetPos = startPos + Vector3.up * riseAmount;
        eventWater.rising = false;
        eventWater.lowering = false;
    }
    private void Update()
    {
        WaterRise();
    }

    public void WaterRise()
    {
        if (eventWater.rising)
        {
            gameObject.gameObject.transform.position = Vector3.MoveTowards(gameObject.gameObject.transform.position, targetPos, speed * Time.deltaTime);
            if (Vector3.Distance(gameObject.gameObject.transform.position, targetPos) < 0.01f)
            {
                eventWater.rising = false;
                stayTimer = 0f;
            }
        }
        else if (!eventWater.lowering)
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayTime)
                eventWater.lowering = true;
        }
        else if (eventWater.lowering)
        {
            gameObject.gameObject.transform.position = Vector3.MoveTowards(gameObject.gameObject.transform.position, startPos, speed * Time.deltaTime);
        }
    }

}

using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private int playerCount = 0;
    private float countDownLength = 5;
    private float countDownCounter;
    private bool gameReady = false;
    void Start()
    {
        countDownCounter = countDownLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (countDownCounter > 0)
        {
            countDownCounter -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;

            if (playerCount == 4)
            {
                gameReady = true;
                countDownCounter = countDownLength + Random.Range(-0.9f, 0);
                // Begin Countdown.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
        }
    }
}

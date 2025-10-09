using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private int playerMax = 4;
    private int playerCount = 0;
    private float countDownLength = 1;
    private float countDownCounter = -1;
    private bool gameReady = false;
    [SerializeField] private TextMeshPro display;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (countDownCounter > 0)
        {
            countDownCounter -= Time.deltaTime;
            display.text = countDownCounter.ToString();

        }
        else if (gameReady)
        {
            //Debug.Log("Game Start!");
            Reset();
            ExampleGameManager.Instance.ChangeState(GameState.Starting);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Contact");
        if (other.CompareTag("Player"))
        {
            playerCount++;
            //Debug.Log("Player amount increased to: " + playerCount);

            if (playerCount == playerMax)
            {
                gameReady = true;
                countDownCounter = countDownLength;
                AudioSystem.Instance.PlayMusic(AudioSystem.Instance.songs[0]);
                // Begin Countdown.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            //Debug.Log("Player amount decreased to: " + playerCount);
            if (playerCount < playerMax && gameReady)
            {
                //Debug.Log("StopMusic!");
                gameReady = false;
                AudioSystem.Instance.StopMusic();
                countDownCounter = -1;
                display.text = "5";
            }
        }
    }

    public void Reset()
    {
        gameReady = false;
        playerCount = 0;
        if (display.text != "5")
        {
            display.text = "5";
            gameObject.SetActive(false);
            display.gameObject.SetActive(false);
        }
        else 
        {
            gameObject.SetActive(true);
            display.gameObject.SetActive(true);
        }
    }
}

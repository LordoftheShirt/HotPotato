using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class Potato : MonoBehaviour
{
    private Transform myHost;
    private float passCooldown = 0.25f;
    private float passCounter = -1f;

    private float potatoSpeedMultiplier = 1.1f;
    [SerializeField] private GameObject thanosSnap;

    private void Awake()
    {
        ExampleGameManager.OnBeforeStateChanged += OnStateChanged;
        //myHost = ExampleGameManager.Instance.players[Random.Range(0, 4)];
        //transform.position = myHost.position;

    }

    private void FixedUpdate()
    {
        if (passCounter > 0)
        {
            passCounter -= Time.deltaTime;
        }

        if (myHost != null)
        {
            transform.position = myHost.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && other.transform != myHost && passCounter < 0)
        {
            Debug.Log("Collided with player:" + other.gameObject.name);
            myHost = other.transform;
            passCounter = passCooldown;
            for (int i = 0; ExampleGameManager.Instance.players.Length > i; i++) 
            {
                if(myHost != ExampleGameManager.Instance.players[i])
                {
                    ExampleGameManager.Instance.players[i].GetComponent<HeroUnitBase>().PotatoSpeedMultiplier = 1f;
                }
                else
                {
                    ExampleGameManager.Instance.players[i].GetComponent<HeroUnitBase>().PotatoSpeedMultiplier = potatoSpeedMultiplier;
                }
            }
        } 
    }

    private void OnStateChanged(GameState newState)
    {
        if(newState == GameState.PlayerDeath)
        {
            ExampleGameManager.OnBeforeStateChanged -= OnStateChanged;
            Instantiate(thanosSnap, myHost.position, Quaternion.identity);
            myHost.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}

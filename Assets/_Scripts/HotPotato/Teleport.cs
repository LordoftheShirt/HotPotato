using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;

    private Teleport otherTeleporter;
    private float teleportCooldown = 0.05f;
    public float teleportCounter = -1f;
    void Start()
    {
        otherTeleporter = teleportDestination.GetComponent<Teleport>();
    }

    void Update()
    {
        if (teleportCounter > 0)
        {
            teleportCounter -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object Entered");
        if (other.CompareTag("Player") && teleportCounter < 0)
        {
            Debug.Log("Teleported!");
            teleportCounter = teleportCooldown;
            otherTeleporter.teleportCounter = teleportCooldown;
            other.transform.position = teleportDestination.position;
        }
    }
}

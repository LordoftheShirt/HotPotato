using UnityEngine;

public class SelfDestructIn : MonoBehaviour
{
    [SerializeField] private float selfDestructCount = 5f;


    // Update is called once per frame
    void Update()
    {
        selfDestructCount -= Time.deltaTime;
        if (selfDestructCount < 0 )
        {
            Destroy(gameObject);
        }
    }
}


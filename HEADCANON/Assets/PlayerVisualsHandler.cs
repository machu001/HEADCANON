using UnityEngine;

public class PlayerVisualsHandler : MonoBehaviour
{
    [SerializeField]
    PlayerMovement movementRef;
    [SerializeField]
    GameObject playerModel;
    float movementSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementSpeed = movementRef.horizontalSpeed;
        if (movementSpeed > 0.1f) playerModel.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        else if (movementSpeed < -0.1f) playerModel.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }
}

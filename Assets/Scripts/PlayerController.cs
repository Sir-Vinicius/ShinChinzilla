using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRB;
    public Transform myCamera;

    private Vector2 direction;

    public float vel = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myCamera = Camera.main.transform;
    }
    public void Mover(InputAction.CallbackContext context)
    {

        direction = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var forward = myCamera.TransformDirection(Vector3.forward);
        forward.y = 0;

        var right = myCamera.TransformDirection(Vector3.right);

        // Aqui é a direção do movimento em relação a câmera
        Vector3 move = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * new Vector3(direction.x, 0f, direction.y);

        // Aqui é o movimento do player
        myRB.MovePosition(myRB.position + move * vel * Time.fixedDeltaTime);

        // Aqui é a rotação do player para olhar na direção do movimento
        if (move != Vector3.zero)
        {
            // conferir se isso aqui ta funcionando corretamente
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 10f);
        }
    }
}

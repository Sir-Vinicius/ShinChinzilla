using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody myRB;
    public Transform myCamera;
    public Animator myAnimator;

    private Vector2 direction;

    public float vel = 5f;
    private bool isGrounded;
    [SerializeField] private Transform footPlayer;
    [SerializeField] private LayerMask playerLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>(true);
        //myCamera = Camera.main.transform;

    }
    public void Move(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            direction = context.ReadValue<Vector2>();
            myAnimator.SetBool("Move", true);  
        }
        else if (context.canceled)
        {
            direction = Vector2.zero;
            myAnimator.SetBool("Move", false);

        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            myRB.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    public void Reset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(myRB.angularVelocity);

        var forward = myCamera.TransformDirection(Vector3.forward);
        forward.y = 0;

        var right = myCamera.TransformDirection(Vector3.right);

        // Aqui é a direção do movimento em relação a câmera
        Vector3 move = Quaternion.Euler(0f, myCamera.transform.eulerAngles.y, 0f) * new Vector3(direction.x, 0f, direction.y);

        // Aqui é o movimento do player
        myRB.MovePosition(myRB.position + move * vel * Time.fixedDeltaTime);

        // Aqui é a rotação do player para olhar na direção do movimento
        if (move != Vector3.zero)
        {
            // conferir se isso aqui ta funcionando corretamente
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 10f);
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(footPlayer.position, 0.1f, playerLayer);
    }
}

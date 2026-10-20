using UnityEngine;
using UnityEngine.InputSystem;

public class ShinSwap : MonoBehaviour
{
    private Rigidbody myRB;
    private MeshFilter myMeshFilter;
    private PlayerController playerController;

    private CameraController cameraController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myMeshFilter = GetComponent<MeshFilter>();
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(cameraController.hasLanded); 
    }

    public void Swap(InputAction.CallbackContext context)
    {
       if (myRB == null || myMeshFilter == null) return;

        // verifica se está quase sem rotação angular
        if (cameraController.hasLanded)
        {
            // cria um primitive temporário apenas para obter a mesh de cápsula
            var tmp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var capMesh = tmp.GetComponent<MeshFilter>().sharedMesh;

            // aplica a mesh da cápsula ao MeshFilter do objeto
            myMeshFilter.sharedMesh = capMesh;

            // se houver um MeshCollider, atualize também
            //var mc = GetComponent<MeshCollider>();
            //if (mc != null) mc.sharedMesh = capMesh;

            // destrói o primitive temporário
            Destroy(tmp);

            playerController.enabled = true; // habilita o PlayerController

        }
    }
}

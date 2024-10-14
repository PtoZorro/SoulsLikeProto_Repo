using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("External References")]
    //[SerializeField] GameObject Camera;
    [SerializeField] Transform target;
    [SerializeField] PlayerInput playerInput;

    [Header("Stats")]
    [SerializeField] float cameraMoveSpeed;
    [SerializeField] float clampAngle;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float stickSensitivity;
    private float rotX;
    private float rotY;

    [Header("Conditional Values")]
    bool isGamepad;

    Vector2 lookInput;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;

        // Bloquar y esconder cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Manejo de rotación de cámara
        HandleRotation();
    }

    void LateUpdate()
    {
        // Seguimiento de la camara al Player
        FollowPlayer();
    }

    void HandleRotation()
    {
        // Obtener input
        float inputX = lookInput.x;
        float inputY = lookInput.y;

        // Aplicar Sensibilidad
        float sensitivity = isGamepad? stickSensitivity : mouseSensitivity;

        rotX += inputY * sensitivity * Time.deltaTime;
        rotY += inputX * sensitivity * Time.deltaTime;

        // Limitar el giro vertical de la cámara
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        // Aplicar el giro obtenido
        Quaternion localRotation = Quaternion.Euler(-rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    void FollowPlayer()
    {
        // Seguimiento de la camara al jugador con la posición del target y una velocidad asignada
        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnDeviceChange()
    {
        // Verificar si el control es mediante gamepad
        isGamepad = playerInput.currentControlScheme.Equals("Gamepad");
    }
}

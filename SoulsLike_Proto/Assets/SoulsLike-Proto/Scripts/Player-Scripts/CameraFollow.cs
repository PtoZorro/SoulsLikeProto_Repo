using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class CameraFollow : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] Transform target;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject LockPointSprite;
    [SerializeField] LayerMask enemyLayer;

    [Header("Private References")]
    GameObject enemyLocked;

    [Header("Stats")]
    [SerializeField] float cameraMoveSpeed;
    [SerializeField] float clampAngle;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float stickSensitivity;
    [SerializeField] float lockEnemyRadius;
    private float rotX;
    private float rotY;

    [Header("Conditional Values")]
    bool isGamepad;
    bool camLocked;

    [Header("Input")]
    Vector2 lookInput;

    // Start is called before the first frame update
    void Start()
    {
        // Definir valores de inicio
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
        enemyLocked = null;
        LockPointSprite.SetActive(false);

        // Bloquar y esconder cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Manejo de rotación de cámara
        if (!camLocked) HandleRotation();
        // Fijación de cámara hacia un enemigo
        else if (camLocked) LookAtEnemy();
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

    void LockDirection()
    {
        // Si la cámara no está fijada en el enemigo, fijamos
        if (!camLocked)
        {
            // Comprobamos todos los enemigos cercanos
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockEnemyRadius, enemyLayer);

            // Si hemos detectado enemigos procedemos a fijar
            if (enemiesInRange != null && enemiesInRange.Length > 0)
            {
                //LockPointSprite.SetActive(true);

                // Definimos la distancia mas cercana para saber que enemigo estará más cerca
                float closestDistance = lockEnemyRadius;

                // Comprobamos uno a uno la distancia de todos los enemigos
                foreach (Collider enemyCollider in enemiesInRange)
                {
                    // Calculamos distancia del enemigo 
                    float distance = Vector3.Distance(transform.position, enemyCollider.gameObject.transform.position);

                    // Cuando obtenemos el enemigo con la menor distancia lo seleccionamos y almacenamos
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        enemyLocked = enemyCollider.gameObject;
                    }
                }

                // Indicamos que la cámara está fijada
                camLocked = true;
            }
        }
        else if (camLocked) 
        {
            //LockPointSprite.SetActive(false);
            
            // Indicamos que la cámara ya no debe estar fijada
            camLocked = false; 
        }
    }

    void LookAtEnemy()
    {
        // La cámara apunta siempre al enemigo
        transform.LookAt(enemyLocked.transform.position);
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

    public void OnLockCam(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            LockDirection();
        }
    }

    public void OnDeviceChange()
    {
        // Verificar si el control es mediante gamepad
        isGamepad = playerInput.currentControlScheme.Equals("Gamepad");
    }
}

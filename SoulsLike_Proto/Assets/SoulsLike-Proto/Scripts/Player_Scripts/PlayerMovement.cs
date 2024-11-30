using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object Components")]
    Rigidbody rb;

    [Header("External Components")]
    [SerializeField] Transform cameraPos;
    [SerializeField] Animator animator;

    [Header("Movement Stats")]
    [SerializeField] float speed;
    [SerializeField] float rotationSmooth;

    [Header("Animation Config")]
    [SerializeField] bool isWalk;
    [SerializeField] bool isRun;
    [SerializeField] float walkAnimMinSpeed;
    [SerializeField] float minSpeedToRunAnim;

    [Header("Input values")]
    Vector2 moveInput;
    Vector3 desiredMoveDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Obtenemos las referencias necesarias
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Manejo de animaciones
        HandleAnimations();
    }

    void FixedUpdate()
    {
        // Movimiento del personaje
        HandleMovement();

        // Rotaci�n del personaje
        HandleRotation();
    }

    void HandleMovement()
    {
        // Obtener la direcci�n hacia adelante y hacia la derecha de la c�mara
        Vector3 forward = cameraPos.forward;
        Vector3 right = cameraPos.right;

        // Asegurarse de que la direcci�n hacia adelante y derecha est� en el plano horizontal
        forward.y = 0f;
        right.y = 0f;

        // Normalizar los vectores para que no se vean afectados por la inclinaci�n de la c�mara
        forward.Normalize();
        right.Normalize();

        // Ajustamos el input ya que queremos movernos en ejes X y Z
        Vector3 inputFixed = new Vector3(moveInput.x, 0, moveInput.y);

        // Convertimos el input en la direcci�n del mundo usando la direcci�n de la c�mara
        desiredMoveDirection = forward * inputFixed.z + right * inputFixed.x;

        // Calculamos la magnitud del input para ajustar la velocidad seg�n la intensidad del joystick
        float inputMagnitude = inputFixed.magnitude;

        // Calculamos el movimiento multiplicando con la velocidad
        Vector3 movement = desiredMoveDirection.normalized * speed * inputMagnitude * Time.fixedDeltaTime;

        // Calculamos cual ser� la pr�xima posici�n sumando el movimiento a la posici�n actual del RB
        Vector3 newPosition = rb.position + movement;

        // Nos movemos mediante el RB
        rb.MovePosition(newPosition);
    }

    void HandleRotation()
    {
        if (moveInput != Vector2.zero)
        {
            // Calcular la rotaci�n hacia la direcci�n en la que queremos movernos
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);

            // Interpola suavemente hacia la rotaci�n deseada usando Slerp para una rotaci�n m�s suave
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSmooth * Time.fixedDeltaTime);

            // Aplica la rotaci�n al Rigidbody
            rb.MoveRotation(smoothedRotation);
        }
    }


    void HandleAnimations()
    {
        // Obtener la magnitud del input de movimiento
        float inputMagnitude = moveInput.magnitude;

        // Seg�n la velocidad activamos animaci�n de caminar o trotar
        if (inputMagnitude > 0 && inputMagnitude < minSpeedToRunAnim)
        {
            isWalk = true;
            isRun = false;
            animator.SetFloat("walkSpeed", Mathf.Lerp(walkAnimMinSpeed, 1f, inputMagnitude / minSpeedToRunAnim)); // Velocidad de animaci�n ajustada
        }
        else if (inputMagnitude >= minSpeedToRunAnim)
        {
            isWalk = false;
            isRun = true;
            animator.SetFloat("walkSpeed", 1f); // Velocidad est�ndar para trotar/correr
        }
        else
        {
            isWalk = false;
            isRun = false;
            animator.SetFloat("walkSpeed", 0f); // Detener la animaci�n
        }

        animator.SetBool("walk", isWalk);
        animator.SetBool("run", isRun);
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}

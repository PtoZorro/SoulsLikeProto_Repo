using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] float finalDistance;
    [SerializeField] float sideRayDistance;
    [SerializeField] float distanceFromHit;
    [SerializeField] float distanceFromSideHit;
    [SerializeField] float smooth;

    [Header("Private References")]
    Vector3 cameraDir;

    // Start is called before the first frame update
    void Awake()
    {
        cameraDir = transform.localPosition.normalized;
        finalDistance = maxDistance;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // la distancia estandar a la que se situar� la c�mara
        Vector3 desiredCameraPos = transform.parent.TransformPoint(cameraDir * maxDistance);
        
        RaycastHit backHit;
        RaycastHit sideHit;

        //Linecast principal entre la distancia m�nima a la cual puede estar la c�mara y la distancia m�xima, sale desde la direcci�n contraria de la c�mara y detecta obst�culos
        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out backHit))
        {
            finalDistance = Mathf.Clamp(backHit.distance * distanceFromHit, minDistance, maxDistance);
        }
        else
        {
            // Si no hay ning�n obst�culo, la distancia final ser� la m�xima
            finalDistance = maxDistance;

            for (int i = 0; i < 4; i++)
            {
                // Si el Linecast no ha detectado haremos una comprobaci�n con 4 Raycast diferentes que salen en oblicuo desde lel mismo sitio

                // Se definen cuatro direcciones en las que saldr� un Raycast oblicuo para evitar que la c�mara entre en algunos objetos 

                int sideRayAngle = 30; // �ngulo en el cual los Raycast saldr�n respecto a la direcci�n contraria de la c�mara

                Vector3[] sideSelect;
                sideSelect = new Vector3[4];

                sideSelect[0] = Quaternion.Euler(sideRayAngle, 0, 0) * Vector3.back;
                sideSelect[1] = Quaternion.Euler(-sideRayAngle, 0, 0) * Vector3.back;
                sideSelect[2] = Quaternion.Euler(0, sideRayAngle, 0) * Vector3.back;
                sideSelect[3] = Quaternion.Euler(0, -sideRayAngle, 0) * Vector3.back;

                if (Physics.Raycast(transform.position, sideSelect[i], out sideHit, sideRayDistance))
                {
                    finalDistance = maxDistance - distanceFromSideHit;

                    // Sale del bucle si encuentra una colisi�n
                    break;
                }
            }
        }

        // Ajustamos la posici�n final tomando como Vector la direcci�n a la que tiene tiene que moverse la c�mara siempre y la distancia calculada
        Vector3 finalPosition = cameraDir * finalDistance;

        // Se aplica el movimiento con la posici�n final obtenida aplicando un ajuste para suavizar la velocidad a la que se aproxima la c�mara
        transform.localPosition = Vector3.Lerp (transform.localPosition, finalPosition, smooth * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPointBehavior : MonoBehaviour
{
    [Header("References")]
    Transform playerPos;

    [Header("Settings")]
    [SerializeField] float riseHeight;
    [SerializeField] float minDistance;
    [SerializeField] float maxSpeed;
    [SerializeField] float farPointY;

    private Vector3 initialPosition;

    void Start()
    {
        // Movemos inicialmente al punto deseado
        transform.localPosition = new Vector3(0, farPointY, 0);

        // Guardamos la posici�n inicial del objeto
        initialPosition = transform.position;

        // Encontramos al jugador en la escena 
        playerPos = GameObject.Find("Player")?.transform;
    }

    void Update()
    {
        // Calculamos la distancia entre el objeto y el jugador
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos.position);

        // Si la distancia es menor que la m�nima
        if (distanceToPlayer < minDistance)
        {
            // Proporcionalidad de la subida, m�s r�pido conforme est� m�s cerca
            // Esta funci�n nos da un valor entre 0 (fuera de rango o en el punto m�s cercano) y 1 (justo al lado del jugador)
            float t = 1 - Mathf.Clamp01(distanceToPlayer / minDistance);

            // Calculamos la nueva posici�n vertical del objeto de manera proporcional
            float newYPosition = Mathf.Lerp(initialPosition.y, initialPosition.y + riseHeight, t);

            // Subimos el objeto con una velocidad limitada
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, newYPosition, transform.position.z), maxSpeed * Time.deltaTime);
        }
        else
        {
            // Si el jugador se aleja m�s de la distancia m�nima, el objeto vuelve a su posici�n inicial
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, maxSpeed * Time.deltaTime);
        }
    }
}

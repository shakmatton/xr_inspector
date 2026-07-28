using System;
using UnityEngine;

/* Fazer:
    - criar o Physics Ray (lembrar que Interaction Toolkit é sobre interações de botões/triggers, e que futuramente em algum momento daria apoio ao Ray)
    - lembrar do uso das layers (relembrar uso "layerA && layerB") para detectar objetos que pertençam a um grupo (verificar em "Layer: Default", no Inspector)
    - raio deve ter a informação de hit/miss, e deve permitir uso de máscara (para detectar apenas os objetos desejados de uma certa layer) 
 */

namespace Scripts
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private LayerMask myLayer;
        private Ray ray;
        private float maxDistance = 5f; 

        private void Update()
        {
            ray = new Ray(transform.position, transform.forward);               // Raio criado a partir da posição da lanterna,
                                                                                               // com origem na posição dela e sentido apontado pela lanterna (eixo Z azul).
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))                         // RayCasting sem layer, apenas com (raio, hit info, maxDistance)
            {                                                                                  // Se houver um Raycast com um HIT do ray a uma distância "maxDistance"...
           
                // verificar alternativas depois...
            
            /*
             * if (hit.collider.gameObject.layer ==
                   LayerMask.NameToLayer("Highlighted Objects"))
               {
                   Debug.Log(hit.collider.name);
               }
             */
            
            /*
             * if ((myLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
             * 
             * */
             
                if (myLayer.value == LayerMask.NameToLayer("Highlighted Objects"))
                {
                    Debug.Log(hit.collider.name);                                                  // ...nome do objeto é mostrado no console
                }
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);               // ray (em debug mode) acompanha o transform da lanterna (ver red line na aba Scene)
        }

       /* private void Update()
        {
            Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
            
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000, myLayer))      
            {
                Debug.Log("Hit: " + hit.collider.name);
            };
        }*/
    }
}
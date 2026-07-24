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
        public LayerMask myLayer;

        private void Start()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray, out RaycastHit hit, 1000, myLayer);
            //hit.collider.gameObject.     // continuar...
        }
        
    }
}
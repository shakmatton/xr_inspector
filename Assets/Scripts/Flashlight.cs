using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/* Fazer:
    - criar o Physics Ray (lembrar que Interaction Toolkit é sobre interações de botões/triggers, e que futuramente em algum momento daria apoio ao Ray)
    - lembrar do uso das layers (relembrar uso "layerA && layerB") para detectar objetos que pertençam a um grupo (verificar em "Layer: Default", no Inspector)
    - raio deve ter a informação de hit/miss, e deve permitir uso de máscara (para detectar apenas os objetos desejados de uma certa layer) 
 */

namespace Scripts
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private LayerMask myLayer;                             // escolher aqui a referência da layer "Highlighted Objects"
        private Ray ray;                                                        // raio para interação XR
        private float maxDistance = 5f;                                         // alcance máximo do raio
        private bool selected = false;                                          // selected: boolean que reflete status do controle (grabbed/ not grabbed)
        
        [SerializeField] private XRBaseInteractable interactable;               // adiciona o Flashlight como objeto interactable (XR Grab Interactable)

        private void Start()                                                    // lembrar de configurar no Start() os eventos Unity abaixo
        {
            interactable.selectEntered.AddListener(OnSelect);                   
            interactable.selectExited.AddListener(OnDeselect);
        }

        private void OnSelect(SelectEnterEventArgs args)                        
        {
            // Chamado quando pega objeto Flashlight (selected)
            selected = true;
        }

        private void OnDeselect(SelectExitEventArgs args)
        {
            // Chamado quando larga objeto Flashlight (not selected)
            selected = false;
        }

        private void Update()                                                       // O Ray é recriado a cada frame para acompanhar a posição e a orientação atuais da lanterna.
        {
            if (!selected)                                                          // Se não estiver selecionado, não faz nada... senão, executa abaixo:
                return;

            ray = new Ray(transform.position, transform.forward);    // Raio criado a partir da posição da lanterna, em sua origem e sentido por ela apontado (eixo Z azul).
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))  // Lança um Raycast usando o Ray criado. O if é true se raio atinge algum collider dentro de maxDistance. 
                                                                                    
            {
                /* =========== Ver 1ª abordagem abaixo ===========
                 
                   Se layer "Highlighted Objects" for Layer 8 (por exemplo), ela retorna 8. Porém, myLayer.value não valeria 8, mas sim, 1 << 8 (ou seja, 256 (2 elevado à 8)).
                   (1 << 8 significa "deslocar 1 bit à esquerda 8 vezes"). Isso produz uma máscara contendo apenas o bit correspondente à Layer 8. 
                   Então, myLayer.value = 256 e LayerMask.nameToLayer(...) = 8.

                if (myLayer.value == LayerMask.NameToLayer("Highlighted Objects"))
                    Debug.Log(hit.collider.name);                                               // nome do objeto mostrado no console
                                                
                                                
                /* =========== Alternativa correta (versão 1) ===========
                
                
                /* a) No HIT, buscar a layer do objeto que teve se collider atingido pelo HIT, e verificar se essa layer é a mesma de "Highlighted Objects":
                   if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Highlighted Objects"))
                      Debug.Log(hit.collider.name);                                             // nome do objeto mostrado no console  */

                
                /* =========== Alternativa correta (versão 2) ===========
                 
                /* b) No HIT, a layer do objeto colidido é deslocada um bit pra esquerda (nº da Layer) vezes.
                      Aplica-se uma operação AND bit a bit (&), ou seja, "&" simples, entre os valores binários dos layers da lanterna e dos objetos na layer "Highlighted Objects".

                   Exemplo:

                   10001000
                   &
                   00001000
                   ---------
                   00001000   (int "8")

                   // Se o resultado do AND for diferente de zero, significa que a Layer do objeto faz parte da LayerMask.
                   // Nesse caso, mostra-se o nome do objeto colidido por meio de seu collider.                         */          
                
                if ((myLayer.value & (1 << hit.collider.gameObject.layer)) != 0)                // Obs.: verificar se haveria uma forma melhor de fazer isso...
                    Debug.Log("Hit: " + hit.collider.name);                                     // veja uso de myLayer, em "if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))"
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);                // ray (em debug mode) acompanha o transform da lanterna (ver red line na aba Scene)
        }
    }
}
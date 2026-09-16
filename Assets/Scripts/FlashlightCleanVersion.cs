
/* Versão do script Flashlight.cs livre de comentários (se houver comentários e/ou linhas de código novos, transferir para Flashlight.cs depois). 
   Tentar manter atualizada essa versão, para acompanhar as mudanças em Flashlight.cs.    */

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Scripts
{
    public class FlashlightCleanVersion : MonoBehaviour
    {
        [SerializeField] private LayerMask myLayer;                            
        [SerializeField] private XRBaseInteractable interactable;
        
        private Ray ray;                                                      
        private float maxDistance = 5f;                                        
        private bool flashlightSelected = false;

        private float hoverTime;                                            // tempo de hover em um objeto
        private string hoverObjectName;                                     // flag de controle sobre qual é o nome do objeto corrente    
        private string currentObj = "";                                     // ponteiro que registra último objeto a sofrer hover

        private void Start()                                                   
        {
            interactable.selectEntered.AddListener(OnSelect);                   
            interactable.selectExited.AddListener(OnDeselect);
        }

        private void OnSelect(SelectEnterEventArgs args)                        
        {
            flashlightSelected = true;
        }

        private void OnDeselect(SelectExitEventArgs args)
        {
            flashlightSelected = false;
        }

        private void Update()                                                      
        {
            if (!flashlightSelected)                                                
                return;

            ray = new Ray(transform.position, transform.forward);    

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))  
            {
                InspectionRegion inspectionRegion = hit.collider.gameObject.GetComponent<InspectionRegion>();  
                
                // Casos:
                // 1 - fazer hover num objeto qualquer, com o tempo definido para esse objeto
                // 2 - pensar no reset desse hover, caso o hover deixe o objeto
                // 3 - pensar no caso de 2 objetos grudados: como fica a questao do hover e do reset?
                
                
                /* PROBLEMA: AO SAIR E RETORNAR AO MESMO OBJETO, O TIMER NÃO RESETA! VERIFICAR (VER ARQUIVO teste.cs depois, na área de trabalho)...
                   SOLUÇÂO: ver último else do código (currentObj = "").

                 * Explicação:
                 *
                 * O currentObj = "" só é executado quando o Physics.Raycast acerta um collider que não tem InspectionRegion.
                 * Mas na prática, quando você "sai" de um objeto olhando para outro lugar, o mais comum é o raio não acertar nada dentro do maxDistance/myLayer
                 * — ou seja, Physics.Raycast retorna false, e o bloco inteiro (inclusive o else que reseta currentObj) é pulado.

                   Resultado: currentObj continua guardando o nome do último objeto mirado. 
                   Quando você volta a mirar nesse mesmo objeto, currentObj != hoverObjectName é false (porque nunca foi resetado), então o hoverTime nunca é reatribuído
                    — ele continua de onde parou (geralmente negativo/zerado), em vez de reiniciar a contagem.                 */
                
                
                if (inspectionRegion != null)                                                                    // raio aponta para ALGUM objeto (lembre-se da distância do raio)
                {
                    hoverObjectName = inspectionRegion.Inspection.inspectionName;                                   // obtenho nome do objeto atual
                    
                    if (currentObj != hoverObjectName)                                                              // se objeto "apontado" difere do objeto corrente 
                    {
                        hoverObjectName = inspectionRegion.Inspection.inspectionName;                               // obtenho nome do objeto 
                        hoverTime = inspectionRegion.Inspection.inspectionTime;                                     // obtenho tempo do objeto

                        currentObj = hoverObjectName;                                               // atualizo "ponteiro" string que "aponta" p/ nome do objeto em hover no momento
                    }

                    hoverTime -= Time.deltaTime;                                                                    // Countdown do objeto em hover

                    Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName} | hoverTime = {hoverTime}");

                    if (hoverTime <= 0)                                                                             // tempo de inspeção do objeto em hover concluído
                    {
                        InspectionManager.Instance.CheckInspection(inspectionRegion.Inspection);                    // chama o método de inspeção para aquele objeto em hover
                    }
                }
                else {                                                                                              // inspectionRegion nulo (raio aponta para fora do objeto)
                    currentObj = "";                                                                                // "ponteiro" (string) resetado para nulo
                }
            }
            else {                                                                                  // PULO DO GATO: saber que o raio não acerta nada dentro do maxDistance/myLayer
                currentObj = "";                                                                    // é preciso esse else extra para resetar o currentObj!
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);      
        }
    }
}
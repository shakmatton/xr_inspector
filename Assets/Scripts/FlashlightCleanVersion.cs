
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
        // private bool hoverFlag = false;                                  // flag de controle sobre se o objeto sofre hover ou não
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
                
                
                // PROBLEMA: AO SAIR E RETORNAR AO MESMO OBJETO, O TIMER NÃO RESETA! VERIFICAR (VER ARQUIVO teste.cs depois. na área de trabalho)...
                
                
                if (inspectionRegion != null)                                                                // objeto inspecionado
                {
                    hoverObjectName = inspectionRegion.Inspection.inspectionName;                               // tento obter objeto e seu nome
                    
                    if (hoverObjectName != null){                                                               // se objeto é válido 

                        if (currentObj != hoverObjectName){                                                     // se o ponteiro para o objeto difere do objeto válido
                            hoverTime = inspectionRegion.Inspection.inspectionTime;                             // obtenho o tempo de inspeção do objeto
                            currentObj = hoverObjectName;                                                       // atualizo o ponteiro de objeto para o objeto válido 
                        }        
        
                        hoverTime -= Time.deltaTime;                                                            // tempo do objeto inspecionado sob hover é decrementado
                        Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName}");      
                        Debug.Log($"hoverTime = {hoverTime}");

                        if (hoverTime <= 0){                                                                    // tempo de inspeção concluído
                            InspectionManager.Instance.CheckInspection(inspectionRegion.Inspection);            // chama o método de inspeção
                        }
                    }
                }
                else                                                                                            // aqui, inspectionRegion é nulo, pois raio aponta p/ fora do objeto...
                {
                    currentObj = "";                                                                            // ... então, o ponteiro é resetado p/ nulo (pois agora aponta p/ "vazio")
                }
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);      
        }
    }
}
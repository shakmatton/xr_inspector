
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

        private float hoverTime;                                    // tempo de hover em um objeto
        // private bool hoverFlag = false;                             // flag de controle sobre se o objeto sofre hover ou não
        private string hoverObjectName;                             // flag de controle sobre qual é o nome do objeto corrente    
        private string currentObj = "";                            // registra último objeto a sofrer hover

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
                
                
                // VERIFICAR POR QUE NÃO FUNCIONA O CASO 3...
                
                
                
                // TENTAR DE NOVO CONSERTAR O TRECHO ABAIXO (FAZER DEBUG.LOG() COM TEMPO DO OBJETO NO CONSOLE...)
                
                if (inspectionRegion != null)                                   // objeto inspecionado
                {

                    hoverObjectName = inspectionRegion.Inspection.inspectionName;

                    if (currentObj != hoverObjectName)
                    {
                        // if (!hoverFlag)                                                
                        // {
                        //     hoverFlag = true;                                          
                        hoverTime = inspectionRegion.Inspection.inspectionTime;
                        currentObj = hoverObjectName;
                        // }     
                    }
                    
                    // // no IF abaixo: primeiro frame detectado permite pegar o inspectionTime (duração de hover) do objeto
                    // if (!hoverFlag)                                                // inicialmente falso
                    // {
                    //     hoverFlag = true;                                          // faz com que o if não seja mais executado no próximo ciclo de Update(). 
                    //
                    //     hoverTime = inspectionRegion.Inspection.inspectionTime;    // Pega o JSON inteiro (inspectionData) do inspectionRegion. O acesso ao inspectionTime se dá via struct.
                    // }

                    // // Nesse if, asseguro a atualização do tempo de inspeção do objeto atual (deixa de ser o tempo antigo)
                    // if (currentObj != hoverObjectName)                            // se o objeto não for mais o mesmo (ainda que em estado de hover ativo)... 
                    // {
                    //     hoverTime = inspectionRegion.Inspection.inspectionTime;    // ... atualizo e considero o tempo do novo objeto 
                    // }

                    
                    // if (currentObj != hoverObjectName)
                    // {
                    //     hoverFlag = false;
                    //     currentObj = hoverObjectName;
                    //     return;
                    // }

                    hoverTime -= Time.deltaTime;                                   // tempo do polígono sob hover é decrementado
                    
                    if (hoverTime <= 0)                                            // se o tempo do polígono esgotar...
                    {
                        InspectionManager.Instance.CheckInspection(inspectionRegion.Inspection);    // ... finalmente fazer a inspeção abaixo.
                    }
                }
                else    // inspectionRegion virou nulo, pois raio apontou para fora do objeto...
                {
                    // hoverFlag = false;     // se raio sair do objeto
                    currentObj = "";
                }

                // currentObj = hoverObjectName;
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);      
        }
    }
}
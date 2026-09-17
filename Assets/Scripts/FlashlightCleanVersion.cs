
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

        public float hoverTime;                                             // tempo de hover em um objeto
        private string hoverObjectName;                                     // flag de controle sobre qual é o nome do objeto corrente    
        private string currentObj = "";                                     // ponteiro que registra último objeto a sofrer hover

        //  ========== Abaixo: seção feita em Flashlight.cs (mas deveria estar no InspectionManager.cs) ==========
        
        public Action<float> OnObjectInspection;
        public Action OnNoObjectInspection;
        
        //  ======================================================================================================

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
            
            //  ========== Abaixo: seção feita em Flashlight.cs (mas deveria estar no InspectionManager.cs) ==========
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))  
            {
                InspectionRegion inspectionRegion = hit.collider.gameObject.GetComponent<InspectionRegion>();  
                
                if (inspectionRegion != null)                                                                   
                {
                    hoverObjectName = inspectionRegion.Inspection.inspectionName;                                  
                    
                    if (currentObj != hoverObjectName)                                                              
                    {
                        hoverObjectName = inspectionRegion.Inspection.inspectionName;                               
                        hoverTime = inspectionRegion.Inspection.inspectionTime;                                    

                        currentObj = hoverObjectName;                                               
                    }

                    hoverTime -= Time.deltaTime;        
                    OnObjectInspection?.Invoke(hoverTime);
                    

                    Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName} | hoverTime = {hoverTime}");

                    if (hoverTime <= 0)                                                             
                    {
                        InspectionManager.Instance.CheckInspection(inspectionRegion.Inspection);    
                    }
                }
                // else {                                                                                              // inspectionRegion nulo (raio aponta para fora do objeto)
                //     currentObj = "";                                                                                // "ponteiro" (string) resetado para nulo
                // }
            }
            else {                                                                                  // PULO DO GATO: saber que o raio não acerta nada dentro do maxDistance/myLayer
                currentObj = "";                                                                    // é preciso esse else extra para resetar o currentObj!
                OnNoObjectInspection?.Invoke();
            }
            
            //  ========== Acima: seção feita em Flashlight.cs (mas deveria estar no InspectionManager.cs) ==========
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);      
        }
    }
}
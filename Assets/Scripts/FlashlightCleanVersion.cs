
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

        private InspectionRegion inspectionRegion;
        
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
                inspectionRegion = hit.collider.gameObject.GetComponent<InspectionRegion>();
                
                if (inspectionRegion != null)                                                    // INSPECTION_HOVER ON START              
                {
                    InspectionManagerCleanVersion.Instance.InspectionHoverStart(inspectionRegion);
                }
                // else {                                                                           // inspectionRegion nulo (raio aponta para fora do objeto)
                //     currentObj = "";                                                             // "ponteiro" (string) resetado para nulo
                // }
            }
            else {                                                                                  // INSPECTION_HOVER CANCELLED
                InspectionManagerCleanVersion.Instance.InspectionHoverCancelled(inspectionRegion);
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);      
        }
    }
}
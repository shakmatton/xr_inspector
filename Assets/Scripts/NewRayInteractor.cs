using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/*
 
     REPOSITÓRIO DE CASOS DE IMPLEMENTAÇÕES DE RAY INTERACTOR (para AR/MR e para VR) com código...
     Nenhum deles foi usado nesse projeto diretamente com código, mas sim com componente
     (solução adotada foi mais simples: adicionar o componente XR Grab Interactable ao objeto FlashLight)


 * VR Raycasting: XR Interaction Toolkit (XRI)
 * If you want to use laser-pointer style raycasting from a VR controller to interact with
 * 3D objects or world-space UI, you use the built-in XR Ray Interactor component.
 *
 * Step 1: Writing the Ray Interactor Setup Script
 * Step 2: Customizing the Targets via Code (this is later, for your other target objects' scripts)
 */

public class Flashlight : MonoBehaviour
{
    private XRRayInteractor _rayInteractor;
    
    // continuar (ou treinar um dia)...
}


/*
 * using UnityEngine;
   using UnityEngine.XR.Interaction.Toolkit;
   using UnityEngine.XR.Interaction.Toolkit.Interactors;
   
   [RequireComponent(typeof(ActionBasedController))] // Ensures input actions are attached
   public class ProgrammaticRayInteractor : MonoBehaviour
   {
       private XRRayInteractor _rayInteractor;
       private LineRenderer _lineRenderer;
       private XRInteractorLineVisual _lineVisual;
   
       void Awake()
       {
           // 1. Setup the Core Ray Interactor
           _rayInteractor = gameObject.AddComponent<XRRayInteractor>();
           
           // 2. Programmatically configure Raycast properties
           _rayInteractor.maxRaycastDistance = 30f;
           _rayInteractor.raycastMask = LayerMask.GetMask("Default", "Interactable");
           _rayInteractor.lineType = XRRayInteractor.LineType.StraightLine; // Or ProjectileCurve
           _rayInteractor.enableUIInteraction = true;
   
           // 3. Attach and configure the Visual Laser Pointer components
           _lineRenderer = gameObject.AddComponent<LineRenderer>();
           _lineVisual = gameObject.AddComponent<XRInteractorLineVisual>();
   
           ConfigureLaserVisuals();
       }
   
       void OnEnable()
       {
           // 4. Subscribe to XRI Raycast Selection Events purely via code
           _rayInteractor.selectEntered.AddListener(OnObjectSelected);
           _rayInteractor.selectExited.AddListener(OnObjectReleased);
           _rayInteractor.hoverEntered.AddListener(OnObjectHovered);
       }
   
       void OnDisable()
       {
           // Always clean up listeners to avoid memory leaks
           _rayInteractor.selectEntered.RemoveListener(OnObjectSelected);
           _rayInteractor.selectExited.RemoveListener(OnObjectReleased);
           _rayInteractor.hoverEntered.RemoveListener(OnObjectHovered);
       }
   
       private void ConfigureLaserVisuals()
       {
           // Set up the programmatic Line Renderer options
           _lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, 0.01f); // Thin laser line
           _lineRenderer.useWorldSpace = true;
   
           // Tune the XRI Line Visual script properties
           _lineVisual.validColorGradient = CreateColorGradient(Color.cyan);
           _lineVisual.invalidColorGradient = CreateColorGradient(Color.red);
           _lineVisual.smoothMovement = true;
       }
   
       // Code execution loop to check what the ray is currently looking at
       void Update()
       {
           if (_rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
           {
               // This functions similarly to Physics.Raycast, but specifically through XRI logic
               // Rider Tip: Debug.Log here will help you trace real-time tracking accuracy
               string objectName = hit.collider.gameObject.name;
           }
       }
   
       // --- XRI Event Callbacks ---
       private void OnObjectHovered(HoverEnterEventArgs args)
       {
           Debug.Log($"Ray is hovering over: {args.interactableObject.transform.name}");
       }
   
       private void OnObjectSelected(SelectEnterEventArgs args)
       {
           Debug.Log($"Ray successfully triggered selection on: {args.interactableObject.transform.name}");
       }
   
       private void OnObjectReleased(SelectExitEventArgs args)
       {
           Debug.Log($"Ray released selection on: {args.interactableObject.transform.name}");
       }
   
       // Helper method to feed gradients to the line visual via script
       private Gradient CreateColorGradient(Color color)
       {
           Gradient gradient = new Gradient();
           gradient.SetKeys(
               new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
               new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
           );
           return gradient;
       }
   }   
*/


/*
 Native Controller Raycasting via Code (Manual C#): if you want to bypass XRI and build 
 a completely custom mechanic using standard C# raycasting from your VR controller,
 use Physics.Raycast paired with the input tracking data.

 
 using UnityEngine;
   using UnityEngine.XR.Interaction.Toolkit.Interactables;
   
   public class ProgrammaticTarget : MonoBehaviour
   {
       private XRGrabInteractable _grabInteractable;
   
       void Awake()
       {
           // Ensure a collider exists so the interactor's physics raycast can hit it
           if (GetComponent<Collider>() == null)
           {
               gameObject.AddComponent<BoxCollider>();
           }
   
           // Add the XRI Interactable script dynamically
           _grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
   
           // Disable standard gravity grab physics so it acts like a remote floating target
           _grabInteractable.useDynamicAttach = true;
       }
   }
*/


/* 
 SE FOSSE AR/MR RAYCASTING (AR FOUNDATION)...:
 
 If you want to project rays into the physical real world to detect environment geometry (like walls or floors), you use the AR Raycast Manager.
 This bypasses Unity's standard physics engine because real-world environments do not natively have rigidbodies or colliders.

 
 using System.Collections.Generic;
   using UnityEngine;
   using UnityEngine.XR.ARFoundation;
   using UnityEngine.XR.ARSubsystems;
   
   public class ARPlacement : MonoBehaviour
   {
       public ARRaycastManager raycastManager;
       private List<ARRaycastHit> hits = new List<ARRaycastHit>();
   
       void Update()
       {
           // Example: Raycast from the center of the screen
           Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
   
           if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
           {
               // Raycast hit a physical plane successfully
               Pose hitPose = hits[0].pose;
               
               // Move an object to the physical hit location
               transform.position = hitPose.position;
               transform.rotation = hitPose.rotation;
           }
       }
   }
*/
using Scripts;
using UnityEngine;

// BOA PRÁTICA: Colocar este script no mesmo namespace une os dois arquivos no mesmo "grupo oficial".
// namespace Scripts { ... }                                                    // deixarei propositalmente comentado como lição referente ao comentário em Flashlight.cs sobre isso. 

public class InspectionRegion : MonoBehaviour                                   // script que serve para referenciar uma Inspection (um asset ScriptableObject) como componente de objetos
{
  [SerializeField] private Inspection inspectionData;                           // Nome do Inspector específico do objeto (mantido privado e seguro no Inspector)
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  // ====================================================================================
  // GETTER (PROPRIEDADE PÚBLICA)
  // Permite que o Flashlight.cs leia o 'inspectionData', mas impede que ele o modifique por acidente.
  // ====================================================================================
  public Inspection InspectionData => inspectionData; 
}

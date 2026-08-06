using Scripts;
using UnityEngine;

public class InspectionRegion : MonoBehaviour                                   // script que serve para referenciar uma Inspection (um asset ScriptableObject) como componente de objetos
{
  [SerializeField] private Inspection inspectionData;                           // Nome do Inspector específico do objeto
}

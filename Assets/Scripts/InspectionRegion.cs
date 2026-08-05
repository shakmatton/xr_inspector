using Scripts;
using UnityEngine;

public class InspectionRegion : MonoBehaviour                                   // script que serve de suporte ao uso do ScriptableObject (um asset) como componente do objeto
{
  [SerializeField] private Inspection inspectionData;                           // Nome do Inspector específico do objeto
}

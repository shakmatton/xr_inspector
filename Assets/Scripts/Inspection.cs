using UnityEngine;

// script que serve de suporte ao uso do ScriptableObject (um asset) como componente do objeto

namespace Scripts
{
    // Na aba Project do Unity, botão direito >> Create >> Systems >> Inspection
    // Inspection aparece na 1ª posição da lista de items do menu Systems
    
    [CreateAssetMenu(fileName = "Inspection", menuName = "Systems/Inspection", order = 0)]       
    public class Inspection : ScriptableObject                                                  // ScriptableObject: um asset, que lembra um .JSON
    {
        public string inspectionName;                                                           // identificador do objeto que possui um ScriptableObject
        public float inspectionTime;                                                            // tempo de inspeção (hovering?)
    }
    
    // A ideia é indicar que os objetos podem ter os mesmos campos identificadores. 
}
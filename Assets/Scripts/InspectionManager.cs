using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        public static InspectionManager Instance { get; private set; }                          // Uso de Singleton
        [SerializeField] private List<Inspection> inspectionList;                               // Lista de Inspection (ver Inspection.cs): arrastar cada inspection para cada campo ali
        
        public Dictionary<Inspection, bool> dictionaryInspection;                               // Uso de dicionário (Map de par/chave: ScriptableObject, boolean)    

        public Action<Inspection> OnSingleInspected;                                            // evento de inspeção de um item do dicionário
        public Action OnFullInspected;                                                          // evento de inspeção de todos os items do dicionário
        
        private void Awake()                                                                    // Checagem de segurança do Singleton: apenas o primeiro deles ficará "vivo" e ativo.
        {                                                                                       // Feito no Awake para garantir a não-concorrência com o Start() de outros objetos. 
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()                                                                    // No Start(), popula-se a List.
        {
            foreach (Inspection inspection in inspectionList)                                   // "Na inspectionList, iterar sobre cada item (chamado "inspection") do tipo Inspection"
            {
                dictionaryInspection.Add(inspection, false);                                    // Método Add adiciona cada chave no dicionário com o valor false.
            }
        }

        public void CheckInspection(Inspection inspection)                                      // evento a ser chamado por outros scripts (ex.: ColorChanger.cs)
        {
            if (!dictionaryInspection.ContainsKey(inspection)) return;                          // Checagem de segurança: se não houver um inspection, não faz nada

            if (dictionaryInspection[inspection] == false)                                      // se houver um item do dicionário contendo false...
            {
                dictionaryInspection.Add(inspection, true);                                     // ...sinaliza esse item agora como contendo true.

                OnSingleInspected?.Invoke(inspection);                                          // dispara evento de inspeção de objeto único inspecionado (false -> true)
            }
            
            // Lógica:
            // loop de iteração no dicionarioInspection
            // verificar para cada item se ha um false no inspection
            // se houver um false, retornar void
            // após o loop, ativar o evento desejado

            
            // Forma 1 de foreach
            foreach (KeyValuePair<Inspection, bool> dictionaryMap in dictionaryInspection)
            {
                if (dictionaryMap.Value == false) return;
            }

            /* Forma 2 de foreach
            foreach (var val in dictionaryInspection.Values)
            {
                if (val == false)
                     return;
            }
            */
            
            OnFullInspected?.Invoke();                                                          // dispara evento de inspeção de todos os objetos inspecionados (false -> true)
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

// script responsável por gerenciar a parte lógica (no caso, se a inspeção foi feita ou não) e disparar eventos para demais scripts que estejam na escuta deles.

namespace Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        public static InspectionManager Instance { get; private set; }                          // Uso de Singleton
        
        [SerializeField] private List<Inspection> inspectionList;                               // Lista de Inspection (ver Inspection.cs): arrastar cada inspection para cada campo ali
        
        public Dictionary<Inspection, bool> dictionaryInspection = new();                       // Uso de dicionário, já inicializado aqui (Map de par/chave: ScriptableObject, boolean)

        [SerializeField] private float timeLimit = 10f;
        
        
        
        // Abaixo: todos os "public Action" retornam void por padrão! Lembrar disso em ColorChanger.cs (ler comentários ali).
        // Há ainda outras maneiras de contornar isso, customizando métodos usando Func<> ou delegates... mas, por agora, vamos usar Action.
        
        public Action<Inspection> OnSingleInspected;                                            // evento de inspeção de um item do dicionário
        public Action OnFullInspected;                                                          // evento de inspeção de todos os items do dicionário
        public Action OnInspectionFailed;
        
        
        
        // Obs.: ver comentários do script ColorChanger.cs, sobre o uso de Awake X OnEnable X Start
        
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
                                                                                                // Em (inspection, bool), INSPECTION É A CHAVE! LEMBRAR DISSO, DAQUI EM DIANTE!
            }
        }

        public void CheckInspection(Inspection inspection)                                      // evento a ser chamado por outros scripts (ex.: Flashlight.cs)
        {
            if (!dictionaryInspection.ContainsKey(inspection)) return;                          // Checagem de segurança (caso não haja nenhuma chave inspection arrastada para a lista) 

            if (dictionaryInspection[inspection] == false)                                      // se houver um item do dicionário contendo false...
            {
                // dictionaryInspection.Add(inspection, true);                                  // ...sinaliza esse item agora como contendo true (par "inspection, true" adicionado).
                                                                                                // porém, ele adiciona uma inspection uma vez e não consegue continuar adicionando depois
                
                dictionaryInspection[inspection] = true;                                        // logo, esse é o workaround para o comentário acima

                OnSingleInspected?.Invoke(inspection);                                          // dispara evento de inspeção de objeto único inspecionado (false -> true)
                Debug.Log($"Inspection {inspection.name} has been inspected");                  // debug de inspeção
            }
            
            /* Acima, há a lógica que resulta no evento OnSingleInspected.  (inspeção de um único objeto)
               Abaixo, há a lógica que resulta no evento OnFullInspected.   (inspeção de todos os objetos)
                        
            1) loop de iteração no dicionarioInspection
            2) verificar para cada item se ha um false no inspection
            3) se houver um false, retornar void
            4) após o loop, ativar o evento desejado                                                    */

            
            // Forma 1 de foreach
            foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)
            {
                if (dictionaryPair.Value == false) return;
            }

            /* Forma 2 de foreach
            foreach (var val in dictionaryInspection.Values)
            {
                if (val == false)
                     return;
            }                                                                                   */
            
            OnFullInspected?.Invoke();                                                          // dispara evento de inspeção de todos os objetos inspecionados (false -> true)
        }

        public void OnTimeLimit()
        {
            if (timeLimit >= 0) return;
            
            OnInspectionFailed?.Invoke();   
            // Debug.Log("Tempo esgotado!\n You lose!");
        }

        private void Update()
        {
            if (timeLimit >= 0) {
                Debug.Log($"Tempo restante: {timeLimit}");
                timeLimit -= Time.deltaTime;
            }
            OnTimeLimit();
        }
    }
}
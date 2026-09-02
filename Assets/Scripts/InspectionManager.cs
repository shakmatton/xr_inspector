using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// script responsável por gerenciar a parte lógica (no caso, se a inspeção foi feita ou não) e disparar eventos para demais scripts que estejam na escuta deles.

// Rider IDE: Atalho "Ctrl+Shift+F" mostra um termo que já foi usado ao longo do projeto. Útil para relembrar como escrever um método.
// Exemplo: "Como que era mesmo que eu tinha definido o IEnumerator?").

namespace Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        public static InspectionManager Instance { get; private set; }                          // Uso de Singleton
        
        [SerializeField] private List<Inspection> inspectionList;                               // Lista de Inspection (ver Inspection.cs): arrastar cada inspection para cada campo ali
        public Dictionary<Inspection, bool> dictionaryInspection = new();                       // Uso de dicionário, já inicializado aqui (Map de par/chave: ScriptableObject, boolean)

        [SerializeField] private float timeLimit = 10f;
        private bool itsOver = false;                                                           // flag de controle para impedir outros eventos após evento OnInspectionFailed.
        
        private Coroutine inspectionTimeCoroutine;                                                  // dispara ou para o método TimeUp().
        
        
        // Abaixo: todos os "public Action" retornam void por padrão! Lembrar disso em ColorChanger.cs (ler comentários ali).
        // Há ainda outras maneiras de contornar isso, customizando métodos usando Func<> ou delegates... mas, por agora, vamos usar Action.
        
        public Action<Inspection> OnSingleInspected;                                            // evento de inspeção de um item do dicionário
        public Action OnFullInspected;                                                          // evento de inspeção de todos os items do dicionário
        public Action OnInspectionFailed;
        public Action<float> OnCountTime;
        
        // Obs.: ver comentários do script ColorChanger.cs, sobre o uso de Awake X OnEnable X Start
        
        public float TimeLimit {                                                                // Property (getter)
            get {                                                                    
                return timeLimit;                                                               // útil para o InspectionUI.cs
            }
        }
        
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

            inspectionTimeCoroutine = StartCoroutine(TimeUp());                              // inicio uma Coroutine (abaixo), para esperar o tempo-limite para a falha do procedimento.
        }                                                                                       // Aqui, é garantido que a falha (detectada pelo InspectionManager.cs) já aconteceu.

        /*private IEnumerator TimeUp()                                                          // definido um intervalo de tempo aqui, em vez de usar o Update
        {                                                                                       // Update é melhor para algo que ocorre do começo ao fim. 
            yield return new WaitForSeconds(timeLimit);                                         // Em TimeUp(), só preciso que algo ocorra até um limite de tempo (default: 10 segundos) 
            OnInspectionFailed?.Invoke();                                                       // Depois disso, disparo o evento OnInspectionFailed.
        }*/
        

        private IEnumerator TimeUp()                     // vai disparar eventos de Sucesso ou Falha de inspeção (futuramente ouvidos por método de InspectionUI.cs)
        {
            while (timeLimit > 0)                        // se ainda há tempo...
            {
                yield return new WaitForSeconds(1);      // ...espera um segundo
                timeLimit--;                             // ...atualiza contador

                OnCountTime?.Invoke(timeLimit);          // dispara evento de contagem 
            }
            
            itsOver = true;
            OnInspectionFailed?.Invoke();                // dispara evento de falha de inspeção, pois o tempo foi esgotado aqui.
        }
        

        public void CheckInspection(Inspection inspection)                                      // evento a ser chamado por outros scripts (ex.: Flashlight.cs)
        {
            if (itsOver) return;
            if (!dictionaryInspection.ContainsKey(inspection)) return;                          // Checagem de segurança (caso não haja nenhuma chave inspection arrastada para a lista)
            
            if (dictionaryInspection[inspection] == false)                                      // se houver um item do dicionário contendo false...
            {
                // dictionaryInspection.Add(inspection, true);                                  // ...sinaliza esse item agora como contendo true (par "inspection, true" adicionado).
                                                                                                // porém, ele adiciona uma inspection uma vez e não consegue continuar adicionando depois
                
                dictionaryInspection[inspection] = true;                                        // logo, esse é o workaround para o comentário acima

                OnSingleInspected?.Invoke(inspection);                                          // dispara evento de inspeção de objeto único inspecionado (false -> true)
                Debug.Log($"Inspection {inspection.name} has been inspected");                  // debug de inspeção
            }
            
    /* Acima: lógica que resulta no evento OnSingleInspected (inspeção de objeto único). Abaixo: lógica que resulta no evento OnFullInspected (inspeção de todos os objetos).
    1) loop de iteração no dicionarioInspection;    2) verificar p/ cada item se há False no inspection;    3) se houver False, retornar void    4) após o loop, ativar evento desejado  */
            
            // Forma 1 de foreach                                                               // Forma 2 de foreach
            foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)     // foreach (var val in dictionaryInspection.Values) 
            {                                                                                   //      if (val == false) return;
                if (dictionaryPair.Value == false) return;
            }
            
            StopCoroutine(inspectionTimeCoroutine);                                             // pára a rotina com e o contador de tempo... 
            OnFullInspected?.Invoke();                                                          // ... e dispara evento de inspeção de todos os objetos inspecionados (false -> true)
        }   
    }
}
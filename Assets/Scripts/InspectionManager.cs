using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class InspectionManager : MonoBehaviour
    {
        #region ANOTAÇÕES / ESTUDOS

        /*
        Script responsável por gerenciar a parte lógica (no caso, se a inspeção foi feita ou não) e disparar eventos para demais scripts que estejam na escuta deles.

        Rider IDE: Atalho "Ctrl+Shift+F" mostra um termo que já foi usado ao longo do projeto. Útil para relembrar como escrever um método.
        Exemplo: "Como que era mesmo que eu tinha definido o IEnumerator?").
        */

        /*
        Abaixo: todos os "public Action" retornam void por padrão! Lembrar disso em ColorChanger.cs (ler comentários ali).

        Há ainda outras maneiras de contornar isso, customizando métodos usando Func<> ou delegates... mas, por agora, vamos usar Action.
        */

        /*
        Obs.: ver comentários do script ColorChanger.cs, sobre o uso de Awake X OnEnable X Start.
        */

        /*
        Em (inspection, bool), INSPECTION É A CHAVE! LEMBRAR DISSO, DAQUI EM DIANTE!
        */

        /*
        Lógica que resulta no evento OnSingleInspected (inspeção de objeto único).
        Abaixo: lógica que resulta no evento OnFullInspected (inspeção de todos os objetos).

        1) loop de iteração no dictionaryInspection;
        2) verificar p/ cada item se há False no inspection;
        3) se houver False, retornar void;
        4) após o loop, ativar evento desejado.

        OBS.: Isso tudo ainda ocorre dentro de CheckInspection(Inspection inspection)...

        (Forma 1 de foreach:)
        foreach (var val in dictionaryInspection.Values){
            if (val == false) return;
        }
        */

        #endregion

        #region CÓDIGO ANTIGO — TRECHOS DA VERSÃO ANTERIOR

        /*
        PRIMEIRA VERSÃO DO TimeUp():

        private IEnumerator TimeUp()
        {
            yield return new WaitForSeconds(timeLimit);
            OnInspectionFailed?.Invoke();
        }

        Explicação original:

        definido um intervalo de tempo aqui, em vez de usar o Update.
        Update é melhor para algo que ocorre do começo ao fim.
        Em TimeUp(), só preciso que algo ocorra até um limite de tempo (default: 10 segundos).
        Depois disso, disparo o evento OnInspectionFailed.
        */

        /*
        TRECHO ANTIGO:

        private IEnumerator TimeUp()
        {
            yield return new WaitForSeconds(timeLimit);
            OnInspectionFailed?.Invoke();
        }
        */

        /*
        TRECHO ANTIGO RELACIONADO AO Dictionary.Add:

        // dictionaryInspection.Add(inspection, true);                                  // ...sinaliza esse item agora como contendo true (par "inspection, true" adicionado).
                                                                                        // porém, ele adiciona uma inspection uma vez e não consegue continuar adicionando depois

        // dictionaryInspection[inspection] = true;                                    // logo, esse é o workaround para o comentário acima
        */

        /*
        TRECHO ANTIGO DA VERIFICAÇÃO DAS INSPEÇÕES:

        foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)
        {
            if (dictionaryPair.Value == false) return;
        }

        StopCoroutine(inspectionTimeCoroutine);
        OnFullInspected?.Invoke();
        */

        #endregion

        public static InspectionManager Instance { get; private set; }                          // Uso de Singleton

        [SerializeField] private List<Inspection> inspectionList;                               // Lista de Inspection (ver Inspection.cs): arrastar cada inspection para cada campo ali
        public Dictionary<Inspection, bool> dictionaryInspection = new();                       // Uso de dicionário, já inicializado aqui (Map de par/chave: ScriptableObject, boolean)

        [SerializeField] private float timeLimit = 10f;
        private bool timeOver = false;                                                           // flag de controle para impedir outros eventos após evento OnInspectionFailed.
        private Coroutine inspectionTimeCoroutine;                                               // dispara ou para o método TimeUp().

        // Abaixo: todos os "public Action" retornam void por padrão! Lembrar disso em ColorChanger.cs (ler comentários ali).
        // Há ainda outras maneiras de contornar isso, customizando métodos usando Func<> ou delegates... mas, por agora, vamos usar Action.

        public Action<Inspection> OnSingleInspected;                                            // evento de inspeção de um item do dicionário
        public Action OnFullInspected;                                                          // evento de inspeção de todos os items do dicionário
        public Action OnInspectionFailed;
        public Action<float> OnCountTime;

        public Action<float> ONObject;
        public Action OFFObject;
        
        /* eventos disparados quando a simulação é resetada
        public Action<float> OnResetAll;
        public Action OnResetCheckbox;  */

        // Obs.: ver comentários do script ColorChanger.cs, sobre o uso de Awake X OnEnable X Start

        public float hoverObjectTime;                                                                 // tempo de hover em um objeto
        private string hoverObjectName;                                                         // flag de controle sobre qual é o nome do objeto corrente
        private string currentObj = "";                                                         // "ponteiro" que registra último objeto a sofrer hover

        public float TimeLimit                                                                  // Property (getter)
        { get
            {
                return timeLimit;                                                               // útil para o InspectionUI.cs
            }
        }

        private void Awake()                                                                     // Checagem de segurança do Singleton: apenas o primeiro deles ficará "vivo" e ativo.
        {                                                                                        // Feito no Awake para garantir a não-concorrência com o Start() de outros objetos.
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()                                                // No Start(), popula-se a List.
        {
            foreach (Inspection inspection in inspectionList)               // "Na inspectionList, iterar sobre cada item (chamado "inspection") do tipo Inspection"
            {
                dictionaryInspection.Add(inspection, false);                // Método Add adiciona cada chave no dicionário com o valor false.
                                                                            // Em (inspection, bool), INSPECTION É A CHAVE!
            }
            
            inspectionTimeCoroutine = StartCoroutine(TimeUp());      // inicio uma Coroutine (abaixo), para esperar o tempo-limite para a falha do procedimento.
        }

        /* Método FullReset() parcialmente substituído por SceneManager.LoadScene("XR Inspector"), do método ResetCounter (em ResetScene.cs).
        public void FullReset()                                                                // Reseta tela de GameOver e faz restart do contador.
        {
            if (inspectionTimeCoroutine != null)                                               // O botão de Reset deve interromper qualquer coroutine em andamento.
                StopCoroutine(inspectionTimeCoroutine);

            timeOver = false;                                                                   // reset da simulação implica em resetar o "timeOver" para false.

            foreach (Inspection inspection in inspectionList)                                  // todas as inspeções são resetadas com false (voltam para o estado inicial)
            {
                dictionaryInspection[inspection] = false;
            }

            timeLimit = 10f;                                                                   // novo valor de timeLimit definido por padrão
            OnResetAll?.Invoke(timeLimit);                                                     // UI mostra uma contagem regressiva (método InspectionTimer, em InspectionUI.cs)
            OnResetCheckbox?.Invoke();

            inspectionTimeCoroutine = StartCoroutine(TimeUp());                         // Reinicia contador da simulação. "TimeUp()" com parêntesis reinvoca método.
        }*/
        

        private IEnumerator TimeUp()                                                            // vai disparar eventos de Sucesso ou Falha de inspeção (futuramente ouvidos por método de InspectionUI.cs)
        {
            while (timeLimit > 0)                                                               // se ainda há tempo...
            {
                yield return new WaitForSeconds(1);                                             // ...espera um segundo
                timeLimit--;                                                                    // ...atualiza contador

                OnCountTime?.Invoke(timeLimit);                                                 // dispara evento de contagem para um Timer de algum objeto específico
            }

            timeOver = true;                                                                     // tempo-limite da simulação esgotado
            OnInspectionFailed?.Invoke();                                                       // dispara evento de falha geral de inspeção, pois o tempo foi esgotado aqui.
        }
        
        // private void InspectionChanged(Inspection inspection)
        // {
        //     // "InspectionHover" ON PROGRESS
        //     hoverObjectName = inspection.inspectionName;
        //     if (currentObj != hoverObjectName)
        //     {
        //         InspectionCancelled(inspection);
        //     }
        // }
        
        
        public void Inspect (Inspection inspection) {
            
            if (timeOver) return;                                          // se tempo da simulação está esgotado, não fazer mais nada. 
            
            /* Checagem de segurança: verifica se inspection recebida (Inspection arrastada no Inspector) existe como chave no dictionaryInspection. */
            if (!dictionaryInspection.ContainsKey(inspection)) return;     // ("Se essa Inspection não estiver cadastrada no dicionário, não fazer nada.")
            
            if (inspection != null)                                     // inspection existe 
            {
                // "InspectionStarted" ON START
                hoverObjectName = inspection.inspectionName;

                // "InspectionChanged" ON CHANGE
                if (currentObj != hoverObjectName)
                {
                    hoverObjectName = inspection.inspectionName;
                    hoverObjectTime = inspection.inspectionTime;           // hoverObjectTime é o tempo do objeto inspecionado
                    currentObj = hoverObjectName;                          // atualizo "ponteiro" string que "aponta" p/ nome do objeto em hover no momento
                }

                // "InspectionHover" ON PROGRESS
                hoverObjectTime -= Time.deltaTime;                               // Countdown do objeto em hover
                ONObject?.Invoke(hoverObjectTime);                               // atualiza InspectionHoverUI

                Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName} | hoverObjectTime = {hoverObjectTime}");

                // "InspectionCompleted" ON FINISH
                if (hoverObjectTime < 1)
                {
                    CheckInspection(inspection);                           // realiza inspeção em objeto (após final do tempo de hover sobre ele)
                }
            }
            else
            {
                // InspectionCancelled ON CANCEL
                currentObj = "";                  // currentObj "resetado" (raio não acerta nada dentro do maxDistance/myLayer)
                OFFObject?.Invoke();              // atualiza InspectionHoverUI
            }
        }
        
        
        public void CheckInspection(Inspection inspection)                                     
        {
            if (dictionaryInspection[inspection] == false)                                      // se houver um item do dicionário contendo false...
            {
                /* dictionaryInspection.Add(inspection, true);
                Sinaliza o item agora como contendo true (par "inspection, true" adicionado) porém, ele adiciona uma inspection uma vez e não consegue continuar adicionando depois  */

                dictionaryInspection[inspection] = true;                                        // logo, esse é o workaround para o comentário acima

                OnSingleInspected?.Invoke(inspection);                                          // dispara evento de inspeção de objeto único inspecionado (false -> true)
                Debug.Log($"Inspection {inspection.name} has been inspected");                  // debug de inspeção
            }

            /* Acima: lógica que resulta no evento OnSingleInspected (inspeção de objeto único).
               Abaixo: lógica que resulta no evento OnFullInspected (inspeção de todos os objetos).

               1) loop de iteração no dicionarioInspection;
               2) verificar p/ cada item se há False no inspection;
               3) se houver False, retornar void
               4) após o loop, ativar evento desejado

               OBS.: Isso tudo ainda ocorre dentro de CheckInspection(Inspection inspection)...

               (Forma 2 de foreach:)  */

            foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)
            {
                if (dictionaryPair.Value == false) return;                                     // se ainda houver algum objeto não inspecionado, sai do método para evitar chegar ao ponto de "StopCoroutine" abaixo.
            }

            // Se chegar até aqui, então "ALL INSPECTIONS COMPLETED!"
            StopCoroutine(inspectionTimeCoroutine);                                            // pára a rotina com e o contador de tempo...
            OnFullInspected?.Invoke();                                                         // ... e dispara evento de inspeção de todos os objetos inspecionados (false -> true)
        }
    }
}

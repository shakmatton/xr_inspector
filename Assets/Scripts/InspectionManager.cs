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
        private bool itsOver = false;                                                           // flag de controle para impedir outros eventos após evento OnInspectionFailed.
        private Coroutine inspectionTimeCoroutine;                                              // dispara ou para o método TimeUp().

        // Abaixo: todos os "public Action" retornam void por padrão! Lembrar disso em ColorChanger.cs (ler comentários ali).
        // Há ainda outras maneiras de contornar isso, customizando métodos usando Func<> ou delegates... mas, por agora, vamos usar Action.

        public Action<Inspection> OnSingleInspected;                                            // evento de inspeção de um item do dicionário
        public Action OnFullInspected;                                                          // evento de inspeção de todos os items do dicionário
        public Action OnInspectionFailed;
        public Action<float> OnCountTime;

        public Action<float> OnObjectInspectionON;
        public Action OnNoObjectInspectionOFF;
        public Action OnReset;                                                                 // evento disparado quando o jogo é resetado

        // Obs.: ver comentários do script ColorChanger.cs, sobre o uso de Awake X OnEnable X Start

        public float hoverTime;                                                                 // tempo de hover em um objeto
        private string hoverObjectName;                                                         // flag de controle sobre qual é o nome do objeto corrente
        private string currentObj = "";                                                         // "ponteiro" que registra último objeto a sofrer hover

        public float TimeLimit                                                                  // Property (getter)
        {
            get
            {
                return timeLimit;                                                             // útil para o InspectionUI.cs
            }
        }

        private void Awake()                                                                    // Checagem de segurança do Singleton: apenas o primeiro deles ficará "vivo" e ativo.
        {                                                                                        // Feito no Awake para garantir a não-concorrência com o Start() de outros objetos.
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()                                                                    // No Start(), popula-se a List.
        {
            foreach (Inspection inspection in inspectionList)                                  // "Na inspectionList, iterar sobre cada item (chamado "inspection") do tipo Inspection"
            {
                dictionaryInspection.Add(inspection, false);                                   // Método Add adiciona cada chave no dicionário com o valor false. Em (inspection, bool), INSPECTION É A CHAVE!
            }

            inspectionTimeCoroutine = StartCoroutine(TimeUp());                                 // inicio uma Coroutine (abaixo), para esperar o tempo-limite para a falha do procedimento.
        }

        public void ResetTimer()                                                                // Reseta tela de GameOver e faz restart do contador.
        {
            if (inspectionTimeCoroutine != null)
                StopCoroutine(inspectionTimeCoroutine);

            itsOver = false;

            foreach (Inspection inspection in inspectionList)
            {
                dictionaryInspection[inspection] = false;                                      // todas as inspeções voltam para o estado inicial
            }

            timeLimit = 10f;
            OnCountTime?.Invoke(timeLimit);
            OnReset?.Invoke();

            inspectionTimeCoroutine = StartCoroutine(TimeUp());                                // "TimeUp()" entre parêntesis, para reinvocar método do tipo coroutine...
        }

        private IEnumerator TimeUp()                                                            // vai disparar eventos de Sucesso ou Falha de inspeção (futuramente ouvidos por método de InspectionUI.cs)
        {
            while (timeLimit > 0)                                                               // se ainda há tempo...
            {
                yield return new WaitForSeconds(1);                                             // ...espera um segundo
                timeLimit--;                                                                    // ...atualiza contador

                OnCountTime?.Invoke(timeLimit);                                                 // dispara evento de contagem para um Timer de algum objeto específico
            }

            itsOver = true;                                                                     // tempo-limite da simulação esgotado
            OnInspectionFailed?.Invoke();                                                       // dispara evento de falha geral de inspeção, pois o tempo foi esgotado aqui.
        }

        // ----------------- Inspection Hover methods --------------- //

        public void InspectionHoverStart(InspectionRegion inspectionRegion)
        {
            // InspectionHover ON PROGRESS
            hoverObjectName = inspectionRegion.Inspection.inspectionName;

            // InspectionHover ON CHANGE
            if (currentObj != hoverObjectName)
            {
                hoverObjectName = inspectionRegion.Inspection.inspectionName;
                hoverTime = inspectionRegion.Inspection.inspectionTime;
                currentObj = hoverObjectName;                                                   // atualizo "ponteiro" string que "aponta" p/ nome do objeto em hover no momento
            }

            // InspectionHover ON PROGRESS
            hoverTime -= Time.deltaTime;                                                        // Countdown do objeto em hover
            OnObjectInspectionON?.Invoke(hoverTime);                                           // atualiza InspectionHoverUI

            Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName} | hoverTime = {hoverTime}");

            // InspectionHover FINISHED
            if (hoverTime <= 0)
            {
                CheckInspection(inspectionRegion.Inspection);                                  // realiza inspeção em objeto (após final do tempo de hover sobre ele)
            }
        }

        public void InspectionHoverCancelled(InspectionRegion inspectionRegion)
        {
            // currentObj deve ser "resetado", pois o raio não acerta nada dentro do maxDistance/myLayer
            currentObj = "";
            OnNoObjectInspectionOFF?.Invoke();                                                 // atualiza InspectionHoverUI
        }

        // ----------------- Inspection methods --------------- //

        public void CheckInspection(Inspection inspection)                                      // evento a ser chamado por outros scripts (ex.: Flashlight.cs)
        {
            if (itsOver) return;
            if (!dictionaryInspection.ContainsKey(inspection)) return;                           // Checagem de segurança (caso não haja nenhuma chave inspection arrastada para a lista)

            if (dictionaryInspection[inspection] == false)                                     // se houver um item do dicionário contendo false...
            {
                // dictionaryInspection.Add(inspection, true);                                  // ...sinaliza esse item agora como contendo true (par "inspection, true" adicionado).
                // porém, ele adiciona uma inspection uma vez e não consegue continuar adicionando depois

                dictionaryInspection[inspection] = true;                                       // logo, esse é o workaround para o comentário acima

                OnSingleInspected?.Invoke(inspection);                                         // dispara evento de inspeção de objeto único inspecionado (false -> true)
                Debug.Log($"Inspection {inspection.name} has been inspected");                 // debug de inspeção
            }

            /* Acima: lógica que resulta no evento OnSingleInspected (inspeção de objeto único).
               Abaixo: lógica que resulta no evento OnFullInspected (inspeção de todos os objetos).

               1) loop de iteração no dicionarioInspection;
               2) verificar p/ cada item se há False no inspection;
               3) se houver False, retornar void
               4) após o loop, ativar evento desejado

               OBS.: Isso tudo ainda ocorre dentro de CheckInspection(Inspection inspection)...

               (Forma 2 de foreach:)
            */

            foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)
            {
                if (dictionaryPair.Value == false) return;                                     // se ainda houver algum objeto não inspecionado, sai do método para evitar chegar ao ponto de "StopCoroutine" abaixo.
            }

            StopCoroutine(inspectionTimeCoroutine);                                            // pára a rotina com e o contador de tempo...
            OnFullInspected?.Invoke();                                                         // ... e dispara evento de inspeção de todos os objetos inspecionados (false -> true)
        }
    }
}

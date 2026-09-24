using System;
using Scripts;
using TMPro;
using UnityEngine;

// Script que trata de UI, usado no objeto "QuestUI". Concentra e ativa/desativa objetos filhos/netos do painel.
// Lógica toda concentrada no InspectionManager.cs ("cérebro/fonte da verdade"), e script de UI apenas reage a tudo que já foi processado nele (lembrar da hierarquia Scripts/Visuals).

public class InspectionUI : MonoBehaviour
{
    [SerializeField] private GameObject inspectionList;                     // gameObject que mostra a tela original de InspectionList
    [SerializeField] private GameObject inspectionVictory;                  // gameObject "painel com mensagem de Vitória" 
    [SerializeField] private GameObject inspectionFailed;                   // gameObject "painel com mensagem de GameOver"
    
    [SerializeField] private TextMeshProUGUI inspectionTimer;               // atenção: não confundir TextMeshProUGUI com TextMeshPro (verificar no painel Hierarchy do editor)

    // [SerializeField] private Flashlight lantern;
    // [SerializeField] private FlashlightCleanVersion lantern;                // arrastar o gameObject da lanterna aqui (objetivo: criar um timer gráfico usando o timeHover do FlashlightCleanVersion.cs)

    [SerializeField] private GameObject singleObjectTimer;
    [SerializeField] private TextMeshProUGUI singleObjectTimerText;
    
    private float time;                                                     // poderia ter feito como variável local em Start(), ou global (como feito aqui).
    
    private void Start()
    {
        /*
        InspectionManager.Instance.OnFullInspected += ShowVictoryScreen;             // evento de "Vitória"
        InspectionManager.Instance.OnInspectionFailed += InspectionGameOver;         // evento que sinaliza "GameOver" quando tempo limite estourar!
        InspectionManager.Instance.OnCountTime += InspectionTimer;                   // evento de contagem de tempo no painel 
        */
        
        InspectionManagerCleanVersion.Instance.OnFullInspected += ShowVictoryScreen;             // evento de "Vitória"
        InspectionManagerCleanVersion.Instance.OnInspectionFailed += InspectionGameOver;         // evento que sinaliza "GameOver" quando tempo limite estourar!
        InspectionManagerCleanVersion.Instance.OnCountTime += InspectionTimer;                   // evento de contagem de tempo no painel
        
        InspectionManagerCleanVersion.Instance.OnObjectInspectionON += SingleObjectTimer;
        InspectionManagerCleanVersion.Instance.OnNoObjectInspectionOFF += SingleObjectTimerReset;
        
        // Abaixo: todos os gameObjects já possuem um transform por padrão...
        // Por isso, é possível desabilitar a "caixinha" do gameObject desse script fazendo o comando abaixo: 
        
        inspectionList.SetActive(true);                                        // no editor, gameObject é tudo que mostra os seus componentes
        inspectionVictory.SetActive(false);
        inspectionFailed.SetActive(false);
        
        singleObjectTimer.SetActive(false);
        
        // não confundir com componente (uma parte integrante/componente do gameObject)

        time = InspectionManager.Instance.TimeLimit;
        inspectionTimer.text = time.ToString();
    }
    
    private void InspectionGameOver()                               // desabilita o painel com as tarefas (inspectionList), e habilita o painel de Inspections Failed ("GameOver").
    {
        inspectionList.SetActive(false);
        inspectionFailed.SetActive(true);
    }

    private void ShowVictoryScreen()                                // desabilita o painel com as tarefas (inspectionList), e habilita o painel de Inspections Completed ("Vitória").
    {
        inspectionList.SetActive(false);
        inspectionVictory.SetActive(true);
    }
    
    private void InspectionTimer(float timeLimit)                   // faz a UI mostrar uma contagem regressiva
    {
        inspectionTimer.text = timeLimit.ToString();
    }

    
   // VER ARQUIVO INSPECTOR_XR NO DESKTOP (COM CASOS A RESOLVER!)
    
    private void SingleObjectTimer(float hoverTime)                // consigo deixar o pequeno trecho de lógica aqui em algum outro lugar (InspectionManager)? 
    {
        if (hoverTime < 0) return;
        singleObjectTimer.SetActive(true);
        
        int hoverTimeInteger = (int)hoverTime;
        singleObjectTimerText.text = hoverTimeInteger.ToString();
    }

    private void SingleObjectTimerReset()
    {
        singleObjectTimer.SetActive(false);
    }
}
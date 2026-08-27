using System;
using Scripts;
using UnityEngine;

// Script que trata de UI, usado no objeto "QuestUI". Concentra e ativa/desativa objetos filhos/netos do painel.
// Lógica toda concentrada no InspectionManager.cs ("cérebro/fonte da verdade"), e script de UI apenas reage a tudo que já foi processado nele (lembrar da hierarquia Scripts/Visuals).

public class InspectionUI : MonoBehaviour
{
    [SerializeField] private GameObject questContent;                       // esse gameObject se refere ao outro GameObject (que mostra a tela "All Inspections Completed!")

    [SerializeField] private GameObject inspectionVictory;
    [SerializeField] private GameObject inspectionFailed;
    [SerializeField] private GameObject inspectionTimer;
    
    private void Start()
    {
        InspectionManager.Instance.OnFullInspected += ShowVictoryScreen;
        InspectionManager.Instance.OnInspectionFailed += InspectionGameOver;         // evento que sinaliza "GameOver" quando tempo limite estourar!
        
        // abaixo: todos os gameObjects já possuem um transform por padrão... por isso, é possível desabilitar a "caixinha" do gameObject desse script fazendo o comando abaixo: 
        
        questContent.SetActive(true);                                        // no editor, gameObject é tudo que mostra os seus componentes
        inspectionVictory.SetActive(false);
        inspectionFailed.SetActive(false);
        
        // não confundir com componente (uma parte integrante/componente do gameObject)
    }
    
    private void InspectionGameOver()                                        
    {
        questContent.SetActive(false);
        inspectionFailed.SetActive(true);
    }

    private void ShowVictoryScreen()                                        // desabilita o painel com as "quests" (questContent), e habilita o painel de Inspections Completed.
    {
        questContent.SetActive(false);
        inspectionVictory.SetActive(true);
    }
    
    // private void InspectionTimer()                   // ARRUMAR A QUESTÃO DO CONTADOR ATUALIZADO AQUI                                        
    // {
    //     questContent.SetActive(false);
    //     inspectionFailed.SetActive(true);
    // }
}
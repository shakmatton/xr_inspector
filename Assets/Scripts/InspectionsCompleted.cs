using System;
using Scripts;
using UnityEngine;

// Scripts que tratam de UI (como esse) já podem ser considerados como UI.
// Assim, lembrar que a lógica fica toda concentrada no InspectionManager.cs ("cérebro/fonte da verdade"), e os scripts de UI apenas reagem a tudo que já foi processado nele.

public class InspectionsCompleted : MonoBehaviour
{
    [SerializeField] private GameObject questContent;                       // esse gameObject se refere ao outro GameObject (que mostra a tela "All Inspections Completed!")
    
    private void Start()
    {
        InspectionManager.Instance.OnFullInspected += ShowVictoryScreen;
        InspectionManager.Instance.OnInspectionFailed += InspectionGameOver;         // evento que sinaliza "GameOver" quando tempo limite estourar!
        
        // abaixo: todos os gameObjects já possuem um transform por padrão... por isso, é possível desabilitar a "caixinha" do gameObject desse script fazendo o comando abaixo: 
        
        gameObject.SetActive(false);                                        // no editor, gameObject é tudo que mostra os seus componentes
                                                                            // não confundir com componente (uma parte integrante/componente do gameObject)
    }
    
    
    
    // continuar depois o método abaixo, criando o painel no editor e fazendo a lógica dele...
    
    
    private void InspectionGameOver()                                        
    {
        questContent.SetActive(false);
        gameObject.SetActive(true);
    }

    private void ShowVictoryScreen()                                        // desabilita o painel com as "quests" (questContent), e habilita o painel de Inspections Completed.
    {
        questContent.SetActive(false);
        gameObject.SetActive(true);
    }
}

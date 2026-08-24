using System;
using Scripts;
using UnityEngine;

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
    
    private void InspectionGameOver()                                       // callback que desinscreve o método de full inspection (evita que ele ocorra após o "Game Over") 
    {
        InspectionManager.Instance.OnFullInspected -= ShowVictoryScreen;  
        Debug.Log("Tempo esgotado para Full Inspection!\n You lose!");
    }

    private void ShowVictoryScreen()                                        // desabilita o painel com as "quests" (questContent), e habilita o painel de Inspections Completed.
    {
        questContent.SetActive(false);
        gameObject.SetActive(true);
    }
}

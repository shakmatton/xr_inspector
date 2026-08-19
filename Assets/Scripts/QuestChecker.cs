// Não usei a classe abaixo...

/*
using System;
using Scripts;
using UnityEngine;

public class QuestChecker : MonoBehaviour
{
    /* Passos:
     
    - Fazer esse script reagir ao evento disparado pelo Singleton InspectionManager.cs
    - Quando um objeto for colidido por um Ray, o InspectionManager invoca o método callback definido aqui
    - Fazer a inscrição do evento desse script no evento do InspectionManager, lembrando da forma de fazer (lembrar de comentários em ColorChanger.cs)
    - Depois fazer com que o painel mude seu quest (algum boolean marcando um "Check" em alguma quest do painel no Unity)
    - Lidar com eventos simples únicos ("quest completed"), e depois, com o evento final ("all quests completed!")                 
    

    private void Start()
    {
        InspectionManager.Instance.OnSingleInspected += SingleQuestCompleted;
        InspectionManager.Instance.OnFullInspected += AllQuestsCompleted;
    }

    private void SingleQuestCompleted(Inspection inspection)
    {
        // adicionar ícone de checked, ou um X, ou algo do tipo, para cada quest completada
    }

    private void AllQuestsCompleted()
    {
        
    }
}
*/
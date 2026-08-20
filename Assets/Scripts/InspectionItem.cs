using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class InspectionItem : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI textMeshProUGUI;                     // TextMeshProUGUI deverá ser arrastado aqui aparecer neste inspectionItem
        [SerializeField] public Sprite checkboxNo;                                   // imagem (sprite do checkbox não preenchido) que deverá aparecer neste inspectionItem
        [SerializeField] public Sprite checkboxYes;                                  // imagem (sprite do checkbox já preenchido) que deverá eventualmente substituir a anterior (após eventos)

        private Image _checkboxImage;                                                // imagem possui como "sub-atributo" um Sprite (acessar como "_checkboxImage.sprite")
        
        // Obs.: na hierarquia do editor Unity, deve-se fazer Image e Text serem filhos (separados) do gameObject InspectionItem (p/ ter maior controle do Transform de cada um). 
        
        [SerializeField] public Inspection inspection;                               // inspection deve ser arrastado aqui
                                                                                     // para cada prefab, algum inspection (de esfera, cubo, cilindro) deve ser arrastado nesse campo.
        
        /* Passos:
         
            - criar o componente no editor, e arrastar o Inspection para o campo inspection.  (ok)
            - fazer o nome do Inspection aparecer no InspectionItem (um Empty na cena). Usar inspectionDescription, de Inspection.cs.
            - verificar como pegar uma referência para TextMeshPro e para Imagens.
            - fazer o mesmo para a imagem de "tick" da quest.
            - configurar reação aos eventos do InspectionManager (tal como ocorrido com o ColorChanger).         */
        
        
            // continuar pensando como fazer as interações de eventos funcionarem para o gameObject... e no final, quando tudo tiver ok, criar 3 prefabs desse gameobject na cena......
            
            
        private void Start()                                                        // Ideia: já de início (no Start()), aparecer no painel o nome das tarefas (quests).
        {
            if (inspection != null)                                              // Caso inspection já tenha sido arrastado...
            {
                textMeshProUGUI.text = inspection.inspectionDescription;            // ...fazer texto do inspectionDescription aparecer aqui.
                                                                                    // Dica: p/ ver o nome das classes, clicar nos 3 pontinhos de cada componente (Image, TextMeshPro etc).
                                                                                    // Em TextMeshPro, ver que a classe herda de outra (o que permite usar diretamente a string "text")
                                                                                    
                _checkboxImage = GetComponentInChildren<Image>();                   // O filho do gameObject é do tipo Image. Salvamos ele em _checkboxImage.
                _checkboxImage.sprite = checkboxNo;                                 // Sprite do _checkboxImage aponta para o sprite original ("checkbox_no")
            }
            
            InspectionManager.Instance.OnSingleInspected += SingleQuestCompleted;   // evento do InspectionManager aponta para o callback SingleQuestCompleted.
            // InspectionManager.Instance.OnFullInspected += AllQuestsCompleted;
        }
        

        private void SingleQuestCompleted(Inspection inspection)
        {
            // if (inspection.inspectionName == inspectionRegion.inspectionData)    // ?????
            
            _checkboxImage.sprite = checkboxYes;                                    // sprite do checkbox atualizado para checkbox_yes 
        }

        private void Update()
        {
            
        }
    }
}
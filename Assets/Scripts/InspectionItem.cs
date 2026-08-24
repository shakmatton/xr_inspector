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
            
            InspectionManager.Instance.OnSingleInspected += SingleInspectionCompleted;   // evento do InspectionManager aponta para o callback SingleInspectionCompleted.
            // InspectionManager.Instance.OnFullInspected += AllInspectionsCompleted;    // isso foi feito em um novo script, dedicado apenas a essa tarefa (ver InspectionsCompleted.cs)
            
            InspectionManager.Instance.OnInspectionFailed += InspectionGameOver;         // evento que sinaliza "GameOver" quando tempo limite estourar!

        }
        private void InspectionGameOver()                                                // callback que desinscreve o método de single inspection (evita que ele ocorra após o "Game Over") 
        {
            InspectionManager.Instance.OnSingleInspected -= SingleInspectionCompleted;   
            Debug.Log("Tempo esgotado para Single Inspection!\n You lose!");
            
            
            // pensar em como evitar que isso fique repetindo eternamente no console (mesmo problema de InspectionGameOver(), em InspectionCompleted.cs).
            
            
        }

        private void SingleInspectionCompleted(Inspection inspection)
        {
            if (this.inspection == inspection)
            {
                _checkboxImage.sprite = checkboxYes;                                    // sprite do checkbox atualizado para checkbox_yes
            }    
        }
    }
}
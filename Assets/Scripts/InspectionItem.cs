using System;
using UnityEngine;

namespace Scripts
{
    public class InspectionItem : MonoBehaviour
    {
        [SerializeField] public Inspection inspection;
        
        /* Passos:
         
            - criar o componente no editor, e arrastar o Inspection para o campo inspection.  (ok)
            - fazer o nome do Inspection aparecer no InspectionItem (um Empty na cena). Usar inspectionDescription, de Inspection.cs.
            - verificar como pegar uma referência para TextMeshPro e para Imagens.
            - fazer o mesmo para a imagem de "tick" da quest.
            - configurar reação aos eventos do InspectionManager (tal como ocorrido com o ColorChanger).         */

        private void Start()
        {
            if (inspection != null)
            {
                
            }
        }
    }
}
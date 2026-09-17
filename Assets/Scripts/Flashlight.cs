using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/* Fazer:
    - criar o Physics Ray (lembrar que Interaction Toolkit é sobre interações de botões/triggers, e que futuramente em algum momento daria apoio ao Ray)
    - lembrar do uso das layers (relembrar uso "layerA && layerB") para detectar objetos que pertençam a um grupo (verificar em "Layer: Default", no Inspector)
    - raio deve ter a informação de hit/miss, e deve permitir uso de máscara (para detectar apenas os objetos desejados de uma certa layer) 
    
    - Atualização: implementar aqui (provisoriamente) o uso do inspectionTime (asset Inspection.cs), com delay para seleção de cada objeto (delay deve ser resetado a cada objeto mirado). 
    
 */

namespace Scripts
{
    /* POR QUE ESTE SCRIPT ENXERGA O INSPECTIONREGION.CS?
       1. COMPILAÇÃO GLOBAL: No Unity, por padrão, todos os scripts públicos na pasta Assets são compilados no mesmo arquivo final (Assembly-CSharp.dll), tornando-os visíveis entre si.
       2. REGRA DE NAMESPACE: Como 'InspectionRegion' está no escopo global (sem namespace), qualquer script dentro de um namespace (como o 'Scripts' aqui) consegue enxergá-lo livremente.  */
    
    
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private LayerMask myLayer;                             // escolher aqui a referência da layer "Highlighted Objects"
        private Ray ray;                                                        // raio para interação XR
        private float maxDistance = 5f;                                         // alcance máximo do raio
        private bool flashlightSelected = false;                                // flashlightSelected: boolean que reflete status do controle (grabbed/ not grabbed)
        
        private float hoverTime= 0;                                                // tempo de hover em um objeto
        private string hoverObjectName;                                         // string de objeto apontado pelo raio
        private string currentObj = "";                                         // string atuando como "ponteiro", registrando último objeto a sofrer hover
        
        [SerializeField] private XRBaseInteractable interactable;               // adiciona o Flashlight como objeto interactable (XR Grab Interactable)

        private void Start()                                                    // lembrar de configurar no Start() os eventos Unity abaixo
        {
            interactable.selectEntered.AddListener(OnSelect);                   
            interactable.selectExited.AddListener(OnDeselect);
        }

        private void OnSelect(SelectEnterEventArgs args)                        
        {
            // Chamado quando pega objeto Flashlight (flashlightSelected)
            flashlightSelected = true;
        }

        private void OnDeselect(SelectExitEventArgs args)
        {
            // Chamado quando larga objeto Flashlight (not flashlightSelected)
            flashlightSelected = false;
        }

        private void Update()                                                       // O Ray é recriado a cada frame para acompanhar a posição e a orientação atuais da lanterna.
        {
            if (!flashlightSelected)                                                // Se não estiver selecionado, não faz nada... senão, executa abaixo:
                return;

            ray = new Ray(transform.position, transform.forward);    // Raio criado a partir da posição da lanterna, em sua origem e sentido por ela apontado (eixo Z azul).

            // TRECHO ABAIXO AINDA É FUNCIONAL (COMENTADO PARA DAR LUGAR AO NOVO CÓDIGO ABAIXO DELE)
            // PARA USAR O TRECHO DE CÓDIGO LOGO ABAIXO, DESCOMENTE-O E COMENTE O TRECHO DO CÓDIGO APÓS ELE 
            
            // if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))  // Lança um Raycast usando o Ray criado. O if é true se raio atinge algum collider dentro de maxDistance.
            // {
            //     /* Processo:
            //         - Verificar se o gameobject que acabamos de colidir tem Inspection Region
            //         - Se tiver, salvar esse inspection Region em uma variavel local
            //         - Salvar o Inspection desse inspectionRegion (Fazer getter ou deixar publico) em um variavel local
            //         - Chamar CheckInspection do inspectionManager passando esse inspection                 */
            //     
            //     // O C# encontra a classe InspectionRegion aqui porque ela é pública e global no projeto:
            //     InspectionRegion inspectionRegion = hit.collider.gameObject.GetComponent<InspectionRegion>();   // (inspectionRegion aponta pro gameObject que teve colisão com o ray)
            //     
            //     if (inspectionRegion != null)                                    // se o objeto colidido pelo raio possuir um InspectionRegion
            //     {
            //         /* Cria-se abaixo uma variável local chamada "inspection" do tipo Inspection e atribui a ela a mesma referência
            //            para o objeto Inspection armazenado em "inspectionData" no InspectionRegion. O acesso a ".Inspection" executa o getter da propriedade. */
            //         
            //         Inspection inspection = inspectionRegion.Inspection;            // cria variável local inspection...
            //                                                                         // ...e atribui a ela o "inspectionData" do InspectionRegion.cs (retornado pelo getter dele).
            //                                                                         
            //         InspectionManager.Instance.CheckInspection(inspection);         // chama o método CheckInspection do InspectionManager (passando como parâmetro a inspection)
            //     }
            // }
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer))  
            {
                InspectionRegion inspectionRegion = hit.collider.gameObject.GetComponent<InspectionRegion>();  
                
                /* Casos:
                   1 - fazer hover num objeto qualquer, com o tempo definido para esse objeto
                   2 - pensar no reset desse hover, caso o hover deixe o objeto
                   3 - pensar no caso de 2 objetos grudados: como fica a questao do hover e do reset?

                 * PROBLEMA: AO SAIR E RETORNAR AO MESMO OBJETO, O TIMER NÃO RESETA!
                 * SOLUÇÃO: ver último else do código (currentObj = "").                 
                 
                 * O currentObj = "" só é executado quando o Physics.Raycast acerta um collider que não tem InspectionRegion.
                 * Mas na prática, quando você "sai" de um objeto olhando para outro lugar, o mais comum é o raio não acertar nada dentro do maxDistance/myLayer
                 * — ou seja, Physics.Raycast retorna false, e o bloco inteiro (inclusive o else que reseta currentObj) é pulado.

                 * Resultado: currentObj continua guardando o nome do último objeto mirado.
                 * Quando você volta a mirar nesse mesmo objeto, currentObj != hoverObjectName é false (porque nunca foi resetado), então o hoverTime nunca é reatribuído
                 * — ele continua de onde parou (geralmente negativo/zerado), em vez de reiniciar a contagem.                 */
                
                if (inspectionRegion != null)                                                                    // raio aponta para ALGUM objeto (lembre-se da distância do raio)
                {
                    hoverObjectName = inspectionRegion.Inspection.inspectionName;                                   // obtenho nome do objeto atual
                    
                    if (currentObj != hoverObjectName)                                                              // se objeto "apontado" difere do objeto corrente 
                    {
                        hoverObjectName = inspectionRegion.Inspection.inspectionName;                               // obtenho nome do objeto 
                        hoverTime = inspectionRegion.Inspection.inspectionTime;                                     // obtenho tempo do objeto

                        currentObj = hoverObjectName;                                               // atualizo "ponteiro" string que "aponta" p/ nome do objeto em hover no momento
                    }

                    hoverTime -= Time.deltaTime;                                                                    // Countdown do objeto em hover

                    Debug.Log($"CurrentObjName = {currentObj} | HoverObjectName = {hoverObjectName} | hoverTime = {hoverTime}");

                    if (hoverTime <= 0)                                                                             // tempo de inspeção do objeto em hover concluído
                    {
                        InspectionManager.Instance.CheckInspection(inspectionRegion.Inspection);                    // chama o método de inspeção para aquele objeto em hover
                    }
                }
                // else {                                                                                              // inspectionRegion nulo (raio aponta para fora do objeto)
                //     currentObj = "";                                                                                // "ponteiro" (string) resetado para nulo
                // }
            }
            else {                                                                                  // PULO DO GATO: saber que o raio não acerta nada dentro do maxDistance/myLayer
                currentObj = "";                                                                    // é preciso esse else extra para resetar o currentObj!
            }
            
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);                // ray (em debug mode) acompanha o transform da lanterna (ver red line na aba Scene)
                
            
                // if ((myLayer.value & hit.collider.gameObject.layer) != 0)          // caso não houvesse "myLayer" em (Physics.Raycast(ray, out RaycastHit hit, maxDistance, myLayer)
                    // Debug.Log("Hit: " + hit.collider.name);
                    
                
                
                /* =========== Algumas coisas sobre Layers abaixo (só use se for necessário mudar layers de lugar...) ===========
                                    
                 
                   Se layer "Highlighted Objects" for Layer 8 (por exemplo), ela retorna 8. Porém, myLayer.value não valeria 8, mas sim, 1 << 8 (ou seja, 256 (2 elevado à 8)).
                   (1 << 8 significa "deslocar 1 bit à esquerda 8 vezes"). Isso produz uma máscara contendo apenas o bit correspondente à Layer 8. 
                   Então, myLayer.value = 256 e LayerMask.nameToLayer(...) = 8.

                if (myLayer.value == LayerMask.NameToLayer("Highlighted Objects"))
                    Debug.Log(hit.collider.name);                                               // nome do objeto mostrado no console
                                                
                                                
                /* =========== Possibilidades (1) ===========
                
                
                /* a) No HIT, buscar a layer do objeto que teve se collider atingido pelo HIT, e verificar se essa layer é a mesma de "Highlighted Objects":
                   if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Highlighted Objects"))
                      Debug.Log(hit.collider.name);                                             // nome do objeto mostrado no console  */

                
                /* =========== Possibilidades (2) ===========

                /* b) No HIT, a layer do objeto colidido é deslocada um bit pra esquerda (nº da Layer) vezes.
                      Aplica-se uma operação AND bit a bit (&), ou seja, "&" simples, entre os valores binários dos layers da lanterna e dos objetos na layer "Highlighted Objects".

                   Exemplo:

                   10001000
                   &
                   00001000
                   ---------
                   00001000   (int "8")

                   // Se o resultado do AND for diferente de zero, significa que a Layer do objeto faz parte da LayerMask.
                   // Nesse caso, mostra-se o nome do objeto colidido por meio de seu collider.                         */     
            
        }
    }
}
using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private GameObject resetBtn;
    
    private void Start()
    {
        resetBtn.SetActive(false);
        
            /*   ==================== EXPLICAÇÃO SOBRE O USO DO XRInteractionSimulator (abaixo) ====================

                  - Motivação: o código abaixo serve para inserir manualmente o XRInteractionSimulator na cena.
                  Isso permite o bypass do "DontDestroyOnLoad" (no painel Hierarchy).

                  Se não fazemos isso manualmente, o Unity fará isso, e por padrão, ela vai inserir o
                  XRInteractionSimulator dentro do "DontDestroyOnLoad" automaticamente.

                  - Explicação: no ResetCounter(), mais abaixo, queremos que o SceneManager.LoadScene("XR Inspector")
                  resete tudo, INCLUINDO a POSIÇÃO/ROTAÇÃO do player. Mas o Unity não reseta o player,
                  pois o Character Controller (em algum lugar dentro de XR origin) "sobrevive" ao LoadScene
                  (XRInteractionSimulator existe dentro do DontDestroyOnLoad).

                  Portanto, ao fazermos a inserção manual do XRInteractionSimulator na cena, fazemos ela
                  ficar fora do DontDestroyOnLoad (note isso durante o Play). Isso permite o reset
                  da posição/rotação do player, usando o código abaixo.

                  Isso é um detalhe da Unity Engine. Pode ser interessante para tratar alguns casos,
                  durante a simulação no editor, mas, na prática, nos óculos VR, isso será irrelevante.   */
         
                  XRInteractionSimulator[] simulators = FindObjectsByType<XRInteractionSimulator>();
                
                  foreach (var simulator in simulators)
                  {
                      SceneManager.MoveGameObjectToScene(simulator.gameObject, SceneManager.GetActiveScene()); 
                  }
                
            //   ==================== FIM DA EXPLICAÇÃO SOBRE O USO DO XRInteractionSimulator (acima) ====================
        
        InspectionManager.Instance.OnInspectionFailed += ShowResetButton;
    }

    private void ShowResetButton()
    {
        resetBtn.SetActive(true);
    }
    
    public void ResetCounter()                      // método ativado pelo clique no botão (ver "On Click" no Inspector dele).
    {
        SceneManager.LoadScene("XR Inspector");
    }
}
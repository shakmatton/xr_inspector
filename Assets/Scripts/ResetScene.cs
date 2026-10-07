using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private GameObject resetBtn;
    
    // Dúvida: será que a lógica de reposicionamento comentado abaixo estaria sendo executada,
    // mas o sistema de tracking simulado estaria sobrescrevendo a pose depois? 
    
    // [SerializeField] private Transform xrOrigin;
    // [SerializeField] private Transform playerPositionAndRotation;

    private void Start()
    {
        // xrOrigin.position = playerPositionAndRotation.position;
        // xrOrigin.rotation = playerPositionAndRotation.rotation;

        // Debug.Log("XR Origin após posicionar: " + xrOrigin.position);

        resetBtn.SetActive(false);
        InspectionManager.Instance.OnInspectionFailed += ShowResetButton;
    }

    private void ShowResetButton()
    {
        resetBtn.SetActive(true);
    }
    
    public void ResetCounter()                      // método ativado pelo clique no botão (ver "On Click" no Inspector dele).
    {
        // Debug.Log("ANTES DO LOADSCENE: " + xrOrigin.position);
        SceneManager.LoadScene("XR Inspector");
    }
}
using Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private GameObject resetBtn;

    private void Start()
    {
        resetBtn.SetActive(false);
        
        InspectionManager.Instance.OnInspectionFailed += ShowResetButton;       // controla aparecimento do botão de reset após tela de GameOver
    }

    private void ShowResetButton()
    {
        resetBtn.SetActive(true);
    }

    public void ResetCounter()
    {
        SceneManager.LoadScene("XR Inspector");
    }
}
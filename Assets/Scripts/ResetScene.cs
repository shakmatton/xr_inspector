using Scripts;
using UnityEngine;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private GameObject resetBtn;

    private void Start()
    {
        resetBtn.SetActive(false);

        InspectionManagerCleanVersion.Instance.OnInspectionFailed += ShowResetButton;       // controla aparecimento do botão de reset após tela de GameOver
    }

    private void ShowResetButton()
    {
        resetBtn.SetActive(true);
    }

    public void ResetCounter()
    {
        resetBtn.SetActive(false);

        InspectionManagerCleanVersion.Instance.ResetTimer();                               // ativa método de reset do timer diretamente no "Manager". 
    }
}
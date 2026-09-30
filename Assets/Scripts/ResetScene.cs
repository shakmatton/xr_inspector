using Scripts;
using UnityEngine;

public class ResetScene : MonoBehaviour
{
    [SerializeField] private GameObject resetBtn;
    private InspectionUI _inspectionUI;

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
        resetBtn.SetActive(false);

        _inspectionUI = resetBtn.AddComponent<InspectionUI>();

        InspectionManager.Instance.ResetTimer();                               // ativa método de reset do timer diretamente no "Manager". 
    }
}
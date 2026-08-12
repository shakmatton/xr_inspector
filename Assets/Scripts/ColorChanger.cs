using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorChanger : MonoBehaviour
{
    // Objetivo: ouvir o evento de "mudança de estado de boolean do inspector", disparado pelo InspectionManager
    // Processar mudança de cor do objeto passado pelo InspectionManager

    [SerializeField] private List <Color> colors;
    [SerializeField] private float frequency;
    
    [SerializeField] private Renderer _renderer;
    [SerializeField] private string colorProperty = "_BaseColor";               // controla a propriedade relacionada à cor do objeto
    
    private Material _material;
    private int _currentColorIndex = 0;
    
    [SerializeField] public InspectionManager inspectionManager;
    [SerializeField] private Inspection inspection;

    private void Start()
    {
        _material = _renderer.material;
        
        /* Sequência para descobrir o nome do shader e se ele possui a cor/propriedade "Color" ou "_BaseColor (que é o padrão para shaders do tipo Shader Graphs/Interactable)" 
         
        Debug.Log(_material.color);                                   
        Debug.Log(_material.shader.name);
        
        Debug.Log(_material.HasColor("_Color"));
        Debug.Log(_material.HasColor("_BaseColor"));
        Debug.Log(_material.HasProperty("_Color"));
        Debug.Log(_material.HasProperty("_BaseColor"));         */
    }

    private void OnInspectionEnter(Inspection inspection)
    {
         // InspectionManager.Instance.OnSingleInspected += OnColorChange;         // callback (método executado quando evento é chamado (lembrando: += faz append em uma lista de métodos)
         inspectionManager.CheckInspection(inspection);
         StartCoroutine(ChangeColorRoutine());
         //InspectionManager.OnSingleInspected += OnColorChange(inspection);
    }

    private void OnColorChange(Inspection inspection)
    {
        
    }
    
    private void OnInspectionExit(Inspection inspection)
    {
        StopCoroutine(ChangeColorRoutine());
    }

    private IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            _material.SetColor(colorProperty, colors[_currentColorIndex]);
            _currentColorIndex = (_currentColorIndex + 1) % colors.Count;
            
            yield return new WaitForSeconds(frequency);
        }
    }
}

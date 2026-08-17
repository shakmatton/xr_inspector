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
    [SerializeField] private float delay = 1;                                   // intervalo de tempo entre as mudanças de cor
    
    [SerializeField] private Renderer _renderer;
    [SerializeField] private string colorProperty = "_BaseColor";               // controla a propriedade relacionada à cor do objeto
    
    private Material _material;
    private int _currentColorIndex = 0;

    [SerializeField] private InspectionRegion inspectionRegion;
    
    // private Inspection inspection;

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

        /* ====================== ATENÇÃO ======================
         
        O código (logo após esse comentário) foi inserida no Start, em vez de OnEnable, pois descobriu-se que o LifeCycle Unity nem sempre é respeitado!
        Exemplo: em teoria, Awake de script 1 executaria antes de OnEnable do script 2... mas no Debug.Log, pode ocorrer que o OnEnable do script 2 ocorra antes do Awake do script 1!!!
        Logo, o mais seguro é usar o Start, em vez de OnEnable, nos casos em que houver um Awake presente em algum script do projeto!           */
        
        InspectionManager.Instance.OnSingleInspected += OnColorChange;              // ATENÇÃO: não fazer "OnColorChange()", pois o objetivo é apenas apontar para o método
                                                                                    // (inscrição na lista de métodos do evento), e não executar ele de imediato (o callback).
                                                                                    // Lembrar da diferença entre Inscrição X Callback nos eventos!
                                                                                    
                                                                                    // Lembrar que o 'OnSingleInspected" espera parâmetro "Inspector" e que retorna void
                                                                                    // (ver comentário em InspectionManager.cs, em public Action<Inspection> OnSingleInspected;).
                                                                   
    }

    private void OnColorChange(Inspection inspection)                               // método é um callback (que só executa quando é chamado por um outro método externo)
    {
        if (inspectionRegion.Inspection == inspection)
        {
            StartCoroutine(ChangeColorRoutine());                                   // Atenção ao "ChangeColorRoutine()", em vez de "ChangeColorRoutine".
        }                                                                           // Isso porque queremos executar a rotina do IEnumerator abaixo.
    }
    
    private IEnumerator ChangeColorRoutine()                                        // aqui as cores mudam conforme a organização delas no objeto (ver editor).
    {
        int cycle = 3;                                                              // haverá 3 sequências de mudança de cor 
        while (cycle >= 0)
        {
            _material.SetColor(colorProperty, colors[_currentColorIndex]);
            _currentColorIndex = (_currentColorIndex + 1) % colors.Count;

            cycle--;
            
            yield return new WaitForSeconds(delay);                                 // delay padrão: 1 segundo
        }
    }
}

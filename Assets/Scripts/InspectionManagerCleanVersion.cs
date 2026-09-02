
/* Versão do script InspectionManager.cs livre de comentários. (se houver comentários e/ou linhas de código novos, transferir para InspectionManager.cs depois).
   Tentar manter atualizada essa versão, para acompanhar as mudanças em InspectionManager.cs.    */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class InspectionManagerCleanVersion : MonoBehaviour
    {
        public static InspectionManagerCleanVersion Instance { get; private set; }                        
        
        [SerializeField] private List<Inspection> inspectionList;                               
        public Dictionary<Inspection, bool> dictionaryInspection = new();                       

        [SerializeField] private float timeLimit = 10f;
        private bool itsOver = false;                                                           
        
        private Coroutine inspectionTimeCoroutine;                                              
        
        public Action<Inspection> OnSingleInspected;                                            
        public Action OnFullInspected;                                                          
        public Action OnInspectionFailed;
        public Action<float> OnCountTime;
        
        public float TimeLimit {                                                                
            get {                                                                    
                return timeLimit;                                                               
            }
        }
        
        private void Awake()                                                                    
        {                                                                                        
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()                                                                    
        {
            foreach (Inspection inspection in inspectionList)                                   
            {
                dictionaryInspection.Add(inspection, false);                                    
                                                                                                
            }
            inspectionTimeCoroutine = StartCoroutine(TimeUp());                          
        }

        private IEnumerator TimeUp()
        {
            while (timeLimit > 0)   
            {
                yield return new WaitForSeconds(1);      
                timeLimit--;                             

                OnCountTime?.Invoke(timeLimit);           
            }
            OnInspectionFailed?.Invoke();                
        }
        
        public void CheckInspection(Inspection inspection)                                      
        {
            if (itsOver) return;
            if (!dictionaryInspection.ContainsKey(inspection)) return;                        
            
            if (dictionaryInspection[inspection] == false)                                    
            {
                dictionaryInspection[inspection] = true;

                OnSingleInspected?.Invoke(inspection);  
                Debug.Log($"Inspection {inspection.name} has been inspected");
            }
            
            foreach (KeyValuePair<Inspection, bool> dictionaryPair in dictionaryInspection)      
            {                                                                                   
                if (dictionaryPair.Value == false) return;
            }
            
            StopCoroutine(inspectionTimeCoroutine);                                              
            OnFullInspected?.Invoke();                                                          
        }   
    }
}
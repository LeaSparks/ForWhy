using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;
public class StageClearDotweenPanBar : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private Vector2 _orangeBarEndLocation;
    [SerializeField] private Vector2 _blackBarEndLocation;

    [Header("Clear Time")]
    [SerializeField] private GameObject _orangeBarOne;
    [SerializeField] private GameObject _blackBarOne;

    [Header("Movement Distance")]
    [SerializeField] private GameObject _orangeBarTwo;
    [SerializeField] private GameObject _blackBarTwo;
    
    [Header("Mission Reward")]
    [SerializeField] private GameObject _orangeBarThree;
    [SerializeField] private GameObject _blackBarThree;
    
    [Header("Clear Reward")]
    [SerializeField] private GameObject _orangeBarFour;
    [SerializeField] private GameObject _blackBarFour;

    [Header("Gold")]
    [SerializeField] private GameObject _orangeBarFive;
    [SerializeField] private GameObject _blackBarFive;
    
    [Header("Exp")]
    [SerializeField] private GameObject _orangeBarSix;
    [SerializeField] private GameObject _blackBarSix;
    
    public void LevelEndResult()
    {
        Sequence levelEndSequence = DOTween.Sequence();
        
        levelEndSequence.Append(transform.DOScaleX(1, _duration));
    }


}

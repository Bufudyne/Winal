using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    
    [SerializeField] private DOTweenAnimation getReadyAnimation;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _comboTextPanel;
    [SerializeField] private TextMeshProUGUI _comboTextValue;
    
    [SerializeField] private GameObject _buttonRetry;

    public int pointPerTile = 5;
    private int _scoreMultiplier = 0;
    private int _currentScore;
    private RectTransform _comboRectTransform;
    private static Material _textHighlightMaterial;
    private readonly StringBuilder comboStrBuilder = new StringBuilder();
    


    private void Awake()
    {
        _comboRectTransform = _comboTextPanel.GetComponent<RectTransform>();
        _textHighlightMaterial = new Material(_comboTextValue.fontSharedMaterial);
        _textHighlightMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.red*8f);
    }

    private void Start()
    {
        EventManager.TriggerEvent(On.LoadStage, null);
    }

    private void OnEnable()
    {
        EventManager.StartListening(On.LoadStage, OnLoadStage);
        EventManager.StartListening(On.TimerRanOut, OnTimerRanOut);
        EventManager.StartListening(On.PointObtained, OnPointObtained);
        EventManager.StartListening(On.PointTimedOut, OnPointTimedOut);
        
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.LoadStage, OnLoadStage);
        EventManager.StartListening(On.TimerRanOut, OnTimerRanOut);
        EventManager.StopListening(On.PointObtained, OnPointObtained);
        EventManager.StopListening(On.PointTimedOut, OnPointTimedOut);
        
    }

    private void OnTimerRanOut()
    {
        if (_currentScore < StageManager.Instance.currentStage.star1Score)
        {
            EventManager.TriggerEvent(On.TookDamage, null);
        }
        else
        {
            EventManager.TriggerEvent(On.StageComplete, _currentScore);
        }
    }

    private void OnPointTimedOut()
    {
        _scoreMultiplier = 0;
        _textHighlightMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
        ShowComboText(false);
    }

    private void OnPointObtained()
    {
        if (_scoreMultiplier < 4)
            _scoreMultiplier++;
        comboStrBuilder.Clear();
        if(_scoreMultiplier <4)
            comboStrBuilder.Append($"COMBO x{_scoreMultiplier}");
        else
            comboStrBuilder.Append($"COMBO x MAX");
        
        _comboTextValue.text = comboStrBuilder.ToString();
        if(!_comboTextPanel.activeSelf) ShowComboText(true);
        _currentScore += (pointPerTile * _scoreMultiplier);
        _scoreText.text = _currentScore.ToString();
        IncreaseIntensity();
        if (_scoreMultiplier <= 4)
            _comboRectTransform.DOPunchScale(new Vector3(x: 0.5f, y: 0.5f, z: 0.5f), 0.3f, 1, 0.20f).SetLoops(1).SetEase(Ease.InOutBounce);
        if (_currentScore > 100)
        {
            EventManager.TriggerEvent(On.StageComplete, _currentScore);
        }
        
    }

    private void ShowComboText(bool value)
    {
        _comboTextPanel.SetActive(value);
    }

    private void OnLoadStage()
    {
        getReadyAnimation.gameObject.SetActive(true);
        getReadyAnimation.DOPlay();
    }

    public void OnLoadingAnimationComplete()
    {
        EventManager.TriggerEvent(On.StartGame, StageManager.Instance.currentStage);
    }
    
    
    private void IncreaseIntensity()
    {
        _textHighlightMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, _scoreMultiplier/10f*1.3f);
        _comboTextValue.fontSharedMaterial = _textHighlightMaterial;
    }
}

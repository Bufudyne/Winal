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
        EventManager.TriggerEvent(On.StartGame, null);
    }

    private void OnEnable()
    {
        EventManager.StartListening(On.LoadStage, OnLoadStage);
        EventManager.StartListening(On.PointObtained, OnPointObtained);
        EventManager.StartListening(On.PointTimedOut, OnPointTimedOut);
        EventManager.StartListening(On.TookDamage, OnTookDamage);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.LoadStage, OnLoadStage);
        EventManager.StopListening(On.PointObtained, OnPointObtained);
        EventManager.StopListening(On.PointTimedOut, OnPointTimedOut);
        EventManager.StopListening(On.TookDamage, OnTookDamage);
    }

    private void OnTookDamage()
    {
        _buttonRetry.SetActive(true);
    }

    public void OnClickButtonRetry()
    {
        EventManager.Reset();
        EventManager.Instance.aReset();
        
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        comboStrBuilder.Append($"COMBO x{_scoreMultiplier}");
        _comboTextValue.text = comboStrBuilder.ToString();
        if(!_comboTextPanel.activeSelf) ShowComboText(true);
        _currentScore += (pointPerTile * _scoreMultiplier);
        _scoreText.text = _currentScore.ToString();
        IncreaseIntensity();
        if (_scoreMultiplier <= 4)
            _comboRectTransform.DOPunchScale(new Vector3(x: 0.5f, y: 0.5f, z: 0.5f), 0.3f, 1, 0.20f).SetLoops(1).SetEase(Ease.InOutBounce);
        
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
    
    
    private void IncreaseIntensity()
    {
        _textHighlightMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, _scoreMultiplier/10f*1.3f);
        _comboTextValue.fontSharedMaterial = _textHighlightMaterial;
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour, ILevelService
{
    #region Components
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Transform _starTransform;
    #endregion

    #region Variables
    private int _currentLevel, _currentLevelOnText, _currentExp, _gainedLevel;
    private Coroutine _barSetRoutine;
    private readonly Queue<float> _barTargets = new();
    #endregion

    #region Properties
    private int ExpForNextLevel => _currentLevel * Constants.LEVEL_EXP_MULTIPLIER;
    private float ProgressRate => _currentExp / (float)ExpForNextLevel;
    #endregion

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<ILevelService>(this);
        InitLevelHandler();
    }

    private void InitLevelHandler()
    {
        _currentLevel = PlayerPrefs.GetInt(Constants.PLAYER_LEVEL_KEY,1);
        _currentExp = PlayerPrefs.GetInt(Constants.PLAYER_EXP_KEY,0);
        _levelText.text = _currentLevel.ToString();
        _currentLevelOnText = _currentLevel;
        _progressSlider.value = ProgressRate;
    }

    public void AddExp()
    {
        _currentExp += Constants.EXP_AT_EACH_CLICK;
        //We might got an exp and earn more than 1 level at once. So check it until we can't get any level
        for (; _currentExp >= ExpForNextLevel; _currentLevel++)
        {
            _currentExp -= ExpForNextLevel;
            SetProgressBar(1);
        }

        SetProgressBar(ProgressRate);
    }

    /// With this method, we can spam the Get Exp button. It will do all of the progress
    /// bar animations in order. And update the level UI at the end of progress bar anim completion
    /// If value is 0, we don't need to do anything because bar will set 0 on previous steps
    private void SetProgressBar(float value)
    {
        if (value == 0) return;

        _barTargets.Enqueue(value);
        _barSetRoutine ??= StartCoroutine(SetProgressBarRoutine());
    }

    private void UpdateLevelUIOnNextLevel()
    {
        _levelText.transform.DOKill();
        _starTransform.DOKill();
        _progressSlider.DOKill();
        _progressSlider.value = 0;
        _currentLevelOnText++;
        _levelText.text = _currentLevelOnText.ToString();
        _levelText.transform.DOScale(1, 0.25f).SetTarget(this).SetEase(Ease.OutBounce).From(0.5f);
        _starTransform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetTarget(this)
            .SetEase(Ease.OutBack);
    }

    //This function can be achieved with DoTween Sequences. But i chose to do it manually. 
    //If it seems a bit over-engineered sorry for that !
    private IEnumerator SetProgressBarRoutine()
    {
        while (_barTargets.Count > 0)
        {
            var currentValue = _barTargets.Dequeue();
            if (currentValue < 1)
            {
                yield return _progressSlider.DOValue(currentValue, Constants.BAR_MOVE_DURATION).SetEase(Ease.Linear)
                    .WaitForCompletion();
            }
            else
            {
                yield return _progressSlider.DOValue(1, Constants.BAR_MOVE_DURATION).SetEase(Ease.Linear)
                    .WaitForCompletion();
                UpdateLevelUIOnNextLevel();
            }
        }

        _barSetRoutine = null;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(Constants.PLAYER_LEVEL_KEY, _currentLevel);
        PlayerPrefs.SetInt(Constants.PLAYER_EXP_KEY, _currentExp);
    }
}
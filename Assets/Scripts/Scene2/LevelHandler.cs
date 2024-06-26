using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour,ILevelService
{
    #region Variables
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Transform _starTransform;
    private int _currentLevel, _currentLevelOnText, _currentExp;
    private Coroutine _barSetRoutine;
    private readonly List<float> _barTargets = new();
    #endregion

    #region Properties
    private int ExpForNextLevel => _currentLevel * Constants.EXP_AT_EACH_CLICK;
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
        //We set progress bar here because we will do some animations
        //if progress bar rate >= 1 (it means next level)
        SetProgressBar();
        if (_currentExp < ExpForNextLevel) return;

        _currentExp -= ExpForNextLevel;
        _currentLevel++;
    }
    
    /// With this method, we can spam the Get Exp button. It will do all of the progress
    /// bar animations in order. And update the level UI at the end of progress bar anim completion
    private void SetProgressBar()
    {
        _barTargets.Add(ProgressRate);
        _barSetRoutine ??= StartCoroutine(SetProgressBarRoutine());
    }

    private void UpdateLevelUIOnNextLevel()
    {
        _levelText.transform.DOKill();
        _starTransform.DOKill();
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
            if (_barTargets[0] > _progressSlider.value && _barTargets[0] < 1)
            {
                yield return _progressSlider.DOValue(_barTargets[0], Constants.BAR_MOVE_DURATION).SetEase(Ease.Linear).WaitForCompletion();
            }
            else
            {
                //If our next target is smaller than our current progress, it means we passed to next level. So first fill the progress bar.
                //Then on complete, update the UI. For safer approach, it's only an UI animation. Actual level is not restored & updated on here.
                //It directly updated on GetExp() function.
                yield return _progressSlider.DOValue(1, Constants.BAR_MOVE_DURATION).SetEase(Ease.Linear).WaitForCompletion();
                UpdateLevelUIOnNextLevel();
                
                //Set the bar to where it should be after next level
                if (_barTargets[0] % 1 > 0)
                {
                    yield return _progressSlider.DOValue(_barTargets[0] % 1, Constants.BAR_MOVE_DURATION).SetEase(Ease.Linear).From(0).WaitForCompletion();
                }
                else
                {
                    _progressSlider.value = 0;
                }
            }
            _barTargets.RemoveAt(0);
        }
        _barSetRoutine = null;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(Constants.PLAYER_LEVEL_KEY,_currentLevel);
        PlayerPrefs.SetInt(Constants.PLAYER_EXP_KEY,_currentExp);
    }
}
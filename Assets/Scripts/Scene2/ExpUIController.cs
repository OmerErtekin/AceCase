using UnityEngine;
using UnityEngine.UI;

public class ExpUIController : MonoBehaviour
{
    #region Components
    [SerializeField] private Button GetExpButton;
    private ILevelService _levelService;
    #endregion

    private void Start()
    {
        _levelService = ServiceLocator.Instance.GetService<ILevelService>();
        GetExpButton.onClick.AddListener(OnGetExp);
    }

    private void OnGetExp()
    {
        _levelService.AddExp();
        Debug.Log("You clicked Get Exp button!");
    }
}

using UnityEngine;
using UnityEngine.UI;

public class InspectUIController : MonoBehaviour
{
    [SerializeField] private Slider _sweetSlider;
    [SerializeField] private Slider _sourSlider;
    [SerializeField] private Slider _bitterSlider;
    [SerializeField] private Slider _saltySlider;
    [SerializeField] private Slider _umamiSlider;
    private PlayerInspector _playerInspector;

    void Start()
    {
        _playerInspector = PlayerController.Instance.PlayerInspector;

        _playerInspector.OnInspect.AddListener(UpdateStats);
    }

    void UpdateStats(IInspectable inspectable)
    {
        if (inspectable == null)
        {
            _sweetSlider.gameObject.SetActive(false);
            _sourSlider.gameObject.SetActive(false);
            _bitterSlider.gameObject.SetActive(false);
            _saltySlider.gameObject.SetActive(false);
            _umamiSlider.gameObject.SetActive(false);
        } else
        {
            _sweetSlider.gameObject.SetActive(true);
            _sourSlider.gameObject.SetActive(true);
            _bitterSlider.gameObject.SetActive(true);
            _saltySlider.gameObject.SetActive(true);
            _umamiSlider.gameObject.SetActive(true);
            SetStats(inspectable.GetStats());
        }
    }

    public void SetStats(IngredientData data)
    {
        _sweetSlider.value = data.SweetValue;
        _sourSlider.value = data.SourValue;
        _bitterSlider.value = data.BitterValue;
        _saltySlider.value = data.SaltyValue;
        _umamiSlider.value = data.UmamiValue;
    }
}

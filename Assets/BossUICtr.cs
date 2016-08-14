using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossUICtr : MonoBehaviour
{
    private Slider _hpSlider;

    private void Awake()
    {
        _hpSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        _hpSlider.value = 1f;
    }

	public void SetHP (float percent)
    {
        _hpSlider.value = percent >= 0 ? percent : 0;
	}

    public void SetHP(float cur, float max)
    {
        this.SetHP(cur / max);
    }

    public void EnableHpSlider(bool enable)
    {
        gameObject.SetActive(enable);
    }
}

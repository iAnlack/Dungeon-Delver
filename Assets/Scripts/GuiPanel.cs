using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiPanel : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Dray Dray;
    public Sprite HealthEmpty;
    public Sprite HealthHalf;
    public Sprite HealthFull;

    private Text _keyCountText;
    private List<Image> _healthImages;

    private void Start()
    {
        // Счётчик ключей
        Transform trans = transform.Find("Key Count");
        _keyCountText = trans.GetComponent<Text>();

        // Индикатор уровня здоровья
        Transform healthPanel = transform.Find("Health Panel");
        _healthImages = new List<Image>();
        if (healthPanel != null)
        {
            for (int i = 0; i < 20; i++)
            {
                trans = healthPanel.Find("H_" + i);
                if (trans == null)
                {
                    break;
                }
                _healthImages.Add(trans.GetComponent<Image>());
            }
        }
    }

    private void Update()
    {
        // Показать количество ключей
        _keyCountText.text = Dray.NumKeys.ToString();

        // Показать уровень здоровья
        int health = Dray.Health;
        for (int i = 0; i < _healthImages.Count; i++)
        {
            if (health > 1)
            {
                _healthImages[i].sprite = HealthFull;
            }
            else if (health == 1)
            {
                _healthImages[i].sprite = HealthHalf;
            }
            else
            {
                _healthImages[i].sprite = HealthEmpty;
            }
            health -= 2;
        }
    }
}

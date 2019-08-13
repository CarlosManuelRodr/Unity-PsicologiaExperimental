﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de opción de volumen del audio.
/// </summary>
public class VolumeController : MonoBehaviour
{
    public GameObject ownNumber;
    public GameObject gameManager;

    private TextMeshProUGUI ownText;
    private Slider ownSlider;
    private Jukebox jukebox;

    void Start()
    {
        ownText = ownNumber.GetComponent<TextMeshProUGUI>();
        ownSlider = GetComponent<Slider>();

        if (gameManager != null)
            jukebox = gameManager.GetComponent<Jukebox>();

        int value = PlayerPrefs.GetInt("Volume", -1);
        if (value == -1)
            value = 100;

        ownSlider.value = value;
        ownText.SetText(value.ToString());
    }

    public void OnUpdateValue()
    {
        int newValue = (int) ownSlider.value;
        ownText.SetText(newValue.ToString());

        if (jukebox != null)
            jukebox.volume = (float) newValue;

        PlayerPrefs.SetInt("Volume", newValue);
    }
}

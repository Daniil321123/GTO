using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    Resolution[] resolutions;


    public void textureQuality(int indexTextureQuality)
    {
        Debug.Log(indexTextureQuality);
        QualitySettings.masterTextureLimit = indexTextureQuality;
    }

    public void antiAlysingSetting(int antyAlysingIndex)
    {
        QualitySettings.antiAliasing = antyAlysingIndex;
    }

    public void setQualityLevel(int QualityLevel)
    {
        QualitySettings.SetQualityLevel(QualityLevel);
    }


    public void setShadow(int shadowIndex)
    {
        if(shadowIndex == 0) QualitySettings.shadows = ShadowQuality.All;
        if(shadowIndex == 1) QualitySettings.shadows = ShadowQuality.Disable;

    }
}

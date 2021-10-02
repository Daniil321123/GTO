using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class TuningDetails : NetworkBehaviour
{
    const int FRONTBUMPER = 0;
    const int REARBUMPER = 1;
    const int EXHAUSTS = 2;
    const int EXTS = 3;
    const int FENDERS= 4;
    const int ROOFS = 5;
    const int SPOILERS = 6;

    public bool isTuningModeOn = false;

    [SerializeField] private Canvas menuTuning;

    [SerializeField] public GameObject[] frontBumpers;
    [SerializeField] public GameObject[] rearBumpers;
    [SerializeField] public GameObject[] exhausts;
    [SerializeField] public GameObject[] exts;
    [SerializeField] public GameObject[] fenders;
    [SerializeField] public GameObject[] roofs;
    [SerializeField] public GameObject[] spoilers;

    private void Update()
    {
        if (isTuningModeOn)
        {
            isTuningModeOn = false;
            Canvas menuInst = Instantiate(menuTuning);
            menuInst.GetComponent<ButtonForTuningMenu>().carDetails = this;
        }
    }

    public void setDetails(int currentDetail, int tuningCategory)
    {
        switch (tuningCategory)
        {
            case FRONTBUMPER:
                setCurrentDetail(frontBumpers, currentDetail);
                break;
            case REARBUMPER:
                setCurrentDetail(rearBumpers, currentDetail);
                break;
            case EXHAUSTS:
                setCurrentDetail(exhausts, currentDetail);
                break;
            case EXTS:
                setCurrentDetail(exts, currentDetail);
                break;
            case FENDERS:
                setCurrentDetail(fenders, currentDetail);
                break;
            case ROOFS:
                setCurrentDetail(roofs, currentDetail);
                break;
            case SPOILERS:
                setCurrentDetail(spoilers, currentDetail);
                break;
        }
    }


    private void setCurrentDetail(GameObject[] details, int indexDetail)
    {
        if (details.Length > 0)
        {
            if (indexDetail >= details.Length)
            {
                indexDetail = 0;
            }
            for (int i = 0; i < details.Length - 1; i++)
            {
                details[i].gameObject.SetActive(false);
            }
            details[indexDetail].gameObject.SetActive(true);
        }
    }
}

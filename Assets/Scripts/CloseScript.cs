using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseScript : MonoBehaviour
{
    public GameObject targetToHide;

    public void Hide()
    {
        if (targetToHide != null)
            targetToHide.SetActive(false);
    }
}

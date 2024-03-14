using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;



    public void ShowPanel1()
    {
        HideAllPanels();
        panel1.SetActive(true);
    }

    public void ShowPanel2()
    {
        HideAllPanels();
        panel2.SetActive(true);
    }

    public void ShowPanel3()
    {
        HideAllPanels();
        panel3.SetActive(true);
    }

    public void ShowPanel4()
    {
        HideAllPanels();
        panel4.SetActive(true);
    }

    public void HideAllPanels()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        panel4.SetActive(false);
    }
}

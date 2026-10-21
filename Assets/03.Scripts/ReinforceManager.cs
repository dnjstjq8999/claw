using UnityEngine;

public class ReinforceManager : MonoBehaviour
{
    [Header("--- 4개의 업그레이드 패널 (순서대로 0~3) ---")]
    public GameObject[] upgradePanels;
    // 0: 이동속도, 1: 집게속도, 2: 뽑기실력, 3: 시간강화 패널
    public GameObject reinforcePanel;
    public GameObject homePanel;

    private void Start()
    {
        ShowPanel(0);
    }

    public void ShowPanel(int index)
    {
        for (int i = 0; i < upgradePanels.Length; i++)
        {
            if (upgradePanels[i] != null)
            {
                upgradePanels[i].SetActive(i == index);
            }
        }
    }

    public void OnClickBackButton()
    {
        if (reinforcePanel != null) reinforcePanel.SetActive(false);
        if (homePanel != null) homePanel.SetActive(true);
    }
}
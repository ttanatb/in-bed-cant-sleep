using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class HomeScreenController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_timeNumber = null;
    [SerializeField]
    TextMeshProUGUI m_amPmText = null;
    [SerializeField]
    TextMeshProUGUI m_dateText = null;
    TimeManager m_timeManager = null;

    Button m_picButton = null;

    // Start is called before the first frame update
    void Start()
    {
        m_timeManager = TimeManager.Instance;
        m_timeManager.StartTime();
        m_timeManager.AddOnTimerTickListener(OnTimerTicker);
    }

    private void OnTimerTicker(System.DateTime time)
    {
        m_timeNumber.text = time.ToString("hh:mm");
        m_dateText.text = time.ToString("dddd, MMMM d");
        m_amPmText.text = time.ToString("tt");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

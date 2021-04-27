using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePageController : MonoBehaviour
{
    [SerializeField]
    List<AppIconController> m_appIcons = new List<AppIconController>();

    [SerializeField]
    float m_loadingTimeMin = 12301230;


    [SerializeField]
    float m_loadingTimeMax = 212301230;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var a in m_appIcons)
        {
            a.SetLoading(Random.Range(m_loadingTimeMin, m_loadingTimeMax));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

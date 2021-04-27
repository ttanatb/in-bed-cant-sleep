using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class TextingTopPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_name = null;

    [SerializeField]
    private Image m_image = null;

    public void Init(ContactModel model)
    {
        m_name.text = model.Name;
        m_image.sprite = model.ProfilePic;
    }
}

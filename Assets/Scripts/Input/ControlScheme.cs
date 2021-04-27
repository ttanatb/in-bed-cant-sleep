using System.Collections;
using System.Collections.Generic;

public class ControlScheme
{
    InputMapping m_inputMapping = null;
    private static ControlScheme m_instance = null;

    private ControlScheme()
    {

    }

    public static ControlScheme Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ControlScheme();
            }

            return m_instance;
        }
    }

    public InputMapping InputMapping
    {
        get
        {
            if (m_inputMapping == null)
            {
                m_inputMapping = new InputMapping();
                m_inputMapping.Enable();
                m_inputMapping.Gameplay.Enable();
            }
            return m_inputMapping;
        }
    }
}

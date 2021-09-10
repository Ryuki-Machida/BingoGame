using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] Image m_image = null;
    [SerializeField] Text m_numberText = null;
    [SerializeField] State m_state = State.Close;
    public string m_column;
    public int m_number;

    public State State
    {
        get => m_state;
        set
        {
            m_state = value;
            OnSteteChanged();
        }
    }

    private void OnValidate()
    {
        OnSteteChanged();
    }

    private void OnSteteChanged()
    {
        if (m_state == State.Open)
        {
            m_image.color = Color.red;
        }
        else if (m_state == State.Close)
        {
            m_image.color = Color.white;
        }
    }

    public void GetNumder(int n)
    {
        m_number = n;
        m_numberText.text = n.ToString();
    }

    public void GetColumn(string c)
    {
        m_column = c;
    }
}

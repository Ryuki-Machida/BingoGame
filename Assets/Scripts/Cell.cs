using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    Close = 0,
    Open = 1,
    StateOpen = 2,
}

public class Cell : MonoBehaviour
{
    [SerializeField] Text m_numberText = null;
    [SerializeField] State m_state = State.Close;
    [SerializeField] Image m_image = null;
    public int m_row;
    public int m_col;
    public int m_number;

    public void GetCoordinate(int r, int c)
    {
        m_row = r;
        m_col = c;
    }

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
            m_image.color = Color.gray;
        }
        else if (m_state == State.Close)
        {
            m_image.color = Color.white;
        }
    }

    public void GetNumber(int n)
    {
        m_number = n;
        m_numberText.text = n.ToString();
        if (m_row == 2 && m_col == 2)
        {
            m_numberText.text = "F";
            m_number = 0;
            m_state = State.Open;
            OnSteteChanged();
        }
    }
}

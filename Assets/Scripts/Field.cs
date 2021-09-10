using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Field : MonoBehaviour
{
    /// <summary>�Z���̃v���n�u<summary>
    [SerializeField] Cell m_cellPrefab;
    /// <summary>�ԍ��̃v���n�u<summary>
    [SerializeField] Board m_boardPrefab;
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] GridLayoutGroup m_board = null;
    /// <summary>�c���̒���<summary>
    [SerializeField] int m_row = 5;
    [SerializeField] int m_col = 5;
    /// <summary>�{�[�h�̏c���̒���<summary>
    [SerializeField] int m_boardRow = 15;
    [SerializeField] int m_boardCol = 5;

    [SerializeField] Text m_resultText = null;
    [SerializeField] GameObject m_resultPanel = null;
    /// <summary>�Z���̔z��<summary>
    Cell[,] m_bingoCell;
    /// <summary>�ԍ��̔z��<summary>
    Board[,] m_bingoBoardNums;
    private int m_turn = 0;

    void Start()
    {
        if (m_col < m_row)
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_container.constraintCount = m_row;
        }
        else
        {
            m_container.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            m_container.constraintCount = m_col;
        }

        m_bingoCell = new Cell[m_row, m_col];
        //�r���S�J�[�h�𐶐�
        for (int col = 0; col < m_col; col++)
        {
            for (int row = 0; row < m_row; row++)
            {
                var cell = Instantiate(m_cellPrefab);
                var parent = m_container.transform;
                cell.transform.SetParent(parent);
                m_bingoCell[row, col] = cell;
                cell.GetCoordinate(row, col);
            }
        }

        //�r���S�J�[�h�ɔԍ���t�^
        for (int row = 0; row < m_row; row++)
        {
            int[] nums = new int[5];//���I���ʂ����ɓ���Ă����z��
            for (int col = 0; col < m_col; col++)
            {
                bool Substitution = false;
                int num = Random.Range(1 + (row * 15), 16 + (row * 15));//�����𒊑I����
                foreach (var i in nums)//���I�������������łɏo�Ă��邩���ׂ�
                {
                    if (i == num)
                    {
                        col--;
                        Substitution = false;
                        break;
                    }
                    else
                    {
                        Substitution = true;
                    }
                }

                if (Substitution == true)//���I�����������o�Ă��Ȃ������璊�I���ʂɉ�����
                {
                    nums[col] = num;
                }
            }
            
            //���I���I�������\�[�g����
            for (int f = 1; f < nums.Length; f++)//�C���T�[�g�\�[�g
            {
                for (int s = f; s > 0; s--)
                {
                    if (nums[s - 1] > nums[s])
                    {
                        int seve = nums[s - 1];
                        nums[s - 1] = nums[s];
                        nums[s] = seve;
                    }
                }
            }

            //�\�[�g���I������炻�ꂼ��̃Z���ɒ��I���ʂ𑗂�
            for (int col = 0; col < nums.Length; col++)
            {
                m_bingoCell[row, col].GetNumber(nums[col]);
            }
        }

        if (m_boardCol < m_boardRow)
        {
            m_board.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_board.constraintCount = m_boardRow;
        }
        else
        {
            m_board.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            m_board.constraintCount = m_boardCol;
        }

        m_bingoBoardNums = new Board[m_boardRow, m_boardCol];
        //�ԍ��𐶐�
        for (int col = 0; col < m_boardCol; col++)
        {
            for (int row = 0; row < m_boardRow; row++)
            {
                var cell = Instantiate(m_boardPrefab);
                var parent = m_board.transform;
                cell.transform.SetParent(parent);
                m_bingoBoardNums[row, col] = cell;
                cell.GetNumder(row + 1 + col * 15);
                if (col == 0)
                {
                    cell.GetColumn("B");
                }
                else if (col == 1)
                {
                    cell.GetColumn("I");
                }
                else if (col == 2)
                {
                    cell.GetColumn("N");
                }
                else if (col == 3)
                {
                    cell.GetColumn("G");
                }
                else
                {
                    cell.GetColumn("O");
                }
            }
        }
    }

    int[] colNumber = { 0, 1, 2, 3, 4 };
    int emptyCol = 0;//State.Close�����O�ɂȂ�����̐�

    /// <summary>
    /// �����𒊑I����
    /// </summary>
    public void Lottery()
    {
        if (m_turn < m_bingoBoardNums.Length)
        {
            int col = Random.Range(0, colNumber.Length - emptyCol);//�����_���ɗ�ԍ��𒊏o����
            int[] rowNum = new int[15];//State.Close�̐�����ۑ����Ă���
            int remainingRow = rowNum.Length;//State.Close����ۑ����Ă���

            for (int rows = 0; rows < rowNum.Length; rows++)//���̍s��State.Close�𐔂���
            {
                if (m_bingoBoardNums[rows, colNumber[col]].State == State.Close)
                {
                    rowNum[rowNum.Length - 1 - rows] = rows;
                    remainingRow--;
                }
            }

            //State.Close�̐�����ۑ����I�������\�[�g����
            for (int f = 1; f < rowNum.Length; f++)//�C���T�[�g�\�[�g
            {
                for (int s = f; s > 0; s--)
                {
                    if (rowNum[s - 1] < rowNum[s])
                    {
                        int seve = rowNum[s - 1];
                        rowNum[s - 1] = rowNum[s];
                        rowNum[s] = seve;
                    }
                }
            }

            int row = Random.Range(0, m_boardRow - remainingRow);//�����_���ɍs�ԍ��𒊏o����
            int rowN = rowNum[row];
            m_turn++;
            m_bingoBoardNums[rowN, colNumber[col]].State = State.Open;
            m_resultText.text = m_turn + "��ځF" + m_bingoBoardNums[rowN, colNumber[col]].m_column + "��" + m_bingoBoardNums[rowN, colNumber[col]].m_number + "���o��";
            NumberSearch(colNumber[col], m_bingoBoardNums[rowN, colNumber[col]].m_number);

            if (remainingRow >= rowNum.Length - 1)//State.Close�����O�������炻�̍s�𒊏o�ł��Ȃ��悤�ɂ���
            {
                int seve = colNumber[col];
                colNumber[col] = colNumber[m_boardCol - 1 - emptyCol];
                colNumber[m_boardCol - 1 - emptyCol] = seve;
                emptyCol++;
            }
        }
        else
        {
            Debug.Log("�����Ȃ�");
        }
    }

    /// <summary>
    /// ���I�ԍ����r���S�J�[�h�ɂ��邩���ׂ�
    /// </summary>
    void NumberSearch(int row, int number)
    {
        for (int col = 0; col < m_col; col++)
        {
            if (m_bingoCell[row, col].m_number == number)
            {
                m_bingoCell[row, col].State = State.Open;
                BingoSearch(row, col);
            }
        }
    }

    /// <summary>
    /// �r���S�ɂȂ������𒲂ׂ�
    /// </summary>
    void BingoSearch(int row, int col)
    {
        int openRow = 0;//���̋󂢂Ă������ۑ����Ă���
        int openCol = 0;//�c�̋󂢂Ă������ۑ����Ă���
        int openDiagonalLeft = 0;//���ォ��΂߂̋󂢂Ă������ۑ����Ă���
        int openDiagonalRight = 0;//�E�ォ��΂߂̋󂢂Ă������ۑ����Ă���
        for (int searchRow = 0; searchRow < m_row; searchRow++)//���̋󂢂Ă�����𐔂���
        {
            if (m_bingoCell[row, searchRow].State == State.Open)
            {
                openRow++;
            }
        }

        for (int searchCol = 0; searchCol < m_col; searchCol++)//�c�̋󂢂Ă�����𐔂���
        {
            if (m_bingoCell[searchCol, col].State == State.Open)
            {
                openCol++;
            }
        }

        if (row == col || row + col == 4)
        {
            for (int search = 0; search < m_col; search++)
            {
                if (m_bingoCell[search, search].State == State.Open)//���ォ��΂߂̋󂢂Ă�����𐔂���
                {
                    openDiagonalLeft++;
                }
            }

            for (int search = 0; search < m_col; search++)
            {
                if (m_bingoCell[4 - search, search].State == State.Open)//�E�ォ��΂߂̋󂢂Ă�����𐔂���
                {
                    openDiagonalRight++;
                }
            }
        }

        if (openRow == 5 || openCol == 5 || openDiagonalLeft == 5 || openDiagonalRight == 5)
        {
            Bingo();
        }
    }

    void Bingo()
    {
        m_resultPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("BingoGame");
    }
}

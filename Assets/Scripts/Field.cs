using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Field : MonoBehaviour
{
    /// <summary>セルのプレハブ<summary>
    [SerializeField] Cell m_cellPrefab;
    /// <summary>番号板のプレハブ<summary>
    [SerializeField] Board m_boardPrefab;
    [SerializeField] GridLayoutGroup m_container = null;
    [SerializeField] GridLayoutGroup m_board = null;
    /// <summary>縦横の長さ<summary>
    [SerializeField] int m_row = 5;
    [SerializeField] int m_col = 5;
    /// <summary>ボードの縦横の長さ<summary>
    [SerializeField] int m_boardRow = 15;
    [SerializeField] int m_boardCol = 5;

    [SerializeField] Text m_resultText = null;
    [SerializeField] GameObject m_resultPanel = null;
    /// <summary>セルの配列<summary>
    Cell[,] m_bingoCell;
    /// <summary>番号板の配列<summary>
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
        //ビンゴカードを生成
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

        //ビンゴカードに番号を付与
        for (int row = 0; row < m_row; row++)
        {
            int[] nums = new int[5];//抽選結果を仮に入れておく配列
            for (int col = 0; col < m_col; col++)
            {
                bool Substitution = false;
                int num = Random.Range(1 + (row * 15), 16 + (row * 15));//数字を抽選する
                foreach (var i in nums)//抽選した数字がすでに出ているか調べる
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

                if (Substitution == true)//抽選した数字が出ていなかったら抽選結果に加える
                {
                    nums[col] = num;
                }
            }
            
            //抽選が終わったらソートする
            for (int f = 1; f < nums.Length; f++)//インサートソート
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

            //ソートが終わったらそれぞれのセルに抽選結果を送る
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
        //番号板を生成
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
    int emptyCol = 0;//State.Close数が０になった列の数

    /// <summary>
    /// 数字を抽選する
    /// </summary>
    public void Lottery()
    {
        if (m_turn < m_bingoBoardNums.Length)
        {
            int col = Random.Range(0, colNumber.Length - emptyCol);//ランダムに列番号を抽出する
            int[] rowNum = new int[15];//State.Closeの数字を保存しておく
            int remainingRow = rowNum.Length;//State.Close数を保存しておく

            for (int rows = 0; rows < rowNum.Length; rows++)//その行のState.Closeを数える
            {
                if (m_bingoBoardNums[rows, colNumber[col]].State == State.Close)
                {
                    rowNum[rowNum.Length - 1 - rows] = rows;
                    remainingRow--;
                }
            }

            //State.Closeの数字を保存し終わったらソートする
            for (int f = 1; f < rowNum.Length; f++)//インサートソート
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

            int row = Random.Range(0, m_boardRow - remainingRow);//ランダムに行番号を抽出する
            int rowN = rowNum[row];
            m_turn++;
            m_bingoBoardNums[rowN, colNumber[col]].State = State.Open;
            m_resultText.text = m_turn + "回目：" + m_bingoBoardNums[rowN, colNumber[col]].m_column + "の" + m_bingoBoardNums[rowN, colNumber[col]].m_number + "が出た";
            NumberSearch(colNumber[col], m_bingoBoardNums[rowN, colNumber[col]].m_number);

            if (remainingRow >= rowNum.Length - 1)//State.Close数が０だったらその行を抽出できないようにする
            {
                int seve = colNumber[col];
                colNumber[col] = colNumber[m_boardCol - 1 - emptyCol];
                colNumber[m_boardCol - 1 - emptyCol] = seve;
                emptyCol++;
            }
        }
        else
        {
            Debug.Log("もうない");
        }
    }

    /// <summary>
    /// 抽選番号がビンゴカードにあるか調べる
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
    /// ビンゴになったかを調べる
    /// </summary>
    void BingoSearch(int row, int col)
    {
        int openRow = 0;//横の空いている個数を保存しておく
        int openCol = 0;//縦の空いている個数を保存しておく
        int openDiagonalLeft = 0;//左上から斜めの空いている個数を保存しておく
        int openDiagonalRight = 0;//右上から斜めの空いている個数を保存しておく
        for (int searchRow = 0; searchRow < m_row; searchRow++)//横の空いている個数を数える
        {
            if (m_bingoCell[row, searchRow].State == State.Open)
            {
                openRow++;
            }
        }

        for (int searchCol = 0; searchCol < m_col; searchCol++)//縦の空いている個数を数える
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
                if (m_bingoCell[search, search].State == State.Open)//左上から斜めの空いている個数を数える
                {
                    openDiagonalLeft++;
                }
            }

            for (int search = 0; search < m_col; search++)
            {
                if (m_bingoCell[4 - search, search].State == State.Open)//右上から斜めの空いている個数を数える
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

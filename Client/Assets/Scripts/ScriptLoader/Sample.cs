using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.Console;

public class Sample
{
    string _ImportPath;
    byte[] _GetBinData;
    List<byte[]> _ByteList;
    List<string> _ConvertToStringByteList;

    /// <summary>
    /// CSVファイルをバイナリデータで取り込みしてリスト化
    /// </summary>
    /// <param name="path">CSVのパス</param>
    public void ImportCSVGetBin(string path)
    {
        _ImportPath = path;

        _GetBinData = null;
        System.IO.FileStream FileStream = null;
        System.IO.BinaryReader BinaryReader = null;

        try
        {
            // フォルダパスのファイルを開くオブジェクト
            FileStream = new System.IO.FileStream(_ImportPath, System.IO.FileMode.Open);
            // FileStreamの内容をバイナリーデータで読み取るオブジェクト
            BinaryReader = new System.IO.BinaryReader(FileStream);

            // ファイルの最後までをinteger型で取り込む
            int Length = System.Convert.ToInt32(FileStream.Length);

            // ファイルの最後までをバイナリ型で読み込み、変数にいれる
            _GetBinData = BinaryReader.ReadBytes(Length);

            FileStream.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }

        int StartPoint = 0;  // 開始位置
        int EndPoint = 0;    // 終了位置
        int LengthValue = 0; // 一行の配列の長さ 
        _ByteList = new List<byte[]>();

        while (EndPoint >= 0)
        {
            EndPoint = SearchCRLF(StartPoint);

            if (EndPoint < 0)
            {
                LengthValue = _GetBinData.Length - StartPoint;

                if (LengthValue <= 0)
                    break;
            }
            else
                // CRLF分減らす
                LengthValue = EndPoint - StartPoint + 1 - 2;

            // 行データ取得
            byte[] LineVal = GetByteFromByte(_GetBinData, StartPoint, LengthValue);

            _ByteList.Add(LineVal);

            StartPoint = EndPoint + 1;
        }

        // 取込データ出力
        int count = 0;
        foreach (var item in _ByteList)
        {
            string s = System.Text.Encoding.Default.GetString(_ByteList[count]);
            count++;


            string[] Values = s.Split(',');
            _ConvertToStringByteList = new List<string>();
            _ConvertToStringByteList.AddRange(Values);

            foreach (var item2 in _ConvertToStringByteList)
            {
                Debug.Log(item2);
            }

            Debug.Log("----------------------");
        }
    }

    /// <summary>
    /// CRLF探索  読み込みデータからのCRLFの位置を返す
    /// </summary>
    /// <param name="StartPoint">開始位置</param>
    /// <returns>CRLFの位置</returns>
    protected int SearchCRLF(int StartPoint)
    {
        int ResultCRLF = -1;

        for (int i = StartPoint; i <= _GetBinData.Length - 2; i++)
        {
            // &HD:13　キャリッジリターン　&HA:10  ラインフィード
            if (_GetBinData[i] == 0xD & _GetBinData[i + 1] == 0xA)
            {
                ResultCRLF = i + 1;
                break;
            }
        }

        return ResultCRLF;
    }

    /// <summary>
    /// Byte配列から位置、長さでByteを取得する
    /// </summary>
    /// <param name="InByte">バイト配列</param>
    /// <param name="StartPoint">開始位置</param>
    /// <param name="LengthValue">配列の長さ</param>
    /// <returns>１行分のバイト配列</returns>
    protected byte[] GetByteFromByte(byte[] InByte, int StartPoint, int LengthValue)
    {
        byte[] OutByte;

        if (LengthValue == 0)
            LengthValue = InByte.Length - StartPoint;

        if (LengthValue < 1)
        {
            OutByte = new byte[0] { };
            return OutByte;
        }

        OutByte = new byte[LengthValue - 1 + 1];

        for (int i = 0; i <= LengthValue - 1; i++)
        {
            if (StartPoint + i < InByte.Length)
                OutByte[i] = InByte[StartPoint + i];
        }

        return OutByte;
    }
}

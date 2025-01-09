using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using GameEngine;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ScriptLoader
{
    public class ScriptLoader : MonoBehaviour
    {
        private void Start()
        {
            CsvToBin();
        }

        public static void CsvToBin()
        {
            string csvPath = Application.dataPath + "/Resources/Table/ItemTable.csv";
            string binFilePath = Application.dataPath + "/Resources/Table/ItemTable.bin";
            
            try
            {
                // CSV 파일을 한 줄씩 읽음
                string[] csvLines = File.ReadAllLines(csvPath);

                using (BinaryWriter writer = new BinaryWriter(File.Open(binFilePath, FileMode.Create)))
                {
                    foreach (string line in csvLines)
                    {
                        // CSV 각 줄의 데이터를 쉼표(,)로 분리
                        string[] values = line.Split(',');

                        foreach (string value in values)
                        {
                            // 값의 바이트 변환
                            byte[] byteValue = GetBytes(value);

                            // 변환된 바이트 배열을 BIN 파일에 씀
                            writer.Write(byteValue);
                            Debug.Log(value +" : "+byteValue.GetString());
                        }
                    }
                }

                Debug.Log("CSV 파일이 성공적으로 BIN 파일로 변환되었습니다.");
            }
            catch (Exception ex)
            {
                Debug.LogError("오류 발생: " + ex.Message);
            }
        }
        
        static byte[] GetBytes(string value)
        {
            // 숫자라면 숫자 타입에 맞춰 변환
            if (int.TryParse(value, out int intValue))
            {
                byte[] intBytes = BitConverter.GetBytes(intValue);
                //Array.Reverse(intBytes);
                return intBytes;
            }
            else if (float.TryParse(value, out float floatValue))
            {
                byte[] floatBytes = BitConverter.GetBytes(floatValue);
                //Array.Reverse(floatBytes);
                return floatBytes;
            }
            else
            {
                // 문자열이면 UTF8로 인코딩하여 바이트 배열로 변환
                return Encoding.UTF8.GetBytes(value);
            }
        }

        public static void ReadBin(string filePath)
        {
            try
            {
                // 파일을 바이너리 모드로 읽기
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    // 정수(int) 읽기
                    int intValue = reader.ReadInt32();
                    Console.WriteLine("정수: " + intValue);

                    // 실수(float) 읽기
                    float floatValue = reader.ReadSingle();
                    Console.WriteLine("실수: " + floatValue);

                    // 문자열 읽기 (길이 먼저 읽은 후 그 길이만큼 문자열 읽기)
                    int stringLength = reader.ReadInt32(); // 문자열 길이 저장 방식
                    byte[] stringBytes = reader.ReadBytes(stringLength);
                    string stringValue = Encoding.UTF8.GetString(stringBytes);
                    Console.WriteLine("문자열: " + stringValue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("파일을 읽는 중 오류 발생: " + e.Message);
            }
        }
        
        
        public static void LoadCSV()
        {
            Stopwatch timer = new();
            timer.Start();
            Resources.Load<TextAsset>("Table/ItemTable.csv");
            timer.Stop();
            Debug.Log("csv : " + timer.ElapsedMilliseconds + "ms");
        }
        
        public static void LoadBIN()
        {
            Stopwatch timer = new();
            timer.Start();
            Resources.Load<TextAsset>("Table/ItemTable2.bytes");
            timer.Stop();
            Debug.Log("bytes : " + timer.ElapsedMilliseconds + "ms");
        }
    }
}
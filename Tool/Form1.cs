using BinDataLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static ByteConverter.Define;

namespace ByteConverter
{
    public partial class Form1 : Form
    {
        static string workspace => Directory.GetCurrentDirectory();

        public Form1()
        {
            InitializeComponent();
            Console.WriteLine(Environment.Is64BitOperatingSystem);
        }

        private void OnClickSerializeButton(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePathText.Text))
                return;

            SerializeFiles(filePathText.Text);
        }

        private void SerializeFiles(string folderPath)
        {
           string[] filePaths = Directory.GetFiles(folderPath);

            foreach(string csvFilePath in filePaths)
            {
                string extension = System.IO.Path.GetExtension(csvFilePath);
                if (string.Compare(extension, ".csv") != 0)
                    continue;

                var binPath = Path.ChangeExtension(csvFilePath, ".bytes");
                using (var reader = new StreamReader(csvFilePath))
                using (var fileStream = new FileStream(binPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new BinaryWriter(fileStream))
                {
                    string line = reader.ReadLine();
                    List<ColumnType> columnTypes = new List<ColumnType>();
                    foreach(var column in line.Split(','))
                    {
                        columnTypes.Add((ColumnType)Enum.Parse(typeof(ColumnType), column));
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        int columnIndex = 0;
                        foreach (string value in values)
                        {
                            if (columnTypes[columnIndex] == ColumnType.Int32)
                                writer.Write(int.Parse(value));
                            else if (columnTypes[columnIndex] == ColumnType.Int64)
                                writer.Write(long.Parse(value));

                            columnIndex++;
                        }
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void OnClickCreateDummy(object sender, EventArgs e)
        {
            TestFileGenerator.Create();
        }
    }
}

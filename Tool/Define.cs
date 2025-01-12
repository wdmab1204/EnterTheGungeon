using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteConverter
{
    internal class Define
    {
        public enum ColumnType : byte
        {
            Int32, // int
            Int64, // long
            Float,
            String
        }
    }
}

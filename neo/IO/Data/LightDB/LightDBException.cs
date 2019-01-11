using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Neo.IO.Data.LightDB
{
    class LightDBException : DbException
    {
        internal LightDBException(string message)
            : base(message)
        {
        }
    }
}

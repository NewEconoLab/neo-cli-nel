using System;
using System.Collections.Generic;
using System.Text;
using LTSnapshot = LightDB.SnapShot;
using LTTableIterator = LightDB.TableIterator;
using LTISnapshot = LightDB.ISnapShot;



namespace Neo.IO.Data.LightDB
{
    public class Iterator : LTTableIterator
    {
        public Iterator(LTSnapshot snapshot, byte[] _tableid, byte[] _beginkeyfinal, byte[] _endkeyfinal) : base(snapshot, _tableid, _beginkeyfinal, _endkeyfinal)
        {

        }
    }
}

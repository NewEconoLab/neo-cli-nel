using System;
using LTSnapshot = LightDB.SnapShot;
using LTISnapshot = LightDB.ISnapShot;

namespace Neo.IO.Data.LightDB
{
    public class Snapshot : LTSnapshot, LTISnapshot
    {
        public Snapshot(IntPtr dbPtr) : base(dbPtr)
        {

        }
    }
}

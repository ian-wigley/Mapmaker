using Newtonsoft.Json;
using System;
using System.Drawing;

namespace mapmaker
{
    [Serializable]
    class Tile
    {
        private Bitmap mBit;
        private int mX;
        private int mY;
        private string mAsciiCode;
        private string mType;

        public Tile(string asc)
        {
            mAsciiCode = asc;
        }

        [JsonConstructor]
        public Tile(int X, int Y, string asc, string type)
        {
            mX = X;
            mY = Y;
            mAsciiCode = asc;
            mType = type;
        }

        [JsonIgnoreAttribute]
        public Bitmap BitmapTile
        {
            get { return mBit; }
            set { mBit = value; }
        }

        public int XStart
        {
            get { return mX; }
            set { mX = value; }
        }

        public int YStart
        {
            get { return mY; }
            set { mY = value; }
        }

        public string AsciiCode
        {
            get { return mAsciiCode; }
            set { mAsciiCode = value; }
        }

        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }

    }
}

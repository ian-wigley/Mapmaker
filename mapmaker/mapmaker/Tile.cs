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

        public Tile(string asc)
        {
            mAsciiCode = asc;
        }

        public Tile(Bitmap bitmap, int X, int Y, string asc)
        {
            mBit = bitmap;
            mX = X;
            mY = Y;
            mAsciiCode = asc;
        }

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

    }
}

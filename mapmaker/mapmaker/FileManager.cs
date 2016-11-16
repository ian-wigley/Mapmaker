using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace mapmaker
{
    class FileManager
    {
        const int MaxWordCount = 80000;
        int wordListCount = 0;
        string[] wordList = new string[MaxWordCount];
        private int mLevel = 0;
        private int mTileWidth = 62;
        private int mTileHeight = 49;
        private int buttons = 0;
        private Bitmap enemyBitmap = new Bitmap("enemies & alcheims.PNG");
        private Bitmap myBitmap = new Bitmap("underground_tiles_small.PNG");
        private Rectangle cloneRect = new Rectangle(0, 0, 62, 49);
        private Bitmap cloneBitmap;
        private List<Screen> screens;

        public FileManager()
        {
        }

        public FileManager(List<Screen> screen, int numButtons)
        {
            screens = screen;
            buttons = numButtons;
        }

        public void ImportTextFile(string fileName)
        {
            this.mLevel = 0;
            int x = 0;
            int y = 30;
            int importCounter = 0;
            int count = 0;
            bool enemy = false;
            StreamReader wordFile;
            string wordLine;

            try
            {
                wordFile = new StreamReader(fileName);
                if (wordFile != null)
                {
                    wordLine = wordFile.ReadLine();
                    while (wordLine != null)
                    {
                        int total = wordList.Count();
                        wordList[wordListCount] = wordLine;
                        wordListCount++;
                        string[] mFileContents = wordLine.Split(new Char[] { ',' });

                        for (int a = 0; a < 14; a++)
                        {
                            int converted = int.Parse(mFileContents[a]);
                            if (converted < 80)
                            {
                                enemy = false;
                            }
                            else
                            {
                                enemy = true;
                                converted -= 80;
                            }

                            if (converted < buttons && converted != 4)
                            {
                                cloneRect = new Rectangle(mTileWidth * converted, mTileHeight * 0, mTileWidth, mTileHeight);
                                addTile(x, y, mFileContents[a], importCounter, enemy);
                            }
                            else if (converted >= buttons && converted < (buttons * 2))
                            {
                                converted -= (buttons * 1);
                                cloneRect = new Rectangle(mTileWidth * converted, mTileHeight * 1, mTileWidth, mTileHeight);
                                addTile(x, y, mFileContents[a], importCounter, enemy);
                            }
                            else if (converted >= (buttons * 2) && converted < (buttons * 3))
                            {
                                converted -= (buttons * 2);
                                cloneRect = new Rectangle(mTileWidth * converted, mTileHeight * 2, mTileWidth, mTileHeight);
                                addTile(x, y, mFileContents[a], importCounter, enemy);
                            }
                            else if (converted >= (buttons * 3) && converted < (buttons * 4))
                            {
                                converted -= (buttons * 3);
                                cloneRect = new Rectangle(mTileWidth * converted, mTileHeight * 3, mTileWidth, mTileHeight);
                                addTile(x, y, mFileContents[a], importCounter, enemy);
                            }

                            if (a == 0)
                            {
                                x = 0;
                            }

                            x += mTileWidth;
                            importCounter++;
                        }
                        y += mTileHeight;

                        if (importCounter == 140)
                        {
                            mLevel++;
                            x = 0;
                            y = 30;
                            importCounter = 0;
                        }

                        wordLine = wordFile.ReadLine();
                        count++;
                    }
                    wordFile.Close();
                    mLevel = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The following error occured while attempting to read the file: " + e.Message);
                wordListCount = 0;
            }
        }

        private void addTile(int x, int y, string ascii, int count, bool enemy)
        {
            if (!enemy)
            {
                cloneBitmap = myBitmap.Clone(cloneRect, myBitmap.PixelFormat);
            }
            else
            {
                cloneBitmap = enemyBitmap.Clone(cloneRect, myBitmap.PixelFormat);
            }
            screens[mLevel].Tiles[count].BitmapTile = cloneBitmap;
            screens[mLevel].Tiles[count].XStart = x;
            screens[mLevel].Tiles[count].YStart = y;
            screens[mLevel].Tiles[count].AsciiCode = ascii;
        }

        public void ExportTextFile(string fileName)
        {
            int count = 0;
            StreamWriter outputFile = null;
            string[] lineToOutput = new string[1];

            try
            {
                outputFile = new StreamWriter(fileName, false);
                if (outputFile != null)
                {
                    foreach (Screen screen in this.screens)
                    {
                        int temp = screen.Tiles.Count();
                        for (int i = 0; i < temp; i += 14)
                        {
                            lineToOutput[0] = screen.GetDataAsString(i) + "//" + count.ToString();
                            outputFile.WriteLine(lineToOutput[0]);
                        }
                        count++;
                    }
                    outputFile.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occurred while saving file: " + e.ToString());
            }
            finally
            {
                if (outputFile != null)
                {
                    outputFile.Close();
                    outputFile.Dispose();
                }
            }
        }
    }
}
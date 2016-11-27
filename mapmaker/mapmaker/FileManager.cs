using Newtonsoft.Json;
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
            int row = 0;

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
                                getRect(converted.ToString());
                                addTile(x, y, mFileContents[a], importCounter, row, enemy);
                            }
                            else if (converted >= buttons && converted < (buttons * 2))
                            {
                                converted -= (buttons * 1);
                                getRect(converted.ToString());
                                addTile(x, y, mFileContents[a], importCounter, row, enemy);
                            }
                            else if (converted >= (buttons * 2) && converted < (buttons * 3))
                            {
                                converted -= (buttons * 2);
                                getRect(converted.ToString());
                                addTile(x, y, mFileContents[a], importCounter, row, enemy);
                            }
                            else if (converted >= (buttons * 3) && converted < (buttons * 4))
                            {
                                converted -= (buttons * 3);
                                getRect(converted.ToString());
                                addTile(x, y, mFileContents[a], importCounter, row, enemy);
                            }

                            if (a == 0)
                            {
                                x = 0;
                            }

                            x += mTileWidth;
                            importCounter++;
                        }
                        y += mTileHeight;
                        row++;
                        importCounter = 0;

                        if (row == 10)
                        {
                            mLevel++;
                            x = 0;
                            y = 30;
                            importCounter = 0;
                            row = 0;
                        }

                        wordLine = wordFile.ReadLine();
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

        public void ImportJSONFile(string fileName)
        {
            StreamReader wordFile;

            try
            {
                wordFile = new StreamReader(fileName);
                if (wordFile != null)
                {
                    // set deserializer to replace default items that are created by the 
                    // constructors, otherwise you will end up with duplicates
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ObjectCreationHandling = ObjectCreationHandling.Replace;

                    string json = wordFile.ReadToEnd();
                    screens = JsonConvert.DeserializeObject<List<Screen>>(json, settings);

                    int screenNum = 0;

                    
                    // draw the tiles
                    //foreach (Screen screen in this.screens)
                    for (int i = 0; i < 1; i++)
                    {
                        Debug.Print(screenNum.ToString());
                        screenNum += 1;

                        foreach (Row row in screens[i].Rows)
                        {
                            foreach (Tile tile in row.Tiles)
                            {
                                // ignore empty tiles
                                if (tile.AsciiCode != "4")
                                {
                                    getRect(tile.AsciiCode);

                                    //Debug.Print("x: " + cloneRect.X.ToString() + " y: " + cloneRect.Y.ToString() +
                                    //            " width: " + cloneRect.Width.ToString() +
                                    //            " height: " + cloneRect.Height.ToString());

                                    Debug.Print(tile.XStart.ToString() + " " + tile.YStart.ToString());

                                    if (tile.Type == "enemy")
                                    {
                                        cloneBitmap = enemyBitmap.Clone(cloneRect, myBitmap.PixelFormat);
                                    }
                                    else
                                    {
                                        cloneBitmap = myBitmap.Clone(cloneRect, myBitmap.PixelFormat);
                                    }

                                    tile.BitmapTile = cloneBitmap;
                                    //cloneBitmap.Dispose();
                                    //cloneBitmap = null;
                                }
                            }
                        }
                    }

                    wordFile.Close();
                    //wordFile.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.Print("The following error occured while attempting to read the file: " + e.Message);
            }
        }

        private void getRect(string asciiCode)
        {
            int converted = int.Parse(asciiCode);
            if (converted > 80)
            {
                converted -= 80;
            }

            if (converted < buttons && converted != 4)
            {
                cloneRect.X = mTileWidth * converted;
                cloneRect.Y = 0;

            }
            else if (converted >= buttons && converted < (buttons * 2))
            {
                converted -= (buttons * 1);
                cloneRect.X = mTileWidth * converted;
                cloneRect.Y = mTileHeight;
            }
            else if (converted >= (buttons * 2) && converted < (buttons * 3))
            {
                converted -= (buttons * 2);
                cloneRect.X = mTileWidth * converted;
                cloneRect.Y = mTileHeight * 2;
            }
            else if (converted >= (buttons * 3) && converted < (buttons * 4))
            {
                converted -= (buttons * 3);
                cloneRect.X = mTileWidth * converted;
                cloneRect.Y = mTileHeight * 3;
            }
        }

        private void addTile(int x, int y, string ascii, int count, int row, bool enemy)
        {
            if (!enemy)
            {
                cloneBitmap = myBitmap.Clone(cloneRect, myBitmap.PixelFormat);
            }
            else
            {
                cloneBitmap = enemyBitmap.Clone(cloneRect, myBitmap.PixelFormat);
            }
            screens[mLevel].Rows[row].Tiles[count].BitmapTile = cloneBitmap;
            screens[mLevel].Rows[row].Tiles[count].XStart = x;
            screens[mLevel].Rows[row].Tiles[count].YStart = y;
            screens[mLevel].Rows[row].Tiles[count].AsciiCode = ascii;
            screens[mLevel].Rows[row].Tiles[count].Type = enemy ? "enemy" : "platform";
        }

        public void ExportTextFile(string fileName)
        {
            StreamWriter outputFile = null;
            JsonSerializer jsonSerializer = new JsonSerializer();
            string outputTest = JsonConvert.SerializeObject(this.screens);

            try
            {
                outputFile = new StreamWriter(fileName, false);
                if (outputFile != null)
                {
                    outputFile.Write(outputTest);
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
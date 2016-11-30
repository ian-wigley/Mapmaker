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

        public List<Screen> ImportJSONFile(string fileName)
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
                    
                    // draw the tiles
                    foreach (Screen screen in this.screens)
                    {
                        foreach (Row row in screen.Rows)
                        {
                            foreach (Tile tile in row.Tiles)
                            {
                                // ignore empty tiles
                                if (tile.AsciiCode != "4")
                                {
                                    Rectangle rect = getRect(tile.AsciiCode);

                                    if (tile.Type == "enemy")
                                    {
                                        cloneBitmap = enemyBitmap.Clone(rect, myBitmap.PixelFormat);
                                    }
                                    else
                                    {
                                        cloneBitmap = myBitmap.Clone(rect, myBitmap.PixelFormat);
                                    }

                                    tile.BitmapTile = cloneBitmap;
                                }
                            }
                        }
                    }

                    wordFile.Close();
                    wordFile.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.Print("The following error occured while attempting to read the file: " + e.Message);
            }

            return this.screens;
        }

        private Rectangle getRect(string asciiCode)
        {
            int x = 0;
            int y = 0;
            
            int converted = int.Parse(asciiCode);
            if (converted > 80)
            {
                converted -= 80;
            }

            if (converted < buttons && converted != 4)
            {
                x = mTileWidth * converted;
                y = 0;

            }
            else if (converted >= buttons && converted < (buttons * 2))
            {
                converted -= (buttons * 1);
                x = mTileWidth * converted;
                y = mTileHeight;
            }
            else if (converted >= (buttons * 2) && converted < (buttons * 3))
            {
                converted -= (buttons * 2);
                x = mTileWidth * converted;
                y = mTileHeight * 2;
            }
            else if (converted >= (buttons * 3) && converted < (buttons * 4))
            {
                converted -= (buttons * 3);
                x = mTileWidth * converted;
                y = mTileHeight * 3;
            }

            return new Rectangle(x, y, mTileWidth, mTileHeight);
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
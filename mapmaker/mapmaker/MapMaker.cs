////////////////////////////////////////////////
//                                            //
// Nodes of Yesod Map Maker                   //
//                                            //
// Written by Ian Wigley Dec 2009 - Jul 2010  //
//                                            //
// Version 1.2                                //
//                                            //
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace mapmaker
{
    partial class MapMaker : Form
    {
        // The menu bar pinches 30 pixels from the top of the screen !!
        private int mMenuBarFudgeFactor = 30;
        private int mGridSquareNmber = 0;
        private int mRowNumber = 0;
        private int mLevel = 0;

        private int m_screenshotStartX;
        private int m_screenshotStartY;
        private int mWidth;
        private int mTileWidth = 62;
        private int mTileHeight = 49;
        private int mNoOfScreens = 256;

        private int screenDisplayX;
        private int screenDisplayY;
        private int noButtonsX;
        private int noButtonsY;

        private string asciiCode;

        private bool areMoving = false;
        private bool platforms = true;

        private Bitmap m_standardEnemyTiles = new Bitmap("enemies & alcheims.PNG");
        private Bitmap m_standardLevelTiles = new Bitmap("underground_tiles_small.PNG");
        private Bitmap m_smallLevelButtonTiles = new Bitmap("underground_tiles_small_screen.PNG");
        private Bitmap cloneScreenDisplay;
        private Bitmap cloneEnemyDisplay;
        private Bitmap cloneBitmap;
        private Bitmap grid = new Bitmap(808, 640);
        private Bitmap split1;
        private Bitmap split2;

        private Rectangle split1rect;
        private Rectangle split2rect;
        private Rectangle buttonRect;
        private Rectangle cloneRect = new Rectangle(0, 0, 62, 49);
        private Rectangle redraw = new Rectangle(62, 30, 1808, 900);

        private Point gridPos = new Point(0, 0);
        private Point enemyPos = new Point(0, 530);
        private Point offScreen = new Point(0, 700);
        private Point pos0 = new Point(0, 530);
        private Point pos1 = new Point(0, 610);

        private List<Screen> screens = new List<Screen>();

        private FileManager m_fileManager;

        private System.Windows.Forms.Button[] levelButtons;
        private System.Windows.Forms.Button[] enemyButtons;

        private List<Bitmap> m_levelButtons = new List<Bitmap>();
        private List<Bitmap> m_enemyButtons = new List<Bitmap>();

        public MapMaker()
        {
            noButtonsX = 19;
            noButtonsY = 4;

            // calculate the width of the remaining bitmap after the intial split 
            mWidth = m_standardLevelTiles.Width - (mTileWidth * 19);//15

            // split the original bitmap if it's larger than our screen !!
            if (m_standardLevelTiles.Width > mTileWidth * 19)//15
            {
                split1rect = new Rectangle(0, 0, mTileWidth * 19, mTileHeight);
                split1 = m_standardLevelTiles.Clone(split1rect, m_standardLevelTiles.PixelFormat);

                split2rect = new Rectangle(mTileWidth * 19, 0, mWidth, mTileHeight);
                split2 = m_standardLevelTiles.Clone(split2rect, m_standardLevelTiles.PixelFormat);
            }

            DrawGrid();

            // Calculate the Width & Height of each tile for buttons.
            screenDisplayX = (m_smallLevelButtonTiles.Width - 20) / noButtonsX;
            screenDisplayY = m_smallLevelButtonTiles.Height / noButtonsY;// 4;

            levelButtons = new Button[4 * 19];
            enemyButtons = new Button[12];

            int j = 0;
            int count = 0;
            int buttonYpos = 525;
            for (int h = 0; h < 4; h++)
            {
                for (int i = 0; i < 19; i++)
                {
                    buttonRect = new Rectangle((i * 43), j, screenDisplayX, screenDisplayY);
                    cloneScreenDisplay = m_smallLevelButtonTiles.Clone(buttonRect, m_smallLevelButtonTiles.PixelFormat);
                    levelButtons[count] = new System.Windows.Forms.Button();
                    levelButtons[count].Location = new System.Drawing.Point(i * (screenDisplayX + 8), buttonYpos);
                    levelButtons[count].Name = "button" + i;
                    levelButtons[count].Size = new System.Drawing.Size(screenDisplayX + 0, screenDisplayY + 4);
                    levelButtons[count].UseVisualStyleBackColor = true;
                    levelButtons[count].BackgroundImage = cloneScreenDisplay;
                    m_levelButtons.Add(cloneScreenDisplay);
                    levelButtons[count].Tag = count;
                    levelButtons[count].Click += new System.EventHandler(ClickHandler);
                    Controls.Add(levelButtons[count]);
                    count++;
                }
                j += screenDisplayY;
                buttonYpos += 40;
            }

            j = 0;
            buttonYpos = 0;
            count = 0;
            for (int i = 0; i < 12; i++)
            {
                buttonRect = new Rectangle((i * 43), j, screenDisplayX, screenDisplayY);
                cloneEnemyDisplay = m_standardEnemyTiles.Clone(buttonRect, m_standardEnemyTiles.PixelFormat);
                enemyButtons[count] = new System.Windows.Forms.Button();
                enemyButtons[count].Location = new System.Drawing.Point(i * (screenDisplayX + 8), buttonYpos);
                enemyButtons[count].Name = "button" + i;
                enemyButtons[count].Size = new System.Drawing.Size(screenDisplayX + 0, screenDisplayY + 4);
                enemyButtons[count].UseVisualStyleBackColor = true;
                enemyButtons[count].BackgroundImage = cloneEnemyDisplay;
                m_enemyButtons.Add(cloneEnemyDisplay);
                enemyButtons[count].Tag = count;
                enemyButtons[count].Click += new System.EventHandler(ClickHandler);
                count++;
            }

            InitializeComponent();

            this.radioButton1.Checked = true;

            for (int i = 0; i < mNoOfScreens; i++)
            {
                screens.Add(new Screen(i));
                this.comboBox1.Items.Add(i);
                this.comboBox1.Text = "0";
            }
            //this.SaveMenuItem.Enabled = false;
            m_fileManager = new FileManager(screens, noButtonsX);
        }

        private void MapMaker_MouseMove(object sender, MouseEventArgs e)
        {
            this.label3.Text = e.X.ToString();
            this.label4.Text = e.Y.ToString();
            m_screenshotStartX = e.X;
            m_screenshotStartY = e.Y;
            Invalidate(redraw);
        }

        private void MapMaker_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(grid, gridPos);

            foreach (Row r in screens[mLevel].Rows)
            {
                foreach (Tile t in r.Tiles)
                {
                    if (t.BitmapTile != null)
                    {
                        e.Graphics.DrawImage(t.BitmapTile, t.XStart, t.YStart);
                    }
                }
            }

            if (areMoving)
            {
                e.Graphics.DrawImage(cloneBitmap, m_screenshotStartX, m_screenshotStartY);
            }

            this.label8.Text = mLevel.ToString();
            this.label5.Text = mGridSquareNmber.ToString();
        }

        // Get our mouse x & y co-ord's if no tile is selected to be placed on the grid i.e. it's null
        //remove it from our list & from the screen.
        private void MapMaker_Down(object sender, MouseEventArgs e)
        {
            m_screenshotStartX = e.X;
            m_screenshotStartY = e.Y;
            if (cloneBitmap == null)
            {
                snapToGrid(e);
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].BitmapTile = null;
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].AsciiCode = "4"; // ".";
            }
        }

        private void MapMaker_MouseUp(object sender, MouseEventArgs e)
        {
            // check mouse x & y co-ords
            if (e.X < 62 || e.X > 804 ||
                e.Y < 30 || e.Y > 521)
            {
                cloneBitmap = null;
                areMoving = false;
            }

            if (e.Button == MouseButtons.Left && cloneBitmap != null)
            {
                snapToGrid(e);
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].BitmapTile = cloneBitmap;
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].XStart = m_screenshotStartX;
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].YStart = m_screenshotStartY;
                screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].AsciiCode = asciiCode;

                if(radioButton1.Checked)
                {
                    screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].Type = "platform";
                }
                else
                {
                    screens[mLevel].Rows[mRowNumber].Tiles[mGridSquareNmber].Type = "enemy";
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                cloneBitmap = null;
                areMoving = false;
            }
        }

        private void startMoving()
        {
            if (platforms)
            {
                cloneBitmap = m_standardLevelTiles.Clone(cloneRect, m_standardLevelTiles.PixelFormat);
            }
            else
            {
                cloneBitmap = m_standardEnemyTiles.Clone(cloneRect, m_standardLevelTiles.PixelFormat);
            }
            areMoving = true;
        }

        private int[] snapToGrid(MouseEventArgs e)
        {
            int counterX = 1;
            int counterY = 0;
            int[] position = new int[2];
            for (int i = mTileWidth; i < 13 * mTileWidth; i += mTileWidth)//12
            {
                for (int j = 0 + mMenuBarFudgeFactor; j < 10 * mTileHeight; j += mTileHeight)
                {
                    if ((e.X >= i && e.X < i + mTileWidth) && (e.Y >= j && e.Y < j + mTileHeight))
                    {
                        m_screenshotStartX = i;
                        m_screenshotStartY = j;
                        counterX = i / mTileWidth;
                        counterY = j / mTileHeight;

                        mRowNumber = counterY;
                        mGridSquareNmber = counterX;
                        position[0] = mRowNumber;
                        position[1] = mGridSquareNmber;

                        return position;
                    }
                }
            }
            return position;
        }

        // Draw the grid 13 squares wide x 10 squares high and convert it to a Bitmap
        private void DrawGrid()
        {
            // Horizontal Lines
            for (int X = mTileWidth; X < (13 * mTileWidth); X++)//12
            {
                for (int Y = 30; Y < (11 * mTileHeight); Y += mTileHeight)
                {
                    grid.SetPixel(X, Y, Color.White);
                }
            }
            // Vertical Lines
            for (int X = mTileWidth; X < (14 * mTileWidth); X += mTileWidth)
            {
                for (int Y = 30; Y < (10 * mTileHeight) + 30; Y++)
                {
                    grid.SetPixel(X, Y, Color.White);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Method to handle the event of a button being clicked in our List of Buttons
        private void ClickHandler(object sender, EventArgs e)
        {
            // Get the TAG number of the button pressed
            string tagger = ((System.Windows.Forms.Button)sender).Tag.ToString();

            int tagNumber;
            int nextLine = 0;
            tagNumber = int.Parse(tagger);

            if (platforms)
            {
                if (tagNumber < noButtonsX)
                {
                    nextLine = 0;
                }
                else if (tagNumber >= noButtonsX && tagNumber < (noButtonsX * 2))
                {
                    tagNumber -= (19 * 1);
                    nextLine += (mTileHeight * 1);
                }
                else if (tagNumber >= (noButtonsX * 2) && tagNumber < (noButtonsX * 3))
                {
                    tagNumber -= (19 * 2);
                    nextLine += (mTileHeight * 2);
                }
                else if (tagNumber >= (noButtonsX * 3) && tagNumber < (noButtonsX * 4))
                {
                    tagNumber -= (19 * 3);
                    nextLine += (mTileHeight * 3);
                }
                asciiCode = tagger;
                cloneRect = new Rectangle(mTileWidth * tagNumber, nextLine, mTileWidth, mTileHeight);
                startMoving();
            }
            else
            {
                // If the button pressed is less than the number of enemies then make
                // it addable to the map.
                if (tagNumber < 13)
                {
                    nextLine = 0;
                    asciiCode = 8 + tagger;
                    cloneRect = new Rectangle(mTileWidth * tagNumber, nextLine, mTileWidth, mTileHeight);
                    startMoving();
                }
            }
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
        }

        // open json file
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Map Maker";
            openFileDialog.InitialDirectory = @"*.*";
            openFileDialog.Filter = "All files (*.json)|*.json|All files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.ToLower().Contains("txt"))
                {
                    screens = m_fileManager.ImportTextFile(openFileDialog.FileName);
                }
                if (openFileDialog.FileName.ToLower().Contains("json"))
                {
                    screens = m_fileManager.ImportJSONFile(openFileDialog.FileName);
                }
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Map Maker";
            saveFileDialog.InitialDirectory = @"*.*";
            saveFileDialog.Filter = "All files (*.json)|*.json|All files (*.txt)|*.txt";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_fileManager.ExportTextFile(saveFileDialog.FileName);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (mLevel > 0)
            {
                mLevel--;
                this.label8.Text = mLevel.ToString();
                Invalidate();
            }
            if (mLevel == 0)
            {
                mLevel = mNoOfScreens - 1;
                this.label8.Text = mLevel.ToString();
                Invalidate();
            }
        }
        // Screens up
        private void button17_Click(object sender, EventArgs e)
        {
            if (mLevel < mNoOfScreens)
            {
                mLevel++;
                this.label8.Text = mLevel.ToString();
                Invalidate();
            }
            if (mLevel == mNoOfScreens)
            {
                mLevel = 0;
                this.label8.Text = mLevel.ToString();
                Invalidate();
            }
        }

        private void LevelButton_Click(object sender, EventArgs e)
        {
            bool castedLevel = int.TryParse(comboBox1.Text, out mLevel);
            this.label8.Text = mLevel.ToString();
            Invalidate();
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Do you want to close without saving ?";
            DialogResult result;
            result = MessageBox.Show(this, message, "Close", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                foreach (Screen scrn in screens)
                {
                    scrn.Rows.Clear();
                    scrn.InitialiseRows();
                }
                Invalidate();
                this.SaveMenuItem.Enabled = false;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();

        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            platforms = true;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].Visible = true;
            }

            Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            platforms = false;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i].Visible = false;
            }

            for (int i = 0; i < enemyButtons.Length; i++)
            {
                enemyButtons[i].Location = new System.Drawing.Point(i * (screenDisplayX + 8), 525);
                Controls.Add(enemyButtons[i]);
            }
            Invalidate();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

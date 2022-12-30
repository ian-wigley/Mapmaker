//////////////////////////////////////////////////////////////
//                                                          //
// Map Maker                                                //
//                                                          //
// Written by Ian Wigley Dec 2009 - Dec 2020                //
//                                                          //
// Version 1.4                                              //
//                                                          //
//////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace mapmaker
{
    public partial class MapMaker : Form
    {
        private int m_asciiCode;

        private int m_gridSquareNmber = 0;
        private int m_level = 0;

        private int m_screenshotStartX;
        private int m_screenshotStartY;

        private int m_tileWidth = 62;
        private int m_tileHeight = 49;
        private int m_noOfScreens = 256;

        private int m_screenDisplayX;
        private int m_screenDisplayY;

        private const int m_numButtonsX = 19;
        private const int m_numButtonsY = 4;
        private const int m_numberOfEnemyButtons = 9;
        private const int m_numberOfLevelButtons = 76;

        private bool m_areMoving = false;
        private bool m_platforms = true;

        private Bitmap m_standardEnemyTiles = new Bitmap("enemy_tiles.png");
        private Bitmap m_smallEnemyButtonTiles = new Bitmap("enemy_tiles_small.png");
        private Bitmap m_standardLevelTiles = new Bitmap("underground_tiles_small.png");
        private Bitmap m_smallLevelButtonTiles = new Bitmap("underground_tiles.png");
        private Bitmap m_cloneScreenDisplay;
        private Bitmap m_cloneEnemyDisplay;
        private Bitmap m_cloneBitmap;
        private Bitmap m_grid = new Bitmap(808, 640);

        private Rectangle m_buttonRect;
        private Rectangle m_cloneRect = new Rectangle(0, 0, 62, 49);
        private Rectangle m_redraw = new Rectangle(0, 30, 1808, 900);

        private Point m_gridPos = new Point(0, 0);

        private readonly FileManager m_fileManager;

        private readonly Button[] m_levelButton;
        private readonly Button[] m_enemyButton;

        private readonly List<Bitmap> m_levelButtons = new List<Bitmap>();
        private readonly List<Bitmap> m_enemyButtons = new List<Bitmap>();
        private readonly List<Screen> m_screens = new List<Screen>();
        readonly Random random = new Random();


        public MapMaker()
        {
            //m_numButtonsX = 19;
            //m_numButtonsY = 4;

            CreateBitmapGrid();

            // Calculate the Width & Height of each tile for buttons.
            m_screenDisplayX = (m_smallLevelButtonTiles.Width - 20) / m_numButtonsX;
            m_screenDisplayY = m_smallLevelButtonTiles.Height / m_numButtonsY;

            m_levelButton = new Button[m_numberOfLevelButtons];
            m_enemyButton = new Button[m_numberOfEnemyButtons];

            int j = 0;
            int count = 0;
            int buttonYpos = 525;
            for (int h = 0; h < m_numButtonsY; h++)
            {
                for (int i = 0; i < m_numButtonsX; i++)
                {
                    m_buttonRect = new Rectangle((i * 43), j, m_screenDisplayX, m_screenDisplayY);
                    m_cloneScreenDisplay = m_smallLevelButtonTiles.Clone(m_buttonRect, m_smallLevelButtonTiles.PixelFormat);
                    m_levelButton[count] = new Button
                    {
                        Location = new Point(i * (m_screenDisplayX + 8), buttonYpos),
                        Name = "button" + i,
                        Size = new Size(m_screenDisplayX + 0, m_screenDisplayY + 4),
                        UseVisualStyleBackColor = true,
                        BackgroundImage = m_cloneScreenDisplay
                    };
                    m_levelButtons.Add(m_cloneScreenDisplay);
                    m_levelButton[count].Tag = count;
                    m_levelButton[count].Click += new EventHandler(ClickHandler);
                    Controls.Add(m_levelButton[count]);
                    count++;
                }
                j += m_screenDisplayY;
                buttonYpos += 40;
            }

            j = 0;
            buttonYpos = 0;
            count = 0;
            for (int i = 0; i < m_numberOfEnemyButtons; i++)
            {
                m_buttonRect = new Rectangle((i * 43), j, m_screenDisplayX, m_screenDisplayY);
                m_cloneEnemyDisplay = m_smallEnemyButtonTiles.Clone(m_buttonRect, m_smallEnemyButtonTiles.PixelFormat);
                m_enemyButton[count] = new Button
                {
                    Location = new Point(i * (m_screenDisplayX + 8), buttonYpos),
                    Name = "button" + i,
                    Size = new Size(m_screenDisplayX + 0, m_screenDisplayY + 4),
                    UseVisualStyleBackColor = true,
                    BackgroundImage = m_cloneEnemyDisplay
                };
                m_enemyButtons.Add(m_cloneEnemyDisplay);
                m_enemyButton[count].Tag = count;
                m_enemyButton[count].Click += new EventHandler(ClickHandler);
                count++;
            }

            InitializeComponent();

            radioButton1.Checked = true;

            for (int i = 0; i < m_noOfScreens; i++)
            {
                m_screens.Add(new Screen());
                screenNumbers.Items.Add(i);
                screenNumbers.Text = "0";
            }
            // SaveMenuItem.Enabled = false;
            m_fileManager = new FileManager(m_screens, m_numButtonsX, m_standardEnemyTiles, m_standardLevelTiles);
        }

        private void MapMaker_MouseMove(object sender, MouseEventArgs e)
        {
            label3.Text = e.X.ToString();
            label4.Text = e.Y.ToString();
            m_screenshotStartX = e.X;
            m_screenshotStartY = e.Y;
            Invalidate(m_redraw);
        }

        private void MapMaker_Paint(object sender, PaintEventArgs e)
        {
            int tileCount = 0;
            e.Graphics.DrawImage(m_grid, m_gridPos);

            foreach (Tile t in m_screens[m_level].Tiles)
            {
                if (m_screens[m_level].Tiles[tileCount].BitmapTile != null)
                {
                    e.Graphics.DrawImage(t.BitmapTile, t.DX, t.DY);
                }
                if (tileCount == m_screens[m_level].Tiles.Count)
                {
                    tileCount = 0;
                }
                tileCount += 1;
            }

            if (m_areMoving)
            {
                e.Graphics.DrawImage(m_cloneBitmap, m_screenshotStartX, m_screenshotStartY);
            }

            label8.Text = m_level.ToString();
            label5.Text = m_gridSquareNmber.ToString();
        }

        // Get our mouse x & y co-ord's if no tile is selected to be placed on the grid i.e. it's null
        // remove it from our list & from the screen.
        private void MapMaker_Down(object sender, MouseEventArgs e)
        {
            m_screenshotStartX = e.X;
            m_screenshotStartY = e.Y;
            if (m_cloneBitmap == null)
            {
                SnapToGrid(e);
                m_screens[m_level].Tiles[m_gridSquareNmber].BitmapTile = null;
                m_screens[m_level].Tiles[m_gridSquareNmber].TileNumber = 4;
            }
        }

        private void MapMaker_MouseUp(object sender, MouseEventArgs e)
        {
            // check mouse x & y co-ords
            if (e.X < 0 || e.X > 804 || e.Y < 30 || e.Y > 521)
            {
                m_cloneBitmap = null;
                m_areMoving = false;
            }

            if (e.Button == MouseButtons.Left && m_cloneBitmap != null)
            {
                SnapToGrid(e);
                m_screens[m_level].Tiles[m_gridSquareNmber].BitmapTile = m_cloneBitmap;
                m_screens[m_level].Tiles[m_gridSquareNmber].DX = m_screenshotStartX;
                m_screens[m_level].Tiles[m_gridSquareNmber].DY = m_screenshotStartY;
                m_screens[m_level].Tiles[m_gridSquareNmber].TileNumber = m_asciiCode;
            }

            if (e.Button == MouseButtons.Right)
            {
                m_cloneBitmap = null;
                m_areMoving = false;
            }
        }

        private void StartMoving()
        {
            if (m_platforms)
            {
                m_cloneBitmap = m_standardLevelTiles.Clone(m_cloneRect, m_standardLevelTiles.PixelFormat);
            }
            else
            {
                m_cloneBitmap = m_standardEnemyTiles.Clone(m_cloneRect, m_standardEnemyTiles.PixelFormat);
            }
            m_areMoving = true;
        }

        private void SnapToGrid(MouseEventArgs e)
        {
            // The menu bar pinches 30 pixels from the top of the screen !!
            int m_menuBarFudgeFactor = 30;
            for (int i = 0; i < 13 * m_tileWidth; i += m_tileWidth)
            {
                for (int j = 0 + m_menuBarFudgeFactor; j < 10 * m_tileHeight; j += m_tileHeight)
                {
                    if ((e.X >= i && e.X < i + m_tileWidth) && (e.Y >= j && e.Y < j + m_tileHeight))
                    {
                        m_screenshotStartX = i;
                        m_screenshotStartY = j;
                        m_gridSquareNmber = (i / m_tileWidth) + ((j / m_tileHeight) * 14);
                    }
                }
            }
        }

        // Create a bitmap grid
        private void CreateBitmapGrid()
        {
            // Horizontal Lines
            for (int X = 0; X < (13 * m_tileWidth); X++)
            {
                for (int Y = 30; Y < (11 * m_tileHeight); Y += m_tileHeight)
                {
                    m_grid.SetPixel(X, Y, Color.White);
                }
            }
            // Vertical Lines
            for (int X = 0; X < (14 * m_tileWidth); X += m_tileWidth)
            {
                for (int Y = 30; Y < (10 * m_tileHeight) + 30; Y++)
                {
                    m_grid.SetPixel(X, Y, Color.White);
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
            string tagger = ((Button)sender).Tag.ToString();

            int tagNumber;
            int nextLine = 0;
            tagNumber = int.Parse(tagger);

            if (m_platforms)
            {
                if (tagNumber < m_numButtonsX)
                {
                    nextLine = 0;
                }
                else if (tagNumber >= m_numButtonsX && tagNumber < (m_numButtonsX * 2))
                {
                    tagNumber -= (19 * 1);
                    nextLine += (m_tileHeight * 1);
                }
                else if (tagNumber >= (m_numButtonsX * 2) && tagNumber < (m_numButtonsX * 3))
                {
                    tagNumber -= (19 * 2);
                    nextLine += (m_tileHeight * 2);
                }
                else if (tagNumber >= (m_numButtonsX * 3) && tagNumber < (m_numButtonsX * 4))
                {
                    tagNumber -= (19 * 3);
                    nextLine += (m_tileHeight * 3);
                }
                m_asciiCode = int.Parse(tagger);
                m_cloneRect = new Rectangle(m_tileWidth * tagNumber, nextLine, m_tileWidth, m_tileHeight);
                StartMoving();
            }
            else
            {
                // TODO check the below

                // If the button pressed is less than the number of enemies then make
                // it addable to the map.
                if (tagNumber < 9)
                {
                    nextLine = 0;
                    m_asciiCode = int.Parse(8 + tagger);
                    m_cloneRect = new Rectangle(m_tileWidth * tagNumber, nextLine, m_tileWidth, m_tileHeight);
                    StartMoving();
                }
            }
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            // Not currently implemented
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Map Maker",
                InitialDirectory = @"*.*",
                Filter = "All files (*.json)|*.json|All files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.ToLower().Contains("txt"))
                {
                    m_fileManager.ImportTextFile(openFileDialog.FileName);
                }
                if (openFileDialog.FileName.ToLower().Contains("json"))
                {
                    m_fileManager.ImportJSONFile(openFileDialog.FileName);
                }
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Map Maker",
                InitialDirectory = @"*.*",
                Filter = "All files (*.*)|*.*|Text (*.txt)|*.txt|Json (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog.FileName.ToLower().Contains(".txt"))
                {
                    m_fileManager.ExportTextFile(saveFileDialog.FileName);
                }
                else if (saveFileDialog.FileName.ToLower().Contains(".json"))
                {
                    m_fileManager.ExportJsonFile(saveFileDialog.FileName);
                }
            }
        }

        private void LevelButton_Click(object sender, EventArgs e)
        {
            m_level = int.Parse(screenNumbers.SelectedItem.ToString());
            label8.Text = m_level.ToString();
            Invalidate();
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this, "Do you want to close without saving ?", "Close", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                foreach (Screen scrn in m_screens)
                {
                    scrn.Tiles.Clear();
                    scrn.InitaliseTiles();
                }
                Invalidate();
                SaveMenuItem.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            m_platforms = true;

            for (int i = 0; i < m_levelButton.Length; i++)
            {
                m_levelButton[i].Visible = true;
            }

            Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            m_platforms = false;

            for (int i = 0; i < m_levelButton.Length; i++)
            {
                m_levelButton[i].Visible = false;
            }

            for (int i = 0; i < m_enemyButton.Length; i++)
            {
                m_enemyButton[i].Location = new Point(i * (m_screenDisplayX + 8), 525);
                Controls.Add(m_enemyButton[i]);
            }

            Invalidate();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LevelDown(object sender, EventArgs e)
        {
            //mLevel = (mLevel - 1) % 255;
            //label8.Text = mLevel.ToString();
            //Invalidate();

            if (m_level > 0)
            {
                m_level--;
                label8.Text = m_level.ToString();
                Invalidate();
                return;
            }
            if (m_level == 0)
            {
                m_level = m_noOfScreens - 1;
                label8.Text = m_level.ToString();
                Invalidate();
            }
        }

        private void LevelUp(object sender, EventArgs e)
        {
            m_level = (m_level + 1) % 255;
            label8.Text = m_level.ToString();
            Invalidate();
        }
    }
}

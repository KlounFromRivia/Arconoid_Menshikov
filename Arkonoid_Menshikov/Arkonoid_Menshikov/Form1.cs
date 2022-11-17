using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkonoid_Menshikov
{
    public partial class Form1 : Form
    {
        const int mapWidth = 20;//размер карты
        const int mapHeight = 30;

        int[,] map = new int[mapHeight, mapWidth];
        int dirX = 0; //полет шарика (мячик)
        int dirY = 0;
        int platformX; //платформа (палочка)
        int platformY;
        int ballX; //сам мячик
        int ballY;
        int score; //кол-во очков

        Bitmap arcanoidSet; //спрайты

        public Label scoreLabel;

        public Form1()
        {
            InitializeComponent();

            scoreLabel = new Label();
            scoreLabel.Location = new Point((mapWidth) * 20 + 1, 50);
            scoreLabel.Text = "Score: " + score;
            this.Controls.Add(scoreLabel);

            timer1.Tick += new EventHandler(update);
            this.KeyUp += new KeyEventHandler(inputCheck);

            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            MessageBox.Show("Управление:\nA и D-передвижение платформы влево и вправо\n" +
                "Escape-выход из игры", "Добро пожаловать в Arcanoid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            timer1.Enabled = true;
        }

        private void inputCheck(object sender, KeyEventArgs e) //управление платформой
        {
            map[platformY, platformX] = 0;
            map[platformY, platformX + 1] = 0;
            map[platformY, platformX + 2] = 0;
            switch (e.KeyCode)
            {
                case Keys.D:
                    if (platformX + 1 < mapWidth - 1)
                        platformX++;
                    break;
                case Keys.A:
                    if (platformX > 0)
                        platformX--;
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
            map[platformY, platformX] = 9;
            map[platformY, platformX + 1] = 99;
            map[platformY, platformX + 2] = 999;
        }

        public void AddLine()//добавление линии
        {
            for (int i = mapHeight - 2; i > 0; i--)
            {
                for (int j = 0; j < mapWidth; j += 2)
                {
                    map[i, j] = map[i - 1, j];
                }
            }
            Random r = new Random();
            for (int j = 0; j < mapWidth; j += 2)
            {
                int currPlatform = r.Next(1, 5);
                map[0, j] = currPlatform;
                map[0, j + 1] = currPlatform + currPlatform * 10;
            }
        }

        private void update(object sender, EventArgs e) //проверка вылета мячика через низ
        {
            if (ballY + dirY > mapHeight - 1)
            {
                Init();
                Application.Restart();
            }


            map[ballY, ballX] = 0;
            if (!IsCollide())
                ballX += dirX;
            if (!IsCollide())
                ballY += dirY;
            map[ballY, ballX] = 8;

            map[platformY, platformX] = 9;
            map[platformY, platformX + 1] = 99;
            map[platformY, platformX + 2] = 999;

            Invalidate();
        }

        public void GeneratePlatforms() //генерация панельки
        {
            Random r = new Random();
            for (int i = 0; i < mapHeight / 3; i++)
            {
                for (int j = 0; j < mapWidth; j += 2)
                {
                    int currPlatform = r.Next(1, 5);
                    map[i, j] = currPlatform;
                    map[i, j + 1] = currPlatform + currPlatform * 10;
                }
            }
        }

        public bool IsCollide() //определение нахождения мячика
        {
            bool isColliding = false;
            if (ballX + dirX > mapWidth - 1 || ballX + dirX < 0)
            {
                dirX *= -1;
                isColliding = true;
            }
            if (ballY + dirY < 0)
            {
                dirY *= -1;
                isColliding = true;
            }

            if (map[ballY + dirY, ballX] != 0)
            {
                bool addScore = false;
                isColliding = true;

                if (map[ballY + dirY, ballX] > 10 && map[ballY + dirY, ballX] < 99)
                {
                    map[ballY + dirY, ballX] = 0;
                    map[ballY + dirY, ballX - 1] = 0;
                    addScore = true;
                }
                else if (map[ballY + dirY, ballX] < 9)
                {
                    map[ballY + dirY, ballX] = 0;
                    map[ballY + dirY, ballX + 1] = 0;
                    addScore = true;
                }
                if (addScore)
                {
                    score += 50;
                    if (score % 200 == 0 && score > 0)
                    {
                        AddLine();
                    }
                }
                dirY *= -1;
            }
            if (map[ballY, ballX + dirX] != 0)
            {
                bool addScore = false;
                isColliding = true;

                if (map[ballY, ballX + dirX] > 10 && map[ballY + dirY, ballX] < 99)
                {
                    map[ballY, ballX + dirX] = 0;
                    map[ballY, ballX + dirX - 1] = 0;
                    addScore = true;
                }
                else if (map[ballY, ballX + dirX] < 9)
                {
                    map[ballY, ballX + dirX] = 0;
                    map[ballY, ballX + dirX + 1] = 0;
                    addScore = true;
                }
                if (addScore)
                {
                    score += 50;
                    if (score % 200 == 0 && score > 0)
                    {
                        AddLine();
                    }
                }
                dirX *= -1;
            }
            scoreLabel.Text = "Score: " + score;

            return isColliding;
        }

        public void DrawArea(Graphics g) //Вырисовывание стенок игры
        {
            g.DrawRectangle(Pens.Black, new Rectangle(0, 0, mapWidth * 20, mapHeight * 20));
        }

        public void Init() //инициализация
        {
            this.Width = (mapWidth + 5) * 20;
            this.Height = (mapHeight + 2) * 20;

            arcanoidSet = (Bitmap) Properties.Resources.arcanoid;
            timer1.Interval = 80;

            score = 0;

            scoreLabel.Text = "Score: " + score;

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = 0;
                }
            }

            platformX = (mapWidth - 1) / 2;
            platformY = mapHeight - 1;

            map[platformY, platformX] = 9;
            map[platformY, platformX + 1] = 99;
            map[platformY, platformX + 2] = 999;

            ballY = platformY - 1;
            ballX = platformX + 1;

            map[ballY, ballX] = 8;

            dirX = 1;
            dirY = -1;

            GeneratePlatforms();

            timer1.Start();
        }

        public void DrawMap(Graphics g) //вырисовка объектов
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (map[i, j] == 9)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(60, 20)), 398, 17, 150, 50, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 8)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(20, 20)), 806, 548, 73, 73, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 1)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(40, 20)), 20, 16, 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 2)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(40, 20)), 20, 16 + 77 * (map[i, j] - 1), 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 3)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(40, 20)), 20, 16 + 77 * (map[i, j] - 1), 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 4)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(40, 20)), 20, 16 + 77 * (map[i, j] - 1), 170, 59, GraphicsUnit.Pixel);
                    }
                    if (map[i, j] == 5)
                    {
                        g.DrawImage(arcanoidSet, new Rectangle(new Point(j * 20, i * 20), new Size(40, 20)), 20, 16 + 77 * (map[i, j] - 1), 170, 59, GraphicsUnit.Pixel);
                    }
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e) //вырисовка игры
        {
            DrawArea(e.Graphics);
            DrawMap(e.Graphics);
        }

    }
}

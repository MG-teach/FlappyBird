using FB1.Properties;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;

namespace FB1
{
    public partial class MainForm : Form
    {
        private bool gameStarted = false; // Показва дали играта е стартирана

        private int score = 0; // Система за точки ...

        private Image background;  // Фон
        private Image bird;        // Птица
        private Image pipeImage;   //Тръби

        private int backgroundX = 0;  // X-позиция на фона
        private int backgroundSpeed = 1; // Скорост на движение на фона

        private int birdX;  // X-позиция на птичката
        private int birdY;  // Y-позиция на птичката
        private int birdVelocity = 0; // Скорост по Y
        private int gravity = 1;  // Гравитация
        private int jumpSpeed = 10; // Сила на скока

        private int pipeWidth = 60; // Ширина на тръбите
        private int pipeGap = 150;  // Разстояние между тръбите
        private int pipeSpeed = 5;  // Скорост на тръбите

        private int[] pipeX = { 400, 650, 900 }; // X-позиции на тръбите
        private int[] pipeY = { 300, 200, 250 }; // Y-позиции на долните тръби


        public MainForm()
        {
            InitializeComponent();

            // Зареждане на изображенията от ресурси
            bird = Image.FromFile("C:\\Users\\Teacher\\source\\repos\\11th grade\\16 Projects OOP\\FlappyBird\\FB1\\Resources\\bird.png");
            background = Image.FromFile("C:\\Users\\Teacher\\source\\repos\\11th grade\\16 Projects OOP\\FlappyBird\\FB1\\Resources\\background .jpg");
            pipeImage = Image.FromFile(@"C:\Users\Teacher\source\repos\11th grade\16 Projects OOP\FlappyBird\FB1\Resources\longPipe.png");

            birdX = this.ClientSize.Width / 2;
            birdY = this.ClientSize.Height / 2;
            // Активиране на Double Buffering за плавно рисуване
            this.DoubleBuffered = true;

            // Задаване на размер на формата
            this.Size = new Size(1200, 800);

            // Центриране на формата на екрана
            this.StartPosition = FormStartPosition.CenterScreen;

            gameTimer.Stop();
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Актуализиране на позицията на фона
            backgroundX -= backgroundSpeed;

            // Рестартиране на фона, когато излезе напълно извън екрана
            if (backgroundX <= -this.ClientSize.Width)
            {
                backgroundX = 0;
            }

            // Движение на тръбите
            for (int i = 0; i < pipeX.Length; i++)
            {
                pipeX[i] -= pipeSpeed;

                // Рестартиране на тръбите, когато излязат извън екрана
                if (pipeX[i] < -pipeWidth)
                {
                    pipeX[i] = this.ClientSize.Width;
                    pipeY[i] = new Random().Next(50, this.ClientSize.Height - pipeGap - 50);

                    // Увеличаване на точките при преминаване на тръбата
                    score++;
                }

                // Проверка за сблъсък
                if (IsColliding(pipeX[i], pipeY[i]))
                {
                    GameOver();
                    return;
                }
            }

            // Актуализиране на позицията на птичката (гравитация)
            birdVelocity += gravity; // Увеличаване на скоростта от гравитацията
            birdY += birdVelocity;   // Промяна на позицията на птичката
            birdX = this.ClientSize.Width / 2;

            // Ограничаване на птичката в рамките на екрана
            if (birdY < 0) birdY = 0; // Не позволява излизане над горната граница
            if (birdY > this.ClientSize.Height - 50) birdY = this.ClientSize.Height - 50; // Не позволява излизане долу

            // Принуждаване на формата да се презарисува
            this.Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (!gameStarted)
            {
                // Начален екран
                string startText = "Press SPACE to Start";
                Font startFont = new Font("Arial", 24, FontStyle.Bold);
                SizeF textSize = g.MeasureString(startText, startFont);
                PointF textPosition = new PointF(
                    (this.ClientSize.Width - textSize.Width) / 2,
                    (this.ClientSize.Height - textSize.Height) / 2
                );

                g.DrawString(startText, startFont, Brushes.Black, textPosition);
            }
            else
            {
                // Рисуване на фона
                g.DrawImage(background, backgroundX, 0, this.ClientSize.Width, this.ClientSize.Height);
                g.DrawImage(background, backgroundX + this.ClientSize.Width, 0, this.ClientSize.Width, this.ClientSize.Height);

                // Рисуване на птичката
                g.DrawImage(bird, birdX, birdY, 50, 50);

                // Рисуване на тръбите
                for (int i = 0; i < pipeX.Length; i++)
                {
                    // Горна тръба
                    g.DrawImage(pipeImage, pipeX[i], 0, pipeWidth, pipeY[i]);

                    // Долна тръба
                    g.DrawImage(pipeImage, pipeX[i], pipeY[i] + pipeGap, pipeWidth, this.ClientSize.Height - pipeY[i] - pipeGap);
                }

                // Рисуване на точките
                string scoreText = $"Score: {score}";
                Font scoreFont = new Font("Arial", 32, FontStyle.Bold); // Голям шрифт
                g.DrawString(scoreText, scoreFont, Brushes.Red, 10, 10); // Позиция: горен ляв ъгъл
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Управление на птичката (подскачане)
            if (e.KeyCode == Keys.Space)
            {
                if (!gameStarted)
                {
                    gameStarted = true; // Стартиране на играта
                    gameTimer.Start();
                }
                else
                {
                    birdVelocity = -jumpSpeed; // Подскок нагоре
                }
            }
        }

        private bool IsColliding(int pipeX, int pipeY)
        {
            Rectangle birdRect = new Rectangle(birdX, birdY, 50, 50);
            Rectangle topPipe = new Rectangle(pipeX, 0, pipeWidth, pipeY);
            Rectangle bottomPipe = new Rectangle(pipeX, pipeY + pipeGap, pipeWidth, this.ClientSize.Height - pipeY - pipeGap);

            return birdRect.IntersectsWith(topPipe) || birdRect.IntersectsWith(bottomPipe);
        }

        private void GameOver()
        {
            gameTimer.Stop();
            // Показване на съобщение с опции за рестарт или изход
            DialogResult result = MessageBox.Show(
                "Game Over! Do you want to restart?",
                "Game Over",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                RestartGame(); // Рестартиране на играта
            }
            else
            {
                Application.Exit(); // Изход от играта
            }
        }

        private void RestartGame()
        {
            // Връщане на началните позиции на птичката
            birdY = 100;
            birdVelocity = 0;

            // Нулиране на точките
            score = 0;

            // Връщане на началните позиции на тръбите
            for (int i = 0; i < pipeX.Length; i++)
            {
                pipeX[i] = this.ClientSize.Width + i * 300; // Нови позиции
                pipeY[i] = new Random().Next(50, this.ClientSize.Height - pipeGap - 50);
            }

            // Стартиране на таймера
            gameTimer.Start();
        }

    }

}

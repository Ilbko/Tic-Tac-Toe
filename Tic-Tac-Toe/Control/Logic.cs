using System;
using System.Drawing;
using System.Windows.Forms;
using Tic_Tac_Toe.Model;

namespace Tic_Tac_Toe.Control
{
    public class Logic
    {
        //"Ссылка" на форму для изменения текста формы (текст формы отображает цвет следующего шага)
        private Form1 form;

        //Конструктор логики
        public Logic(Form1 form)
        {
            this.form = form;
            this.form.Text = "Red";

            //Расстояние между кнопками
            int widthPadding = this.form.ClientSize.Width / 30;
            int heightPadding = this.form.ClientSize.Height / 30;

            //Размер кнопок с учётом расстояния между ними
            Size buttonSize = new Size(this.form.ClientSize.Width / 3 - widthPadding, this.form.ClientSize.Height / 3 - heightPadding);

            //Цикл, устанавливающий положение и размер кнопок
            for (int i = 0; i < TicModel.buttons.Length; i++)
            {
                //Заполнение происходит по рядам ([0, 0] -> [0, 1] -> [0, 2] -> [1, 0]...)
                TicModel.buttons[i / 3, i % 3] = new Button();
                TicModel.buttons[i / 3, i % 3].Size = buttonSize;
                //Поскольку заполнение параметров происходит по рядам, то локация X будет изменяться в три раза чаще, чем локация Y. Поэтому формулу нужно перевернуть
                //(положение кнопок - инвертированная индексация кнопок)
                TicModel.buttons[i / 3, i % 3].Location = new Point(widthPadding + buttonSize.Width * (i % 3), heightPadding + buttonSize.Height * (i / 3));
                TicModel.buttons[i / 3, i % 3].Click += Logic_Click;
            }
        }

        //Метод "перерисовки" кнопок при изменении размера окна
        internal void Redraw(Size clientSize)
        {
            int widthPadding = clientSize.Width / 30;
            int heightPadding = clientSize.Height / 30;

            Size buttonSize = new Size(clientSize.Width / 3 - widthPadding, clientSize.Height / 3 - heightPadding);

            for (int i = 0; i < TicModel.buttons.Length; i++)
            {
                TicModel.buttons[i / 3, i % 3].Size = buttonSize;
                TicModel.buttons[i / 3, i % 3].Location = new Point(widthPadding + buttonSize.Width * (i % 3), heightPadding + buttonSize.Height * (i / 3));
            }
        }

        //Метод оглашения победителя
        private void WinnerAnnouncer(Color color)
        {
            MessageBox.Show($"{color.ToKnownColor()} won!");
            Environment.Exit(0);
        }

        //Событие нажатия на кнопку
        private void Logic_Click(object sender, EventArgs e)
        {
            //Если у кнопки стандартный цвет (она не была нажата ранее)
            if ((sender as Button).BackColor == SystemColors.Control)
            {
                //В зависимости от значения bool курсора устанавливается цвет кнопки
                if (TicModel.cursor)
                {
                    this.form.Text = "Red";
                    (sender as Button).BackColor = Color.Blue;
                }
                else
                {
                    this.form.Text = "Blue";
                    (sender as Button).BackColor = Color.Red;
                }

                //Значение курсора инвертируется и инкрементируется переменная, считающая шаги
                TicModel.cursor = !TicModel.cursor;
                TicModel.step++;
            }

            //Минимальное количество шагов, нужное для победы в Крестиках-Ноликах - 5, поэтому смысла в проверке на победителя раньше пяти шагов нет
            if (TicModel.step >= 5 && TicModel.step < 9)
            {
                GameLogic();
            }
            //Если по проходу девяти шагов победитель так и не был оглашён
            else if (TicModel.step >= 9)
            {
                //Ничья
                MessageBox.Show("Tie!");
                Environment.Exit(0);
            }
        }

        private void GameLogic()
        {
            //Переменная для проверки на победителя и цвет команды для проверки
            bool check = true;
            Color teamColor;

            #region Проверка по горизонтали
            for (int i = 0; i < TicModel.buttons.Length / 3; i++)
            {
                //Устанавливается цвет левой кнопки ряда
                teamColor = TicModel.buttons[i, 0].BackColor;
                //Если левая кнопка ряда не была нажата, очевидно, что этот ряд не сможет определить победителя, поэтому проверяется следующий ряд
                if (teamColor == SystemColors.Control)
                    continue;

                //Проверка выбранного ряда на победителя. Цикл начинается со второго элемента ряда, ибо цвет первого уже был определён
                for (int j = 1; j < TicModel.buttons.Length / 3; j++)
                {
                    //Если цвет элемента ряда не совпадает с первым элементом ряда,
                    if (TicModel.buttons[i, j].BackColor != teamColor)
                    {
                        //то в этом ряду никто не победил
                        check = false;
                        break;
                    }
                }

                //Если в ряде был определён победитель
                if (check)
                    WinnerAnnouncer(teamColor);
                //Иначе проверяется другой ряд или начинается проверка по столбцам
                else
                    check = true;
            }
            #endregion

            #region Проверка по вертикали
            //Всё то же самое, только с колонками
            for (int i = 0; i < TicModel.buttons.Length / 3; i++)
            {
                teamColor = TicModel.buttons[0, i].BackColor;
                if (teamColor == SystemColors.Control)
                    continue;

                for (int j = 1; j < TicModel.buttons.Length / 3; j++)
                {
                    if (TicModel.buttons[j, i].BackColor != teamColor)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    WinnerAnnouncer(teamColor);
                else
                    check = true;
            }
            #endregion

            //Поскольку диагоналей на поле игры только две, то цикл будет выглядеть по-другому и для каждой диагонали будет своя проверка (один свой цикл в отличии от двух)
            #region Проверка по диагонали \
            //Установка цвета верхней левой клетки
            teamColor = TicModel.buttons[0, 0].BackColor;
            for (int i = 0; i < TicModel.buttons.Length / 3; i++)
            {
                //Если верхняя левая клетка не была нажата, очевидно, что эта диагональ не сможет определить победителя
                if (teamColor == SystemColors.Control)
                {
                    check = false;
                    break;
                }

                //Если следующий элемент диагонали не совпадает с верхним левым, то победителя там нет
                if (TicModel.buttons[i, i].BackColor != teamColor)
                {
                    check = false;
                }
            }

            if (check)
                WinnerAnnouncer(teamColor);
            else
                check = true;
            #endregion

            #region Проверка по диагонали /
            //Всё то же самое, только для другой диагонали (сверху вниз)
            teamColor = TicModel.buttons[0, 2].BackColor;
            for (int i = 0; i < TicModel.buttons.Length / 3; i++)
            {
                if (teamColor == SystemColors.Control)
                {
                    check = false;
                    break;
                }

                if (TicModel.buttons[i, 2 - i].BackColor != teamColor)
                {
                    check = false;
                }
            }

            if (check)
                WinnerAnnouncer(teamColor);
            else
                check = true;
            #endregion
        }
    }
}

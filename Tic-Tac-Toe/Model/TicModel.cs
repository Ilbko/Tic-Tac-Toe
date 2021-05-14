using System.Windows.Forms;

namespace Tic_Tac_Toe.Model
{
    //Класс модели
    public static class TicModel
    {
        //Двухмерный массив кнопок (поле игры)
        public static Button[,] buttons = new Button[3, 3];
        //Курсор (красный или синий)
        public static bool cursor = false;
        //Номер шага
        public static int step = 0;
    }
}

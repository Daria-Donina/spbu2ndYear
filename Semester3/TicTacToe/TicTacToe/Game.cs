using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Game
    {
        public bool[][] Map { get; set; }

        public Game()
        {
            Map = new bool[3, 3] { };
        }

        private void Move()
        {

        }

        private bool IsGameOver()
        {
            //Проверка строк
            for (int i = 0; i < 3; ++i)
            {
                if (Map[i][0] == Map[i][1] && Map[i][1] == Map[i][2])
                {
                    return true;
                }
            }

            //Проверка столбцов
            for (int j = 0; j < 3; ++j)
            {
                if (Map[0][j] == Map[1][j] && Map[1][j] == Map[2][j])
                {
                    return true;
                }
            }

            return false;
        }
    }
}

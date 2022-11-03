using System;
using System.Collections.Generic;

int rX = 8, rY = 8, cM = 8;
Model model = new Model(rX,rY, cM);
while (model.IsAlive())
{
    if (model.IsAllHide())
    {
        model.DoRandomMove();
    }
    else
    {
        model.DoMove();
    }
    model.ShowModel(false);
    Console.Read();
}
enum CellType
{
    Number,
    Mine
}
struct Cell
{
    public CellType type;
    public int value;
    public bool isHide, isFlag;
}
class Model
{
    private List<(int, int)> coords;
    private Cell[,] table;
    private int cMines;
    public Model(int tableX, int tableY, int mines)
    {
        coords = new List<(int, int)>();
        if (mines <= tableX*tableY)
        {
            cMines = mines;
        }
        else
        {
            cMines = tableX*tableY;
        }
        table = new Cell[tableX,tableY];
        for (int i = 0; i < tableX; i++)
        {
            for (int j = 0; j < tableY; j++)
            {
                table[i,j].isHide = true;
                table[i, j].isFlag = false;
                table[i,j].type = CellType.Number;
                table[i,j].value = 0;
            }
        }
        GenerateMines();
        ShowModel(true);
    }
    public void ShowModel(bool isFull)
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (table[i, j].isHide && !isFull)
                {
                    Console.Write("E");
                }
                else if (table[i,j].type is CellType.Number)
                {
                    Console.Write(table[i, j].value);
                }
                else if (table[i, j].type is CellType.Mine)
                {
                    Console.Write("X");
                }
                else if (table[i,j].isFlag)
                {
                    Console.Write("F");
                }

            }
            Console.Write('\n');
        }
        Console.Write('\n');
    }
    public void GenerateMines()
    {
        int count = 0;
        while (count < cMines)
        {
            int x = new Random().Next(0, table.GetLength(0));
            int y = new Random().Next(0, table.GetLength(1));
            if (table[x,y].type is not CellType.Mine)
            {
                count++;
                table[x, y].type = CellType.Mine;
                if (x == 0 || x == table.GetLength(0)-1)
                {
                    int temp = x == 0 ? 1 : -1;
                    if (y == 0)
                    {
                        table[x + 1 * temp, y].value += 1;
                        table[x + 1 * temp, y + 1].value += 1;
                        table[x, y + 1].value += 1;
                    }
                    else if (y == table.GetLength(1) - 1)
                    {
                        table[x, y - 1].value += 1;
                        table[x + 1 * temp, y - 1].value += 1;
                        table[x + 1 * temp, y].value += 1;
                    }
                    else
                    {
                        table[x, y - 1].value += 1;
                        table[x, y + 1].value += 1;
                        table[x + 1 * temp, y - 1].value += 1;
                        table[x + 1 * temp, y].value += 1;
                        table[x + 1 * temp, y + 1].value += 1;
                    }
                }
                else
                {
                    if (y == 0 || y == table.GetLength(1)-1)
                    {
                        int temp = y == 0 ? 1 : -1;
                        table[x, y + 1 * temp].value += 1;
                        table[x - 1, y].value += 1;
                        table[x - 1, y + 1 * temp].value += 1;
                        table[x + 1, y].value += 1;
                        table[x + 1, y + 1 * temp].value += 1;
                    }
                    else
                    {
                        table[x, y+1].value += 1;
                        table[x, y-1].value += 1;
                        table[x + 1, y - 1].value += 1;
                        table[x + 1, y].value += 1;
                        table[x + 1, y + 1].value += 1;
                        table[x - 1, y - 1].value += 1;
                        table[x - 1, y].value += 1;
                        table[x - 1, y + 1].value += 1;
                    }
                }
            }
        }
    }
    public bool IsAllHide()
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (!table[i, j].isHide) return false;
            }
        }
        return true;
    }
    public void DoRandomMove()
    {
        int x, y;
        do
        {
            x = new Random().Next(0, table.GetLength(0));
            y = new Random().Next(0, table.GetLength(1));
        }
        while (!table[x, y].isHide);
        CheckZero(x, y);
        table[x, y].isHide = false;
    }
    public bool IsAlive()
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (!table[i,j].isHide && table[i,j].type is CellType.Mine)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void DoMove()
    {
        int minCount = 100;
        for (int x = 0; x < table.GetLength(0); x++)
        {
            for (int y = 0; y < table.GetLength(1); y++)
            {
                if (!table[x, y].isHide)
                {
                    if (x == 0 || x == table.GetLength(0) - 1)
                    {
                        int temp = x == 0 ? 1 : -1;
                        int mini = 0;
                        if (y == 0)
                        {
                            if (table[x + 1 * temp, y].isHide && !table[x + 1 * temp, y].isFlag) mini++;
                            table[x + 1 * temp, y + 1].value += 1;
                            table[x, y + 1].value += 1;
                        }
                        else if (y == table.GetLength(1) - 1)
                        {
                            table[x, y - 1].value += 1;
                            table[x + 1 * temp, y - 1].value += 1;
                            table[x + 1 * temp, y].value += 1;
                        }
                        else
                        {
                            table[x, y - 1].value += 1;
                            table[x, y + 1].value += 1;
                            table[x + 1 * temp, y - 1].value += 1;
                            table[x + 1 * temp, y].value += 1;
                            table[x + 1 * temp, y + 1].value += 1;
                        }
                    }
                    else
                    {
                        if (y == 0 || y == table.GetLength(1) - 1)
                        {
                            int temp = y == 0 ? 1 : -1;
                            table[x, y + 1 * temp].value += 1;
                            table[x - 1, y].value += 1;
                            table[x - 1, y + 1 * temp].value += 1;
                            table[x + 1, y].value += 1;
                            table[x + 1, y + 1 * temp].value += 1;
                        }
                        else
                        {
                            table[x, y + 1].value += 1;
                            table[x, y - 1].value += 1;
                            table[x + 1, y - 1].value += 1;
                            table[x + 1, y].value += 1;
                            table[x + 1, y + 1].value += 1;
                            table[x - 1, y - 1].value += 1;
                            table[x - 1, y].value += 1;
                            table[x - 1, y + 1].value += 1;
                        }
                    }
                }
            }
        }
    }
    private void CheckZero(int x, int y)
    {
        if (x >= table.GetLength(0) || x < 0 || y >= table.GetLength(1) || y < 0) return;
        if (table[x, y].value == 0 && table[x, y].isHide)
        {
            table[x, y].isHide = false;
            CheckZero(x + 1, y);
            CheckZero(x + 1, y + 1);
            CheckZero(x + 1, y - 1);
            CheckZero(x, y + 1);
            CheckZero(x, y - 1);
            CheckZero(x - 1, y + 1);
            CheckZero(x - 1, y);
            CheckZero(x - 1, y - 1);
            ShowNearZero(x, y);
        }
    }
    private void ShowNearZero(int x, int y)
    {
        if (x >= table.GetLength(0) || x < 0 || y >= table.GetLength(1) || y < 0) return;
        if (x == 0 || x == table.GetLength(0) - 1)
        {
            int temp = x == 0 ? 1 : -1;
            if (y == 0)
            {
                table[x + 1 * temp, y].isHide = false;
                table[x + 1 * temp, y + 1].isHide = false;
                table[x, y + 1].isHide = false;
            }
            else if (y == table.GetLength(1) - 1)
            {
                table[x, y - 1].isHide = false;
                table[x + 1 * temp, y - 1].isHide = false;
                table[x + 1 * temp, y].isHide = false;
            }
            else
            {
                table[x, y - 1].isHide = false;
                table[x, y + 1].isHide = false;
                table[x + 1 * temp, y - 1].isHide = false;
                table[x + 1 * temp, y].isHide = false;
                table[x + 1 * temp, y + 1].isHide = false;
            }
        }
        else
        {
            if (y == 0 || y == table.GetLength(1) - 1)
            {
                int temp = y == 0 ? 1 : -1;
                table[x, y + 1 * temp].isHide = false;
                table[x - 1, y].isHide = false;
                table[x - 1, y + 1 * temp].isHide = false;
                table[x + 1, y].isHide = false;
                table[x + 1, y + 1 * temp].isHide = false;
            }
            else
            {
                table[x, y + 1].isHide = false;
                table[x, y - 1].isHide = false;
                table[x + 1, y - 1].isHide = false;
                table[x + 1, y].isHide = false;
                table[x + 1, y + 1].isHide = false;
                table[x - 1, y - 1].isHide = false;
                table[x - 1, y].isHide = false;
                table[x - 1, y + 1].isHide = false;
            }
        }
    }
}



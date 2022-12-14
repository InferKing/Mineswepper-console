using System;
using System.Collections.Generic;

int rX = 9, rY = 9, cM = 10, countFailure = 0, razm = 1000;
for (int i = 0; i < razm; i++)
{
    Model model = new Model(rX, rY, cM);
    while (model.IsAlive() == 0)
    {
        if (model.IsAllHide())
        {
            model.DoRandomMove();
        }
        else
        {
            model.DoMove();
        }
    }
    if (model.IsAlive() == -1) countFailure += 1;
}

Console.WriteLine($"Процент выигрыша = {(razm - (float)countFailure) / razm * 100}");

enum CellType
{
    Number,
    Mine
}
struct CellInfo
{
    public int flags;
    public int hides;
}
struct Cell
{
    public CellType type;
    public CellInfo info;
    public int value;
    public bool isHide, isFlag;
}
class Model
{
    private Cell[,] table;
    private int cMines;
    public Model(int tableX, int tableY, int mines)
    {
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
                table[i,j].info.hides = 0;
                table[i, j].info.flags = 0;
                table[i,j].type = CellType.Number;
                table[i,j].value = 0;
            }
        }
        GenerateMines();
    }
    public void ShowModel(bool isFull)
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (table[i, j].isFlag)
                {
                    Console.Write("F");
                }
                else if (table[i, j].isHide && !isFull)
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
                List<(int,int)> s = GetCellsCoordNear(x,y);
                foreach (var c in s)
                {
                    table[c.Item1, c.Item2].value += 1;
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
        while (!table[x, y].isHide && table[x,y].isFlag);
        CheckZero(x, y);
        table[x, y].isHide = false;
    }
    public int IsAlive()
    {
        int c = 0, c1 = 0;
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (table[i,j].type is CellType.Number && !table[i,j].isHide)
                {
                    c1 += 1;
                }
                if (table[i,j].type is CellType.Mine && table[i,j].isFlag)
                {
                    c += 1;
                }
                if (table[i,j].type is CellType.Mine && !table[i,j].isHide)
                {
                    return -1;
                }
            }
        }
        if (c != cMines || c1 != table.GetLength(0) * table.GetLength(1) - cMines)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
    public void DoMove()
    {
        CalcInfoCell();
        bool noMove = false;
        List<(int, int)> coords = new List<(int, int)>();
        if (!HasMove())
        {
            DoRandomMove();
            CalcInfoCell();
            return;
        }
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                // Console.WriteLine($"{i},{j} hides = {table[i, j].info.hides}, flags = {table[i, j].info.flags}");
                if (table[i,j].info.hides == (table[i,j].value - table[i,j].info.flags) && !table[i,j].isHide)
                {
                    coords = GetCellsCoordNear(i, j);
                    foreach (var it in coords)
                    {
                        if (table[it.Item1, it.Item2].isHide && !table[it.Item1, it.Item2].isFlag)
                        {
                            table[it.Item1, it.Item2].isFlag = true;
                            noMove = true;
                        }
                    }
                    CalcInfoCell();
                }
                if (table[i,j].value - table[i,j].info.flags == 0)
                {
                    coords = GetCellsCoordNear(i, j);
                    foreach (var it in coords)
                    {
                        if (table[it.Item1, it.Item2].isHide && !table[it.Item1, it.Item2].isFlag)
                        {
                            noMove = true;
                            table[it.Item1, it.Item2].isHide = false;
                        }
                    }
                    CalcInfoCell();
                }
            }
        }
        if (!noMove)
        {
            DoRandomMove();
            CalcInfoCell();
        }
    }
    private void CalcInfoCell()
    {
        for (int i = 0; i < table.GetLength(0);i++)
        {
            for (int j = 0; j < table.GetLength(1);j++)
            {
                List<(int, int)> s = new List<(int, int)>();
                s = GetCellsCoordNear(i, j);
                int q1 = 0, q2 = 0;
                foreach (var c in s)
                {
                    if (table[c.Item1,c.Item2].isHide && !table[c.Item1, c.Item2].isFlag)
                    {
                        q1 += 1;
                    }
                    else if (table[c.Item1,c.Item2].isFlag)
                    {
                        q2 += 1;
                    }
                }
                table[i, j].info.hides = q1;
                table[i, j].info.flags = q2;
            }
        }
    }
    private List<(int,int)> GetCellsCoordNear(int x, int y)
    {
        List<(int, int)> s = new List<(int, int)>(); 
        if (x == 0 || x == table.GetLength(0) - 1)
        {
            int temp = x == 0 ? 1 : -1;
            if (y == 0)
            {
                s.Add((x + 1 * temp, y));
                s.Add((x + 1 * temp, y + 1));
                s.Add((x, y + 1));
            }
            else if (y == table.GetLength(1) - 1)
            {
                s.Add((x, y - 1));
                s.Add((x + 1 * temp, y - 1));
                s.Add((x + 1 * temp, y));
            }
            else
            {
                s.Add((x, y - 1));
                s.Add((x, y + 1));
                s.Add((x + 1 * temp, y - 1));
                s.Add((x + 1 * temp, y));
                s.Add((x + 1 * temp, y + 1));
            }
        }
        else
        {
            if (y == 0 || y == table.GetLength(1) - 1)
            {
                int temp = y == 0 ? 1 : -1;
                s.Add((x, y + 1 * temp));
                s.Add((x - 1, y));
                s.Add((x - 1, y + 1 * temp));
                s.Add((x + 1, y));
                s.Add((x + 1, y + 1 * temp));
            }
            else
            {
                s.Add((x, y + 1));
                s.Add((x, y - 1));
                s.Add((x + 1, y - 1));
                s.Add((x + 1, y));
                s.Add((x + 1, y + 1));
                s.Add((x - 1, y - 1));
                s.Add((x - 1, y));
                s.Add((x - 1, y + 1));
            }
        }
        return s;
    }
    private void CheckZero(int x, int y)
    {
        if (x >= table.GetLength(0) || x < 0 || y >= table.GetLength(1) || y < 0) return;
        if (table[x, y].value == 0 && table[x, y].isHide && table[x,y].type is CellType.Number)
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
        List<(int, int)> s = GetCellsCoordNear(x, y);
        foreach (var c in s)
        {
            table[c.Item1, c.Item2].isHide = false;
        }
    }
    private bool HasMove() // use for cases when can't get info about mines
    {
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                if (table[i, j].info.hides != 0) return true;
            }
        }
        return false;
    }
}



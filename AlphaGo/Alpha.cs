using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace STARK_INDUSTRIES_斯塔克工业_1.Alpha
{
    /*
     *  1.用户下棋
     *  2.用户下棋结束：调整棋盘
     *  3.全盘更新DP1
     *  4.选择LV
     *  5.计算机下棋
     *  6.计算机下棋结束：调整棋盘
     */
    public class Alpha
    {
        /*
         *  [内存区]
         */

        /// <summary>
        /// 当前位置下了之后与附近的黑或白棋子组成的串的最长长度
        /// </summary>
        private int[,] Dp1 { get; }//白
        private int[,] Dp2 { get; }//黑
        /// <summary>
        /// 棋盘中每个棋子的状态
        /// </summary>
        private Color[,] B { get; }//false:白色   true:黑色
        /// 长度
        private int Length { get; }
        /// <summary>
        /// A N：当前DP1的各个LV
        /// </summary>
        private List<int[]> A6 { get; set; }
        private List<int[]> A5 { get; set; }
        private List<int[]> A4 { get; set; }
        private List<int[]> A3 { get; set; }
        private List<int[]> A2 { get; set; }
        private List<int[]> A1 { get; set; }
        /// <summary>
        /// B N：当前DP2的各个LV
        /// </summary>
        private List<int[]> B6 { get; set; }
        private List<int[]> B5 { get; set; }
        private List<int[]> B4 { get; set; }
        private List<int[]> B3 { get; set; }
        private List<int[]> B2 { get; set; }
        private List<int[]> B1 { get; set; }

        public Alpha(int n)
        {
            Length = n;
            Dp1 = new int[n, n];
            Dp2 = new int[n, n];

            A1 = new List<int[]>();
            A2 = new List<int[]>();
            A3 = new List<int[]>();
            A4 = new List<int[]>();
            A5 = new List<int[]>();
            A6 = new List<int[]>();

            B1 = new List<int[]>();
            B2 = new List<int[]>();
            B3 = new List<int[]>();
            B4 = new List<int[]>();
            B5 = new List<int[]>();
            B6 = new List<int[]>();

            B = new Color[n, n];
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    Dp1[i, j] = 1;
                    Dp2[i, j] = 1;
                    B[i, j] = new Color();
                    A1.Add(new[] { i, j, 2 });
                    B1.Add(new[] { i, j, 2 });
                }
            }
        }
        /*
         * [控制器区]
         */

        /// <summary>
        /// 下棋结束
        /// </summary>
        public bool? Finish(int row, int col, Color color)
        {
            /*
             *  1.修改棋盘
             *  2.更新下棋位置附近串中棋子的串长度
             *  3.DP赋0【因为该位置不会再被选到】
             *  4.移除LV【因为该位置不会被选到】
             */
            if (B[row, col] != Color.Null) return null;
            switch (color)
            {
                case Color.White:
                    B[row, col] = Color.White;
                    break;
                case Color.Black:
                    B[row, col] = Color.Black;
                    break;
                case Color.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
            RemoveLv(row, col, Color.White);
            RemoveLv(row, col, Color.Black);
            Dp1[row, col] = 0;
            Dp2[row, col] = 0;
            return CheckWin(row, col, color);
        }
        /*
         *  [CPU区]
         */

        /// <summary>
        /// 更新全棋盘DP1或DP2
        /// </summary>
        public void UpdateDp(Color color)
        {
            /*
             *  1.取出相应LV中老的DP
             *  2.更新DP
             *  3.将更新之后的DP装入LV
             */
            for (var i = 0; i < Length; i++)
            {
                for (var j = 0; j < Length; j++)
                {
                    if (B[i, j] != Color.Null) continue;
                    RemoveLv(i, j, color);
                    AddLv(i, j, color, UpdateDpImp(i, j, color));
                }
            }
        }
        /// <summary>
        /// 更新指定坐标的DP1或DP2
        /// </summary>
        public void UpdateDp(int row, int col, Color color)
        {
            RemoveLv(row, col, color);
            AddLv(row, col, color, UpdateDpImp(row, col, color));
        }
        /// <summary>
        /// 取出相应LV
        /// </summary>
        private void RemoveLv(int i, int j, Color color)
        {
            /*
             *  1.找到某DP的位置的棋子位于相应LV中的位置，将其移除
             */
            switch (color)
            {
                case Color.White:
                    switch (Dp1[i, j])
                    {
                        case 1: A1.Remove(A1.Find(k => k[0] == i && k[1] == j)); break;
                        case 2: A2.Remove(A2.Find(k => k[0] == i && k[1] == j)); break;
                        case 3: A3.Remove(A3.Find(k => k[0] == i && k[1] == j)); break;
                        case 4: A4.Remove(A4.Find(k => k[0] == i && k[1] == j)); break;
                        case 5: A5.Remove(A5.Find(k => k[0] == i && k[1] == j)); break;
                        case 6: A6.Remove(A6.Find(k => k[0] == i && k[1] == j)); break;
                    }
                    break;
                case Color.Black:
                    switch (Dp2[i, j])
                    {
                        case 1: B1.Remove(B1.Find(k => k[0] == i && k[1] == j)); break;
                        case 2: B2.Remove(B2.Find(k => k[0] == i && k[1] == j)); break;
                        case 3: B3.Remove(B3.Find(k => k[0] == i && k[1] == j)); break;
                        case 4: B4.Remove(B4.Find(k => k[0] == i && k[1] == j)); break;
                        case 5: B5.Remove(B5.Find(k => k[0] == i && k[1] == j)); break;
                        case 6: B6.Remove(B6.Find(k => k[0] == i && k[1] == j)); break;
                    }
                    break;
                case Color.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
        /// <summary>
        /// 压入相应LV
        /// </summary>
        private void AddLv(int i, int j, Color color,int maxC)
        {
            /*
             *  键更新之后的DP添加到LV中
             */
            var pos = new[] { i, j, maxC };
            switch (color)
            {
                case Color.White:
                    switch (Dp1[i, j])
                    {
                        case 1: A1.Add(pos); A1 = A1.OrderBy(k => k[2]).ToList(); break;
                        case 2: A2.Add(pos); A2 = A2.OrderBy(k => k[2]).ToList(); break;
                        case 3: A3.Add(pos); A3 = A3.OrderBy(k => k[2]).ToList(); break;
                        case 4: A4.Add(pos); A4 = A4.OrderBy(k => k[2]).ToList(); break;
                        case 5: A5.Add(pos); A5 = A5.OrderBy(k => k[2]).ToList(); break;
                        case 6: A6.Add(pos); A6 = A6.OrderBy(k => k[2]).ToList(); break;
                    }
                    break;
                case Color.Black:
                    switch (Dp2[i, j])
                    {
                        case 1: B1.Add(pos); B1 = B1.OrderBy(k => k[2]).ToList(); break;
                        case 2: B2.Add(pos); B2 = B2.OrderBy(k => k[2]).ToList(); break;
                        case 3: B3.Add(pos); B3 = B3.OrderBy(k => k[2]).ToList(); break;
                        case 4: B4.Add(pos); B4 = B4.OrderBy(k => k[2]).ToList(); break;
                        case 5: B5.Add(pos); B5 = B5.OrderBy(k => k[2]).ToList(); break;
                        case 6: B6.Add(pos); B6 = B6.OrderBy(k => k[2]).ToList(); break;
                    }
                    break;
                case Color.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
        private int UpdateDpImp(int row, int col, Color color)
        {
            /*
             *  1.上下检测相应颜色棋子数量
             *  2.左右检测相应颜色棋子数量
             *  3.左上到右下检测相应颜色棋子数量
             *  4.右上到坐下检测相应颜色棋子数量
             *  
             *  Tip1：每次检测之后更新max，并初始化num = 0
             *  Tip2：检测下棋位置四周串的两个端点是否都没有棋子挡住【Color为Null】
             */

            //1.检查上下串个数，和端点空值个数：C：端点空位个数
            int[] T = new int[2], b = new int[2], l = new int[2], r = new int[2], tl = new int[2], tr = new int[2], bl = new int[2], br = new int[2];
            T[0] = Search(row, col, color, Orientation.Top, out T[1]);
            b[0] = Search(row, col, color, Orientation.Bottom, out b[1]);
            l[0] = Search(row, col, color, Orientation.Left, out l[1]);
            r[0] = Search(row, col, color, Orientation.Right, out r[1]);
            tl[0] = Search(row, col, color, Orientation.TopLeft, out tl[1]);
            tr[0] = Search(row, col, color, Orientation.TopRight, out tr[1]);
            bl[0] = Search(row, col, color, Orientation.BottomLeft, out bl[1]);
            br[0] = Search(row, col, color, Orientation.BottomRight, out br[1]);
            return CheckCore(T, b, l, r, tl, tr, bl, br, row, col, color);
        }
        private int CheckCore(IReadOnlyList<int> T, IReadOnlyList<int> b, IReadOnlyList<int> l, IReadOnlyList<int> r, IReadOnlyList<int> tl, IReadOnlyList<int> tr, IReadOnlyList<int> bl, IReadOnlyList<int> br, int row, int col, Color color)
        {
            /*
             *  1.检查5位
             *  2.检查4位 End >= 2*1
             *  3.检查3位 End >= 2*2
             *  4.检查4位 End < 2
             *  5.检查3位 End < 2
             *  
             *  TIP：1.End：成为高位的下棋口
             */
            int n, c = 0;
            if (T[0] + b[0] == 5 || l[0] + r[0] == 5 || tl[0] + br[0] == 5 || tr[0] + bl[0] == 5)
            {
                n = 6;
            }
            else if (T[0] + b[0] == 4 || l[0] + r[0] == 4 || tl[0] + br[0] == 4 || tr[0] + bl[0] == 4)
            {
                n = 5;
            }
            else if ((T[0] + b[0] == 3 && T[1] + b[1] == 2) || (l[0] + r[0] == 3 && l[1] + r[1] == 2) || (tl[0] + br[0] == 3 && tl[1] + br[1] == 2) || (tr[0] + bl[0] == 3 && tr[1] + bl[1] == 2))
            {
                n = 4;
                c = 100;//单数
            }
            else if (((T[0] + b[0] == 2 && T[1] + b[1] == 2 ? 2 : 0) + (l[0] + r[0] == 2 && l[1] + r[1] == 2 ? 2 : 0) + (tl[0] + br[0] == 2 && tl[1] + br[1] == 2 ? 2 : 0) + (tr[0] + bl[0] == 2 && tr[1] + bl[1] == 2 ? 2 : 0) >= 4))
            {
                n = 3;
                c = 100;//对数
            }
            else if ((T[0] + b[0] == 3 && T[1] + b[1] == 2) || (l[0] + r[0] == 3 && l[1] + r[1] == 1) || (tl[0] + br[0] == 3 && tl[1] + br[1] == 1) || (tr[0] + bl[0] == 3 && tr[1] + bl[1] == 1))
            {
                n = 4;
                c = 50;//单数
            }
           
            else if (((T[0] + b[0] == 2 && T[1] + b[1] == 2 ? 2 : 0) + (l[0] + r[0] == 2 && l[1] + r[1] == 2 ? 2 : 0) + (tl[0] + br[0] == 2 && tl[1] + br[1] == 2 ? 2 : 0) + (tr[0] + bl[0] == 2 && tr[1] + bl[1] == 2 ? 2 : 0) >= 2))
            {
                n = 3;
                c = 50;//对数
            }
            else
            {
                n = Math.Max(Math.Max(T[0] + b[0], l[0] + r[0]), Math.Max(tl[0] + br[0], tr[0] + bl[0])) + 1;
                c = (T[0] + b[0] < n ? T[1] + b[1] : 0) + (l[0] + r[0] < n ? T[1] + b[1] : 0) + (tl[0] + br[0] < n ? tl[1] + tr[1] : 0) + (tr[0] + bl[0] < n ? tr[1] + bl[1] : 0); 
            }
            switch (color)
            {
                case Color.White: Dp1[row, col] = n; return c;
                case Color.Black: Dp2[row, col] = n; return c;
                case Color.Null:
                default: return -1;
            }
        }
        public enum Orientation
        {
            Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight
        }
        //private int Search(int row, int col, Color color, Orientation path, out int c)
        //{
        //    /*
        //     *  1.扫描对应方向的棋子个数及其出口个数
        //     */
        //    int i, j, n = 0;
        //    c = 0;
        //    switch (path)
        //    {
        //        case Orientation.Top:
        //            for (i = row - 1; i >= 0 && B[i, col] == color; i--) n++;
        //            if (i >= 0 && B[i, col] == Color.Null && i - 1 >= 0 && B[i - 1, col] == color) { n++; i--; }
        //            if (i >= 0 && B[i, col] == Color.Null) c = 1; break;
        //        case Orientation.Bottom:
        //            for (i = row + 1; i < Length && B[i, col] == color; i++) n++;
        //            if (i < Length && B[i, col] == Color.Null && i + 1 < Length && B[i + 1, col] == color) { n++; i++; }
        //            if (i < Length && B[i, col] == Color.Null) c = 1; break;
        //        case Orientation.Left:
        //            for (i = col - 1; i >= 0 && B[row, i] == color; i--) n++;
        //            if (i >= 0 && B[row, i] == Color.Null && i - 1 >= 0 && B[row, i - 1] == color) { n++; i--; }
        //            if (i >= 0 && B[row, i] == Color.Null) c = 1; break;
        //        case Orientation.Right:
        //            for (i = col + 1; i < Length && B[row, i] == color; i++) n++;
        //            if (i < Length && B[row, i] == Color.Null && i + 1 < Length && B[row, i + 1] == color) { n++; i++; }
        //            if (i < Length && B[row, i] == Color.Null) c = 1; break;
        //        case Orientation.TopLeft:
        //            for (i = row - 1, j = col - 1; i >= 0 && j >= 0 && B[i, j] == color; i--, j--) n++;
        //            if (i >= 0 && j >= 0 && B[i, j] == Color.Null && i - 1 >= 0 && j - 1 >= 0 && B[i - 1, j - 1] == color) { n++; i--; j--; }
        //            if (i >= 0 && j >= 0 && B[i, j] == Color.Null) c = 1; break;
        //        case Orientation.TopRight:
        //            for (i = row - 1, j = col + 1; i >= 0 && j < Length && B[i, j] == color; i--, j++) n++;
        //            if (i >= 0 && j < Length && B[i, j] == Color.Null && i - 1 >= 0 && j + 1 < Length && B[i - 1, j + 1] == color) { n++; i--; j++; }
        //            if (i >= 0 && j < Length && B[i, j] == Color.Null) c = 1; break;
        //        case Orientation.BottomLeft:
        //            for (i = row + 1, j = col - 1; i < Length && j >= 0 && B[i, j] == color; i++, j--) n++;
        //            if (i < Length && j >= 0 && B[i, j] == Color.Null && i + 1 < Length && j - 1 >= 0 && B[i + 1, j - 1] == color) { n++; i++; j--; }
        //            if (i < Length && j >= 0 && B[i, j] == Color.Null) c = 1; break;
        //        case Orientation.BottomRight:
        //            for (i = row + 1, j = col + 1; i < Length && j < Length && B[i, j] == color; i++, j++) n++;
        //            if (i < Length && j < Length && B[i, j] == Color.Null && i + 1 < Length && j + 1 < Length && B[i + 1, j + 1] == color) { n++; i++; j++; }
        //            if (i < Length && j < Length && B[i, j] == Color.Null) c = 1; break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(path), path, null);
        //    }
        //    return n;
        //}
        private int Search(int row, int col, Color color, Orientation path, out int c)
        {
            /*
             *  1.扫描对应方向的棋子个数及其出口个数
             */
            int i, j, n = 0;
            c = 0;
            switch (path)
            {
                case Orientation.Top:
                    for (i = row - 1; i >= 0 && B[i, col] == color; i--) n++;
                    if (i >= 0 && B[i, col] == Color.Null) c = 1; break;
                case Orientation.Bottom:
                    for (i = row + 1; i < Length && B[i, col] == color; i++) n++;
                    if (i < Length && B[i, col] == Color.Null) c = 1; break;
                case Orientation.Left:
                    for (i = col - 1; i >= 0 && B[row, i] == color; i--) n++;
                    if (i >= 0 && B[row, i] == Color.Null) c = 1; break;
                case Orientation.Right:
                    for (i = col + 1; i < Length && B[row, i] == color; i++) n++;
                    if (i < Length && B[row, i] == Color.Null) c = 1; break;
                case Orientation.TopLeft:
                    for (i = row - 1, j = col - 1; i >= 0 && j >= 0 && B[i, j] == color; i--, j--) n++;
                    if (i >= 0 && j >= 0 && B[i, j] == Color.Null) c = 1; break;
                case Orientation.TopRight:
                    for (i = row - 1, j = col + 1; i >= 0 && j < Length && B[i, j] == color; i--, j++) n++;
                    if (i >= 0 && j < Length && B[i, j] == Color.Null) c = 1; break;
                case Orientation.BottomLeft:
                    for (i = row + 1, j = col - 1; i < Length && j >= 0 && B[i, j] == color; i++, j--) n++;
                    if (i < Length && j >= 0 && B[i, j] == Color.Null) c = 1; break;
                case Orientation.BottomRight:
                    for (i = row + 1, j = col + 1; i < Length && j < Length && B[i, j] == color; i++, j++) n++;
                    if (i < Length && j < Length && B[i, j] == Color.Null) c = 1; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(path), path, null);
            }
            return n;
        }
        /// <summary>
        /// 检查每个LV，选择下一个下棋位置
        /// </summary>
        /// <returns></returns>
        public int[] Choose()
        {
            /*
             *  1.在LV队列中选取下一个优先下的位置
             */
            int[] res;
            if (B6.Count > 0) { res = B6.Last(); B6.RemoveAt(B6.Count - 1); }
            else if (A6.Count > 0) { res = A6.Last(); A6.RemoveAt(A6.Count - 1); }

            else if (B5.Count > 0) { res = B5.Last(); B5.RemoveAt(B5.Count - 1); }
            else if (A5.Count > 0) { res = A5.Last(); A5.RemoveAt(A5.Count - 1); }

            else if (B4.Count > 0 && B4.Last()[2] == 100) { res = B4.Last(); B4.RemoveAt(B4.Count - 1); }
            else if (A4.Count > 0 && A4.Last()[2] == 100) { res = A4.Last(); A4.RemoveAt(A4.Count - 1); }

            else if (B3.Count > 0 && B3.Last()[2] == 100) { res = B3.Last(); B3.RemoveAt(B3.Count - 1); }
            else if (A3.Count > 0 && A3.Last()[2] == 100) { res = A3.Last(); A3.RemoveAt(A3.Count - 1); }

            else if (B4.Count > 0 && B4.Last()[2] == 50) { res = B4.Last(); B4.RemoveAt(B4.Count - 1); }
            else if (B3.Count > 0 && B3.Last()[2] == 50) { res = B3.Last(); B3.RemoveAt(B3.Count - 1); }

            else if (A4.Count > 0 && A4.Last()[2] == 50) { res = A4.Last(); A4.RemoveAt(A4.Count - 1); }

            else if (A3.Count > 0 && A3.Last()[2] == 50) { res = A3.Last(); A3.RemoveAt(A3.Count - 1); }

            else if (B4.Count > 0) { res = B4.Last(); B4.RemoveAt(B4.Count - 1); }
            else if (A4.Count > 0) { res = A4.Last(); A4.RemoveAt(A4.Count - 1); }

            else if (B3.Count > 0) { res = B3.Last(); B3.RemoveAt(B3.Count - 1); }
            else if (A3.Count > 0) { res = A3.Last(); A3.RemoveAt(A3.Count - 1); }

            else if (B2.Count > 0) { res = B2.Last(); B2.RemoveAt(B2.Count - 1); }
            else { res = B1.Last(); B1.RemoveAt(B1.Count - 1); }
            return res;
        }
        private bool CheckWin(int row, int col, Color color)
        {
            /*
             *  1.搜索全部方向上有没有大于等于5个子的串
             */
            return Search(row, col, color, Orientation.Top, out _) + Search(row, col, color, Orientation.Bottom, out _) >= 4 ||
                   Search(row, col, color, Orientation.Left, out _) + Search(row, col, color, Orientation.Right, out _) >= 4 ||
                   Search(row, col, color, Orientation.TopLeft, out _) + Search(row, col, color, Orientation.BottomRight, out _) >= 4 ||
                   Search(row, col, color, Orientation.TopRight, out _) + Search(row, col, color, Orientation.BottomLeft, out _) >= 4;
        }
        /*
         *  [IO区]
         */
        public void ConsoleB()
        {
            for (var i = 0; i < Length; i++)
            {
                for (var j = 0; j < Length; j++)
                {
                    Write($"{B[i, j]}     ");
                }
                WriteLine();
            }
            WriteLine();
        }

        public void ConsoleDp1()
        {
            for (var i = 0; i < Length; i++)
            {
                for (var j = 0; j < Length; j++)
                {
                    Write($"{Dp1[i, j]}\t");
                }
                WriteLine();
            }
            WriteLine();
        }

        public void ConsoleDp2()
        {
            for (var i = 0; i < Length; i++)
            {
                for (var j = 0; j < Length; j++)
                {
                    Write($"{Dp2[i, j]}\t");
                }
                WriteLine();
            }
            WriteLine();
        }

        public void ConsoleA543()
        {
            WriteLine();
            WriteLine("LV5：");
            foreach (var item in A5)
            {
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV4：");
            foreach (var item in A4)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV3：");
            foreach (var item in A3)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV2：");
            foreach (var item in A2)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV1：");
            foreach (var item in A1)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
        }

        public void ConsoleB543()
        {
            WriteLine("LV5：");
            foreach (var item in B5)
            {
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }                                            
            WriteLine();
            WriteLine("LV4：");
            foreach (var item in B4)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }                                            
            WriteLine();
            WriteLine("LV3：");
            foreach (var item in B3)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV2：");
            foreach (var item in B2)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }
            WriteLine();
            WriteLine("LV1：");
            foreach (var item in B1)                     
            {                                            
                Write($"{item[0]},{item[1]},{item[2]}    ");
            }                                            
            WriteLine();
        }

        public enum Color
        {
            Null, Black, White//Null：空位置    Black：黑棋    White：白棋
        }
    }
}

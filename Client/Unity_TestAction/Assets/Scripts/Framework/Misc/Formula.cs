using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosBaseFramework
{
    public class Formula
    {

        /// <summary>
        /// 操作数类型
        /// </summary>
        public enum OperandType
        {
            /// <summary>
            /// 函数
            /// </summary>
            FUNC = 1,

            /// <summary>
            /// 日期
            /// </summary>
            DATE = 2,

            /// <summary>
            /// 数字
            /// </summary>
            NUMBER = 3,

            /// <summary>
            /// 布尔
            /// </summary>
            BOOLEAN = 4,

            /// <summary>
            /// 字符串
            /// </summary>
            STRING = 5

        }

        /// <summary>
        /// 运算符类型(从上到下优先级依次递减)，数值越大，优先级越低
        /// </summary>
        public enum OperatorType
        {
            /// <summary>
            /// 左括号:(,left bracket
            /// </summary>
            LB = 10,

            /// <summary>
            /// 右括号),right bracket
            /// </summary>
            RB = 11,

            /// <summary>
            /// 逻辑非,!,NOT
            /// </summary>
            NOT = 20,

            /// <summary>
            /// 正号,+,positive sign
            /// </summary>
            PS = 21,

            /// <summary>
            /// 负号,-,negative sign
            /// </summary>
            NS = 22,

            /// <summary>
            /// 正切，tan
            /// </summary>
            TAN = 23,
            /// <summary>
            /// 反正切，atan
            /// </summary>
            ATAN = 24,


            /// <summary>
            /// 乘,*,multiplication
            /// </summary>
            MUL = 30,

            /// <summary>
            /// 除,/,division
            /// </summary>
            DIV = 31,

            /// <summary>
            /// 余,%,modulus
            /// </summary>
            MOD = 32,

            /// <summary>
            /// 加,+,Addition
            /// </summary>
            ADD = 40,

            /// <summary>
            /// 减,-,subtraction
            /// </summary>
            SUB = 41,

            /// <summary>
            /// 小于,less than
            /// </summary>
            LT = 50,

            /// <summary>
            /// 小于或等于,less than or equal to
            /// </summary>
            LE = 51,

            /// <summary>
            /// 大于,>,greater than
            /// </summary>
            GT = 52,

            /// <summary>
            /// 大于或等于,>=,greater than or equal to
            /// </summary>
            GE = 53,

            /// <summary>
            /// 等于,=,equal to
            /// </summary>
            ET = 60,

            /// <summary>
            /// 不等于,unequal to
            /// </summary>
            UT = 61,

            /// <summary>
            /// 逻辑与,&,AND
            /// </summary>
            AND = 70,

            /// <summary>
            /// 逻辑或,|,OR
            /// </summary>
            OR = 71,

            /// <summary>
            /// 逗号,comma
            /// </summary>
            CA = 80,

            /// <summary>
            /// 结束符号 @
            /// </summary>
            END = 255,

            /// <summary>
            /// 错误符号
            /// </summary>
            ERR = 256

        }

        public class Operand
        {
            #region Constructed Function
            public Operand(OperandType type, object value)
            {
                this.Type = type;
                this.Value = value;
            }

            public Operand(string opd, object value)
            {
                this.Type = ConvertOperand(opd);
                this.Value = value;
            }
            #endregion

            #region Variable &　Property
            /// <summary>
            /// 操作数类型
            /// </summary>
            public OperandType Type { get; set; }

            /// <summary>
            /// 关键字
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 操作数值
            /// </summary>
            public object Value { get; set; }

            #endregion

            #region Public Method
            /// <summary>
            /// 转换操作数到指定的类型
            /// </summary>
            /// <param name="opd">操作数</param>
            /// <returns>返回对应的操作数类型</returns>
            public static OperandType ConvertOperand(string opd)
            {
                if (opd.IndexOf("(") > -1)
                {
                    return OperandType.FUNC;
                }
                else if (IsNumber(opd))
                {
                    return OperandType.NUMBER;
                }
                else if (IsDate(opd))
                {
                    return OperandType.DATE;
                }
                else
                {
                    return OperandType.STRING;
                }
            }

            /// <summary>
            /// 判断对象是否为数字
            /// </summary>
            /// <param name="value">对象值</param>
            /// <returns>是返回真,否返回假</returns>
            public static bool IsNumber(object value)
            {
                double val;
                return double.TryParse(value.ToString(), out val);
            }

            /// <summary>
            /// 判断对象是否为日期
            /// </summary>
            /// <param name="value">对象值</param>
            /// <returns>是返回真,否返回假</returns>
            public static bool IsDate(object value)
            {
                DateTime dt;
                return DateTime.TryParse(value.ToString(), out dt);
            }
            #endregion
        }

        public class Operator
        {
            public Operator(OperatorType type, string value)
            {
                this.Type = type;
                this.Value = value;
            }

            /// <summary>
            /// 运算符类型
            /// </summary>
            public OperatorType Type { get; set; }

            /// <summary>
            /// 运算符值
            /// </summary>
            public string Value { get; set; }


            /// <summary>
            /// 对于>或者&lt;运算符，判断实际是否为>=,&lt;&gt;、&lt;=，并调整当前运算符位置
            /// </summary>
            /// <param name="currentOpt">当前运算符</param>
            /// <param name="currentExp">当前表达式</param>
            /// <param name="currentOptPos">当前运算符位置</param>
            /// <param name="adjustOptPos">调整后运算符位置</param>
            /// <returns>返回调整后的运算符</returns>
            public static string AdjustOperator(string currentOpt, string currentExp, ref int currentOptPos)
            {
                switch (currentOpt)
                {
                    case "<":
                        if (currentExp.Substring(currentOptPos, 2) == "<=")
                        {
                            currentOptPos++;
                            return "<=";
                        }
                        if (currentExp.Substring(currentOptPos, 2) == "<>")
                        {
                            currentOptPos++;
                            return "<>";
                        }
                        return "<";

                    case ">":
                        if (currentExp.Substring(currentOptPos, 2) == ">=")
                        {
                            currentOptPos++;
                            return ">=";
                        }
                        return ">";
                    case "t":
                        if (currentExp.Substring(currentOptPos, 3) == "tan")
                        {
                            currentOptPos += 2;
                            return "tan";
                        }
                        return "error";
                    case "a":
                        if (currentExp.Substring(currentOptPos, 4) == "atan")
                        {
                            currentOptPos += 3;
                            return "atan";
                        }
                        return "error";
                    default:
                        return currentOpt;
                }
            }

            /// <summary>
            /// 转换运算符到指定的类型
            /// </summary>
            /// <param name="opt">运算符</param>
            /// <param name="isBinaryOperator">是否为二元运算符</param>
            /// <returns>返回指定的运算符类型</returns>
            public static OperatorType ConvertOperator(string opt, bool isBinaryOperator)
            {
                switch (opt)
                {
                    case "!": return OperatorType.NOT;
                    case "+": return isBinaryOperator ? OperatorType.ADD : OperatorType.PS;
                    case "-": return isBinaryOperator ? OperatorType.SUB : OperatorType.NS;
                    case "*": return isBinaryOperator ? OperatorType.MUL : OperatorType.ERR;
                    case "/": return isBinaryOperator ? OperatorType.DIV : OperatorType.ERR;
                    case "%": return isBinaryOperator ? OperatorType.MOD : OperatorType.ERR;
                    case "<": return isBinaryOperator ? OperatorType.LT : OperatorType.ERR;
                    case ">": return isBinaryOperator ? OperatorType.GT : OperatorType.ERR;
                    case "<=": return isBinaryOperator ? OperatorType.LE : OperatorType.ERR;
                    case ">=": return isBinaryOperator ? OperatorType.GE : OperatorType.ERR;
                    case "<>": return isBinaryOperator ? OperatorType.UT : OperatorType.ERR;
                    case "=": return isBinaryOperator ? OperatorType.ET : OperatorType.ERR;
                    case "&": return isBinaryOperator ? OperatorType.AND : OperatorType.ERR;
                    case "|": return isBinaryOperator ? OperatorType.OR : OperatorType.ERR;
                    case ",": return isBinaryOperator ? OperatorType.CA : OperatorType.ERR;
                    case "@": return isBinaryOperator ? OperatorType.END : OperatorType.ERR;
                    default: return OperatorType.ERR;
                }
            }

            /// <summary>
            /// 转换运算符到指定的类型
            /// </summary>
            /// <param name="opt">运算符</param>
            /// <returns>返回指定的运算符类型</returns>
            public static OperatorType ConvertOperator(string opt)
            {
                switch (opt)
                {
                    case "!": return OperatorType.NOT;
                    case "+": return OperatorType.ADD;
                    case "-": return OperatorType.SUB;
                    case "*": return OperatorType.MUL;
                    case "/": return OperatorType.DIV;
                    case "%": return OperatorType.MOD;
                    case "<": return OperatorType.LT;
                    case ">": return OperatorType.GT;
                    case "<=": return OperatorType.LE;
                    case ">=": return OperatorType.GE;
                    case "<>": return OperatorType.UT;
                    case "=": return OperatorType.ET;
                    case "&": return OperatorType.AND;
                    case "|": return OperatorType.OR;
                    case ",": return OperatorType.CA;
                    case "@": return OperatorType.END;
                    case "tan": return OperatorType.TAN;
                    case "atan": return OperatorType.ATAN;
                    default: return OperatorType.ERR;
                }
            }

            /// <summary>
            /// 运算符是否为二元运算符,该方法有问题，暂不使用
            /// </summary>
            /// <param name="tokens">语法单元堆栈</param>
            /// <param name="operators">运算符堆栈</param>
            /// <param name="currentOpd">当前操作数</param>
            /// <returns>是返回真,否返回假</returns>
            public static bool IsBinaryOperator(ref Stack<object> tokens, ref Stack<Operator> operators, string currentOpd)
            {
                if (currentOpd != "")
                {
                    return true;
                }
                else
                {
                    object token = tokens.Peek();
                    if (token is Operand)
                    {
                        if (operators.Peek().Type != OperatorType.LB)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (((Operator)token).Type == OperatorType.RB)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            /// <summary>
            /// 运算符优先级比较
            /// </summary>
            /// <param name="optA">运算符类型A</param>
            /// <param name="optB">运算符类型B</param>
            /// <returns>A与B相比，-1，低；0,相等；1，高</returns>
            public static int ComparePriority(OperatorType optA, OperatorType optB)
            {
                if (optA == optB)
                {
                    //A、B优先级相等
                    return 0;
                }

                //乘,除,余(*,/,%)
                if ((optA >= OperatorType.MUL && optA <= OperatorType.MOD) &&
                    (optB >= OperatorType.MUL && optB <= OperatorType.MOD))
                {
                    return 0;
                }
                //加,减(+,-)
                if ((optA >= OperatorType.ADD && optA <= OperatorType.SUB) &&
                    (optB >= OperatorType.ADD && optB <= OperatorType.SUB))
                {
                    return 0;
                }
                //小于,小于或等于,大于,大于或等于(<,<=,>,>=)
                if ((optA >= OperatorType.LT && optA <= OperatorType.GE) &&
                    (optB >= OperatorType.LT && optB <= OperatorType.GE))
                {
                    return 0;
                }
                //等于,不等于(=,<>)
                if ((optA >= OperatorType.ET && optA <= OperatorType.UT) &&
                    (optB >= OperatorType.ET && optB <= OperatorType.UT))
                {
                    return 0;
                }
                //三角函数
                if ((optA >= OperatorType.TAN && optA <= OperatorType.ATAN) &&
                        (optB >= OperatorType.TAN && optB <= OperatorType.ATAN))
                {
                    return 0;
                }

                if (optA < optB)
                {
                    //A优先级高于B
                    return 1;
                }

                //A优先级低于B
                return -1;

            }
        }

        /// <summary>
        /// Reverse Polish Notation
        /// 逆波兰式
        /// </summary>
        public class RPN
        {
            Stack<object> m_tokens = new Stack<object>();            //最终逆波兰式堆栈
                                                                     /// <summary>
                                                                     /// 最终逆波兰式堆栈
                                                                     /// </summary>
            public Stack<object> Tokens
            {
                get { return m_tokens; }
            }

            private string _RPNExpression;
            /// <summary>
            /// 生成的逆波兰式字符串
            /// </summary>
            public string RPNExpression
            {
                get
                {
                    if (_RPNExpression == null)
                    {
                        foreach (var item in Tokens)
                        {
                            if (item is Operand)
                            {
                                _RPNExpression += ((Operand)item).Value + ",";
                            }
                            if (item is Operator)
                            {
                                _RPNExpression += ((Operator)item).Value + ",";
                            }
                        }
                    }
                    return _RPNExpression;
                }
            }

            List<string> m_Operators = new List<string>(new string[]{
            "(","tan",")","atan","!","*","/","%","+","-","<",">","=","&","|",",","@"});    //允许使用的运算符

            /// <summary>
            /// 检查表达式中特殊符号(双引号、单引号、井号、左右括号)是否匹配 1         
            /// </summary>
            /// <param name="exp"></param>
            /// <returns></returns>
            private bool IsMatching(string exp)
            {
                string opt = "";    //临时存储 " ' # (

                for (int i = 0; i < exp.Length; i++)
                {
                    string chr = exp.Substring(i, 1);   //读取每个字符
                    if ("\"'#".Contains(chr))   //当前字符是双引号、单引号、井号的一种
                    {
                        if (opt.Contains(chr))  //之前已经读到过该字符
                        {
                            opt = opt.Remove(opt.IndexOf(chr), 1);  //移除之前读到的该字符，即匹配的字符
                        }
                        else
                        {
                            opt += chr;     //第一次读到该字符时，存储
                        }
                    }
                    else if ("()".Contains(chr))    //左右括号
                    {
                        if (chr == "(")
                        {
                            opt += chr;
                        }
                        else if (chr == ")")
                        {
                            if (opt.Contains("("))
                            {
                                opt = opt.Remove(opt.IndexOf("("), 1);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                return (opt == "");
            }

            /// <summary>
            /// 从表达式中查找运算符位置
            /// </summary>
            /// <param name="exp">表达式</param>
            /// <param name="findOpt">要查找的运算符</param>
            /// <returns>返回运算符位置</returns>
            private int FindOperator(string exp, string findOpt)
            {
                string opt = "";
                for (int i = 0; i < exp.Length; i++)
                {
                    string chr = exp.Substring(i, 1);
                    if ("\"'#".Contains(chr))//忽略双引号、单引号、井号中的运算符
                    {
                        if (opt.Contains(chr))
                        {
                            opt = opt.Remove(opt.IndexOf(chr), 1);
                        }
                        else
                        {
                            opt += chr;
                        }
                    }
                    if (opt == "")
                    {
                        if (findOpt != "")
                        {
                            if (findOpt == chr)
                            {
                                return i;
                            }
                        }
                        else
                        {
                            if (m_Operators.Exists(x => x.Contains(chr)))
                            {
                                return i;
                            }
                        }
                    }
                }
                return -1;
            }

            /// <summary>
            /// 语法解析,将中缀表达式转换成后缀表达式(即逆波兰表达式)
            /// </summary>
            /// <param name="exp"></param>
            /// <returns></returns>
            public bool Parse(string exp)
            {
                m_tokens.Clear();//清空语法单元堆栈
                if (exp.Trim() == "")//表达式不能为空
                {
                    return false;
                }
                else if (!this.IsMatching(exp))//括号、引号、单引号等必须配对
                {
                    return false;
                }

                Stack<object> operands = new Stack<object>();             //操作数堆栈
                Stack<Operator> operators = new Stack<Operator>();      //运算符堆栈
                OperatorType optType = OperatorType.ERR;                //运算符类型
                string curOpd = "";                                 //当前操作数
                string curOpt = "";                                 //当前运算符
                int curPos = 0;                                     //当前位置
                                                                    //int funcCount = 0;                                        //函数数量

                curPos = FindOperator(exp, "");

                exp += "@"; //结束操作符
                while (true)
                {
                    curPos = FindOperator(exp, "");

                    curOpd = exp.Substring(0, curPos).Trim();
                    curOpt = exp.Substring(curPos, 1);

                    //////////////测试代码///////////////////////////////////
                    //System.Diagnostics.Debug.WriteLine("***************");
                    //System.Diagnostics.Debug.WriteLine("当前读取的操作数：" + curOpd);

                    //foreach (var item in operands.ToArray())
                    //{
                    //    if (item is Operand)
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("操作数栈：" + ((Operand)item).Value);
                    //    }
                    //    if (item is Operator)
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("操作数栈：" + ((Operator)item).Value);
                    //    }
                    //}

                    //System.Diagnostics.Debug.WriteLine("当前读取的运算符：" + curOpt);
                    //foreach (var item in operators.ToArray())
                    //{
                    //    System.Diagnostics.Debug.WriteLine("运算符栈：" + item.Value);
                    //}
                    ////////////////////////////////////////////////////////

                    //存储当前操作数到操作数堆栈
                    if (curOpd != "")
                    {
                        operands.Push(new Operand(curOpd, curOpd));
                    }

                    //若当前运算符为结束运算符，则停止循环
                    if (curOpt == "@")
                    {
                        break;
                    }
                    //若当前运算符为左括号,则直接存入堆栈。
                    if (curOpt == "(")
                    {
                        operators.Push(new Operator(OperatorType.LB, "("));
                        exp = exp.Substring(curPos + 1).Trim();
                        continue;
                    }

                    //若当前运算符为右括号,则依次弹出运算符堆栈中的运算符并存入到操作数堆栈,直到遇到左括号为止,此时抛弃该左括号.
                    if (curOpt == ")")
                    {
                        while (operators.Count > 0)
                        {
                            if (operators.Peek().Type != OperatorType.LB)
                            {
                                operands.Push(operators.Pop());
                            }
                            else
                            {
                                operators.Pop();
                                break;
                            }
                        }
                        exp = exp.Substring(curPos + 1).Trim();
                        continue;
                    }


                    //调整运算符
                    curOpt = Operator.AdjustOperator(curOpt, exp, ref curPos);

                    optType = Operator.ConvertOperator(curOpt);

                    //若运算符堆栈为空,或者若运算符堆栈栈顶为左括号,则将当前运算符直接存入运算符堆栈.
                    if (operators.Count == 0 || operators.Peek().Type == OperatorType.LB)
                    {
                        operators.Push(new Operator(optType, curOpt));
                        exp = exp.Substring(curPos + 1).Trim();
                        continue;
                    }

                    //若当前运算符优先级大于运算符栈顶的运算符,则将当前运算符直接存入运算符堆栈.
                    if (Operator.ComparePriority(optType, operators.Peek().Type) > 0)
                    {
                        operators.Push(new Operator(optType, curOpt));
                    }
                    else
                    {
                        //若当前运算符若比运算符堆栈栈顶的运算符优先级低或相等，则输出栈顶运算符到操作数堆栈，直至运算符栈栈顶运算符低于（不包括等于）该运算符优先级，
                        //或运算符栈栈顶运算符为左括号
                        //并将当前运算符压入运算符堆栈。
                        while (operators.Count > 0)
                        {
                            if (Operator.ComparePriority(optType, operators.Peek().Type) <= 0 && operators.Peek().Type != OperatorType.LB)
                            {
                                operands.Push(operators.Pop());

                                if (operators.Count == 0)
                                {
                                    operators.Push(new Operator(optType, curOpt));
                                    break;
                                }
                            }
                            else
                            {
                                operators.Push(new Operator(optType, curOpt));
                                break;
                            }
                        }

                    }
                    exp = exp.Substring(curPos + 1).Trim();
                }
                //转换完成,若运算符堆栈中尚有运算符时,
                //则依序取出运算符到操作数堆栈,直到运算符堆栈为空
                while (operators.Count > 0)
                {
                    operands.Push(operators.Pop());
                }
                //调整操作数栈中对象的顺序并输出到最终栈
                while (operands.Count > 0)
                {
                    m_tokens.Push(operands.Pop());
                }

                return true;
            }
            /// <summary>
            /// 对逆波兰表达式求值  1         
            /// </summary>
            /// <returns></returns>
            public object Evaluate()
            {
                /*
                  逆波兰表达式求值算法：
                  1、循环扫描语法单元的项目。
                  2、如果扫描的项目是操作数，则将其压入操作数堆栈，并扫描下一个项目。
                  3、如果扫描的项目是一个二元运算符，则对栈的顶上两个操作数执行该运算。
                  4、如果扫描的项目是一个一元运算符，则对栈的最顶上操作数执行该运算。
                  5、将运算结果重新压入堆栈。
                  6、重复步骤2-5，堆栈中即为结果值。
                */

                if (m_tokens.Count == 0) return null;

                object value = null;
                Stack<Operand> opds = new Stack<Operand>();
                Stack<object> pars = new Stack<object>();
                Operand opdA, opdB;

                foreach (object item in m_tokens)
                {
                    if (item is Operand)
                    {
                        //TODO 解析公式，替换参数

                        //如果为操作数则压入操作数堆栈
                        opds.Push((Operand)item);
                    }
                    else
                    {
                        switch (((Operator)item).Type)
                        {
                            #region 乘,*,multiplication
                            case OperatorType.MUL:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) * double.Parse(opdA.Value.ToString())));
                                }
                                else
                                {
                                    throw new Exception("乘运算的两个操作数必须均为数字");
                                }
                                break;
                            #endregion

                            #region 除,/,division
                            case OperatorType.DIV:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) / double.Parse(opdA.Value.ToString())));
                                }
                                else
                                {
                                    throw new Exception("除运算的两个操作数必须均为数字");
                                }
                                break;
                            #endregion

                            #region 余,%,modulus
                            case OperatorType.MOD:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) % double.Parse(opdA.Value.ToString())));
                                }
                                else
                                {
                                    throw new Exception("余运算的两个操作数必须均为数字");
                                }
                                break;
                            #endregion

                            #region 加,+,Addition
                            case OperatorType.ADD:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) + double.Parse(opdA.Value.ToString())));
                                }
                                else
                                {
                                    throw new Exception("加运算的两个操作数必须均为数字");
                                }
                                break;
                            #endregion

                            #region 减,-,subtraction
                            case OperatorType.SUB:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) - double.Parse(opdA.Value.ToString())));
                                }
                                else
                                {
                                    throw new Exception("减运算的两个操作数必须均为数字");
                                }
                                break;
                            #endregion

                            #region 正切,tan,subtraction
                            case OperatorType.TAN:
                                opdA = opds.Pop();
                                if (Operand.IsNumber(opdA.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                                }
                                else
                                {
                                    throw new Exception("正切运算的1个操作数必须均为角度数字");
                                }
                                break;
                            #endregion

                            #region 反正切,atan,subtraction
                            case OperatorType.ATAN:
                                opdA = opds.Pop();
                                if (Operand.IsNumber(opdA.Value))
                                {
                                    opds.Push(new Operand(OperandType.NUMBER, Math.Atan(double.Parse(opdA.Value.ToString()))));
                                }
                                else
                                {
                                    throw new Exception("反正切运算的1个操作数必须均为数字");
                                }
                                break;
                                #endregion

                        }
                    }
                }

                if (opds.Count == 1)
                {
                    value = opds.Pop().Value;
                }

                return value;
            }
        }
    }
}



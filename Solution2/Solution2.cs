using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LittleAssembler
{
	public class Translator
	{
		internal class Register
		{
			public string Name { get; set; }
			public bool Available { get; set; }

			public override string ToString()
			{
				return Name;
			}
		}

		internal static List<Register> Registers = new List<Register>
		{
			new Register {Name = "A", Available = true },
			new Register {Name = "B", Available = true },
			new Register {Name = "C", Available = true },
			new Register {Name = "D", Available = true },
		};

		public static string Translate(Expression<Func<int, int>> expression)
		{
			return DecomposeLambda(expression);
		}
		public static string Translate(Expression<Func<int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		public static string Translate(Expression<Func<int, int, int, int, int, int, int, int, int, int, int>> expression)
		{
			return DecomposeLambda(expression);
		}

		private static string DecomposeLambda(LambdaExpression expression)
		{
			var memory = expression.Parameters;
			var body = expression.Body;
			var sb = new StringBuilder();
			Decompose(body, memory, ref sb);
			return sb.ToString();
		}

		private static void Decompose(Expression expression, ReadOnlyCollection<ParameterExpression> parameters, ref StringBuilder sb)
		{
			if (expression is BinaryExpression)
			{
				var binaryExpression = (BinaryExpression)expression;
				var left = binaryExpression.Left;
				Decompose(left, parameters, ref sb);
				var right = binaryExpression.Right;
				Decompose(right, parameters, ref sb);
				BinaryExpressionCommand(binaryExpression.NodeType, Registers[0], Registers[1], ref sb);
				return;
			}
			if (expression is UnaryExpression)
			{
				var unaryExpression = (UnaryExpression)expression;
				var nodeType = unaryExpression.NodeType;
				var operand = unaryExpression.Operand;
				Decompose(operand, parameters, ref sb);
				UnaryExpressionCommand(unaryExpression.NodeType, Registers[0], ref sb);
			}
			if (expression is ParameterExpression)
			{
				var parameterExperssion = (ParameterExpression)expression;
				var memoryIndex = parameters.IndexOf(parameterExperssion);

				sb.AppendLine(string.Format("MOV {0} {1}", memoryIndex, Registers[0]));
				return;
			}
			if (expression is ConstantExpression)
			{
				var value = ((ConstantExpression)expression).Value;
				sb.AppendLine(string.Format("MOV {0} {1}", value, Registers[0]));
				return;
			}
		}

		private static void BinaryExpressionCommand(ExpressionType nodeType, Register reg1, Register reg2, ref StringBuilder sb)
		{
			string command;
			switch (nodeType)
			{
				case ExpressionType.Add:
					command = "ADD";
					break;
				case ExpressionType.Divide:
					command = "DIV";
					break;
				case ExpressionType.Subtract:
					command = "SUB";
					break;
				case ExpressionType.Multiply:
					command = "MUL";
					break;
				default:
					throw new ArgumentOutOfRangeException("nodeType", nodeType, null);
			}

			sb.AppendLine(string.Format("{0} {1} {2}", command, reg1.ToString(), reg2.ToString()));
		}

		private static void UnaryExpressionCommand(ExpressionType nodeType, Register reg1, ref StringBuilder sb)
		{
			string command;
			switch (nodeType)
			{
				case ExpressionType.Negate:
					command = "SUB";
					break;
				default:
					throw new ArgumentOutOfRangeException("nodeType", nodeType, null);
			}

			sb.AppendLine(string.Format("{0} {1} {2}", command, reg1.ToString(), reg1.ToString()));
		}
	}

}

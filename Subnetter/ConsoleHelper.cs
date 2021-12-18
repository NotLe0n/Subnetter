using System;

namespace Subnetter
{
	public static class ConsoleHelper
	{
		/// <summary>
		/// Schreibt einen gefärbten string in die Konsole
		/// </summary>
		/// <param name="str">der string der in die Konsole geschrieben werden soll</param>
		/// <param name="color">die Farbe die der string bekommen soll</param>
		public static void WriteColoredLine(string str, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(str);
			Console.ResetColor();
		}
	}
}

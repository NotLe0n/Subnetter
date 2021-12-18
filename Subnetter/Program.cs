using System;

namespace Subnetter
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			eingabe:
			ConsoleHelper.WriteColoredLine("Bitte wähle aus was du machen willst!", ConsoleColor.Yellow);
			Console.WriteLine("1) Zufällige Aufgabe\n2) Eigene Aufgabe");

			switch (Console.ReadKey().Key)
			{
				case ConsoleKey.D1:
					Console.Clear();
					ZufallsAufgabe();
					break;
				case ConsoleKey.D2:
					Console.Clear();
					EigeneAufgabe();
					break;
				default:
					ConsoleHelper.WriteColoredLine("\nBitte nur die Zahlen 1 oder 2 eingeben.", ConsoleColor.Red);
					goto eingabe;
			}

			Console.WriteLine("\n\nDrücken Sie eine Taste um das Programm zu schließen");
			Console.ReadKey();
		}

		private static void EigeneAufgabe()
		{
			// IP eingabe
			Console.WriteLine("Bitte geben Sie eine gültige IP Adresse mit CIDR ein");
			IP ip;
			try {
				ip = new IP(Console.ReadLine());
			}
			catch(Exception e) {
				ConsoleHelper.WriteColoredLine(e.Message, ConsoleColor.Red);
				return;
			}

			// FLSM Netze eingabe
			eingabe:
			Console.WriteLine("Bitte geben Sie ein wie viele Teilnetze gebildet werden sollen");
			if (!int.TryParse(Console.ReadLine(), out int anzahl))
			{
				ConsoleHelper.WriteColoredLine("Bitte nur ganze Zahlen eingeben", ConsoleColor.Red);
				goto eingabe;
			}

			// VLSM Netze eingabe
			Console.WriteLine("Bitte geben Sie 6 Netzgrößen in Hosts ein");
			uint[] VLSM_netze = new uint[6];
			for (int i = 0; i < 6; i++)
			{
				zurück:
				if (!uint.TryParse(Console.ReadLine(), out uint num))
				{
					ConsoleHelper.WriteColoredLine("Fehlerhafte eingabe", ConsoleColor.Red);
					goto zurück;
				}

				VLSM_netze[i] = num;
			}

			// FLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[FLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.FLSM(ip, anzahl);

			// VLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[VLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.VLSM(ip, VLSM_netze);
		}

		private static void ZufallsAufgabe()
		{
			Random rand = new Random();

			// Zufällige IP-Adresse Generieren
			const uint min = 0x0A000000; // 10.0.0.0
			const uint max = 0xFFFFFFFE; // 255.255.255.254

			// IP von 10.0.0.0 - 255.255.255.254 mit subnetzmaske von 8-25
			IP ip = new IP((uint)(rand.NextDouble() * (max - min) + min), (uint)rand.Next(8, 25)); 
			Console.WriteLine(ip);

			// FLSM Aufgabe generieren
			int FLSM_netze = rand.Next(4, 16);
			Console.WriteLine($"[FLSM] Erstellen Sie bitte {FLSM_netze} Teilnetze!");

			// VLSM Aufgabe generieren
			uint[] VLSM_netze = { GenNum(), GenNum() / 3, GenNum() / 8, 2, 2, 2 };
			Console.WriteLine($"[VLSM] Erstellen Sie die Folgenden Teilnetze:");
			Console.WriteLine($"  LAN1: {VLSM_netze[0]} Hosts");
			Console.WriteLine($"  LAN2: {VLSM_netze[1]} Hosts");
			Console.WriteLine($"  LAN3: {VLSM_netze[2]} Hosts");
			Console.WriteLine($"  Link A, B, C: 2 Hosts\n");

			// Zufällige anzahl an hosts für VLSM Aufgabe generieren
			uint GenNum() => (uint)rand.Next((int)Math.Pow(2, 30 - ip.cidr), (int)Math.Pow(2, 31 - ip.cidr));

			ConsoleHelper.WriteColoredLine("Drücken Sie eine Taste um die Lösungen anzuzeigen!", ConsoleColor.Cyan);
			Console.ReadKey();

			// FLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[FLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.FLSM(ip, FLSM_netze);

			// VLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[VLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.VLSM(ip, VLSM_netze);
		}
	}
}

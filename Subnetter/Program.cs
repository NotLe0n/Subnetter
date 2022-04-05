using System;

namespace Subnetter;

internal class Program
{
	private static void Main()
	{
	eingabe:
		ConsoleHelper.WriteColoredLine("Bitte wähle aus was du machen willst!", ConsoleColor.Yellow);
		Console.WriteLine("1) Zufällige Aufgabe\n2) Eigene Aufgabe");

		var key = Console.ReadKey().Key;
		Console.Clear();
		if (key == ConsoleKey.D1) {
			ZufallsAufgabe();
		}
		else if (key == ConsoleKey.D2) {
			EigeneAufgabe();
		}
		else {
			ConsoleHelper.WriteColoredLine("\nBitte nur die Zahlen 1 oder 2 eingeben.", ConsoleColor.Red);
			goto eingabe;
		}

		Console.WriteLine("\n\nDrücken Sie eine Taste um das Programm zu schließen");
		Console.ReadKey();
	}

	private static IPv4 ip;
	private static uint FLSMNetze;
	private static uint[] VLSMNetze;

	private static void EigeneAufgabe()
	{
		// IP eingabe
		Console.WriteLine("Bitte geben Sie eine gültige IP Adresse mit CIDR ein");
		try {
			ip = new IPv4(Console.ReadLine());
		}
		catch (Exception e) {
			ConsoleHelper.WriteColoredLine(e.Message, ConsoleColor.Red);
			return;
		}

		// Subnetting typ auswahl
		Console.Write("(F)LSM, (V)LSM oder (b)eides: ");
		var key = Console.ReadKey().Key;
		Console.Write('\n');

		if (key is ConsoleKey.F or ConsoleKey.B) {
			Console.WriteLine("Bitte geben Sie ein wie viele Teilnetze gebildet werden sollen");

		eingabe:
			if (!uint.TryParse(Console.ReadLine(), out FLSMNetze)) {
				ConsoleHelper.WriteColoredLine("Bitte nur ganze Zahlen eingeben", ConsoleColor.Red);
				goto eingabe;
			}

			// FLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[FLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.FLSM(ip, FLSMNetze);
		}

		if (key is ConsoleKey.V or ConsoleKey.B) {
			Console.WriteLine("Bitte geben Sie 6 Netzgrößen in Hosts ein");

			// VLSM Netze eingabe
			VLSMNetze = new uint[6];
			for (int i = 0; i < 6; i++) {
			zurück:
				if (!uint.TryParse(Console.ReadLine(), out uint num)) {
					ConsoleHelper.WriteColoredLine("Bitte nur ganze Zahlen eingeben", ConsoleColor.Red);
					goto zurück;
				}

				VLSMNetze[i] = num;
			}

			// VLSM Lösung
			ConsoleHelper.WriteColoredLine("\n[VLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.VLSM(ip, VLSMNetze);
		}
	}

	private static void ZufallsAufgabe()
	{
		Random rand = new();

		// Zufällige IP-Adresse Generieren
		const uint min = 0x0A000000; // 10.0.0.0
		const uint max = 0xFFFFFFFE; // 255.255.255.254

		// IP von 10.0.0.0 - 255.255.255.254 mit subnetzmaske von 8-25
		ip = new IPv4((uint)(rand.NextDouble() * (max - min) + min), (uint)rand.Next(8, 25));
		Console.WriteLine(ip);

		// Subnetting typ auswahl
		Console.Write("(F)LSM, (V)LSM oder (b)eides: ");
		var key = Console.ReadKey().Key;
		Console.Write('\n');

		if (key is ConsoleKey.F or ConsoleKey.B) {
			FLSMNetze = GenFLSM(rand);
		}
		if (key is ConsoleKey.V or ConsoleKey.B) {
			VLSMNetze = GenVLSM(rand, ip);
		}

		ConsoleHelper.WriteColoredLine("Drücken Sie eine Taste um die Lösungen anzuzeigen!", ConsoleColor.Cyan);
		Console.ReadKey();

		LösungAnzeigen(key);
	}

	private static uint GenFLSM(in Random rand)
	{
		// FLSM Aufgabe generieren
		uint netze = (uint)rand.Next(4, 16);
		Console.WriteLine($"[FLSM] Erstellen Sie bitte {netze} Teilnetze!");

		return netze;
	}

	private static uint[] GenVLSM(in Random rand, in IPv4 ip)
	{
		// VLSM Aufgabe generieren
		uint[] netze = new uint[] { GenNum(rand, ip), GenNum(rand, ip) / 3, GenNum(rand, ip) / 8, 2, 2, 2 };
		Console.WriteLine($"[VLSM] Erstellen Sie die Folgenden Teilnetze:");
		Console.WriteLine($"  LAN1: {netze[0]} Hosts");
		Console.WriteLine($"  LAN2: {netze[1]} Hosts");
		Console.WriteLine($"  LAN3: {netze[2]} Hosts");
		Console.WriteLine($"  Link A, B, C: 2 Hosts\n");

		return netze;
	}

	// Zufällige anzahl an hosts für VLSM Aufgabe generieren
	private static uint GenNum(in Random rand, in IPv4 ip)
	{
		return (uint)rand.Next((int)Math.Pow(2, 30 - ip.CIDR), (int)Math.Pow(2, 31 - ip.CIDR));
	}

	private static void LösungAnzeigen(ConsoleKey key)
	{
		if (key is ConsoleKey.F or ConsoleKey.B) {
			ConsoleHelper.WriteColoredLine("\n[FLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.FLSM(ip, FLSMNetze);
		}
		if (key is ConsoleKey.V or ConsoleKey.B) {
			ConsoleHelper.WriteColoredLine("\n[VLSM] LÖSUNG:\n", ConsoleColor.Green);
			Subnetting.VLSM(ip, VLSMNetze);
		}
	}
}


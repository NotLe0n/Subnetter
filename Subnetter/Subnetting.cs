using System;

namespace Subnetter
{
	public static class Subnetting
	{
		/// <summary>
		/// Teilt ein Netz in eine gegebene Anzahl an Teilnetzen
		/// </summary>
		/// <param name="ip">Eine IP in einem Netzwerk</param>
		/// <param name="subnets">die Anzahl der Teilnetze die generiert werden soll</param>
		public static void FLSM(IP ip, int subnets)
		{
			byte verschiebung = 1; // um wie viele stellen die alte subnetzmaske verschoben werden muss um die neue zu bekommen
			byte anzahl = 2; // wie viele netze erstellt werden *müssen* (hoch gerundet auf die nächste Zweierpotenz)
			while (anzahl % subnets == anzahl)
			{
				verschiebung++;
				anzahl *= 2;
			}

			uint increment = (uint)Math.Pow(2, 32u - (ip.cidr + verschiebung)); // inkrement ausrechnen: 2^(32-(cidr+verschiebung))

			ConsoleHelper.WriteColoredLine($"-> Neue CIDR: {ip.cidr + verschiebung}, Inkrement: {increment % 255} ({increment}), Verschiebung: {verschiebung}, Unbenutzte Netze: {anzahl - subnets}, Hosts: {ip.Hosts}",
				ConsoleColor.Yellow);

			ip.cidr += verschiebung; // cidr verändern

			ConsoleHelper.WriteColoredLine("# :   Netzadresse   | Broadcastadresse", ConsoleColor.Yellow);
			for (int i = 1; i <= anzahl; i++)
			{
				if (i > subnets)
					Console.ForegroundColor = ConsoleColor.DarkGray;

				Console.WriteLine($"{i,-2}: {ip.NetzAdresseDD,-15} | {ip.BroadcastDD,-15}");
				ip.adresse = ip.NetzAdresse + increment;

				Console.ResetColor();
			}
		}

		/// <summary>
		/// Teilt ein Netz in mehrere Teilnetze mit verschiedener Größe
		/// </summary>
		/// <param name="ip">Eine IP in einem Netzwerk</param>
		/// <param name="hosts">die mindest Anzahl an Hosts in dem Teilnetz</param>
		public static void VLSM(IP ip, params uint[] hosts)
		{
			ConsoleHelper.WriteColoredLine("# :   Netzadresse   | Broadcastadresse | CIDR", ConsoleColor.Yellow);

			for (int i = 1; i <= hosts.Length; i++)
			{
				// Ceil(log2(num)) gibt mir den Zweierexponent von "num"
				// passt die gegebene Hostzahl in die Hostzahl der IP?
				while (Math.Log2(ip.Hosts) != Math.Ceiling(Math.Log2(hosts[i - 1] + 2))) // plus 2 wegen Netz- und Broadcastadresse
				{
					ip.cidr++; // wenn nein erhöhe die CIDR (halbiere die Hostzahl)
				}

				Console.WriteLine($"{i,-2}: {ip.NetzAdresseDD,-15} | {ip.BroadcastDD,-16} | /{ip.cidr}");
				ip.adresse = ip.NetzAdresse + ip.Hosts; // die Anzahl der hosts ist automatisch das Inkement
			}
		}
	}
}

using System;

namespace Subnetter;

public static class Subnetting
{
	/// <summary>
	/// Teilt ein Netz in eine gegebene Anzahl an Teilnetzen mit selber größe
	/// </summary>
	/// <param name="ip">Eine IP in einem Netzwerk</param>
	/// <param name="subnets">die Anzahl der Teilnetze die generiert werden soll</param>
	public static void FLSM(IPv4 ip, uint subnets)
	{
		byte verschiebung = 1; // um wie viele stellen die alte subnetzmaske verschoben werden muss um die neue zu bekommen
		byte anzahl = 2; // wie viele netze erstellt werden *müssen* (hoch gerundet auf die nächste Zweierpotenz)
		while (anzahl % subnets == anzahl) {
			verschiebung++;
			anzahl *= 2;
		}

		IPv4 neueIP = new(ip.NetzAdresse, ip.CIDR + verschiebung);

		ConsoleHelper.WriteColoredLine($"-> Neue CIDR: {neueIP.CIDR}, Inkrement: {neueIP.Hosts % 255} ({neueIP.Hosts}), Verschiebung: {verschiebung}, Unbenutzte Netze: {anzahl - subnets}, Hosts: {neueIP.Hosts}",
			ConsoleColor.Yellow);

		ConsoleHelper.WriteColoredLine("# :   Netzadresse   | Broadcastadresse", ConsoleColor.Yellow);
		for (int i = 1; i <= anzahl; i++) {
			if (i > subnets)
				Console.ForegroundColor = ConsoleColor.DarkGray;

			Console.WriteLine($"{i,-2}: {IPv4.ToDD(neueIP.NetzAdresse),-15} | {IPv4.ToDD(neueIP.BroadcastAdresse),-15}");
			neueIP.Adresse += neueIP.Hosts; // die anzahl der Hosts ist automatisch das Inkrement

			Console.ResetColor();
		}
	}

	/// <summary>
	/// Teilt ein Netz in mehrere Teilnetze mit verschiedener Größe
	/// </summary>
	/// <param name="ip">Eine IP in einem Netzwerk</param>
	/// <param name="hosts">die mindest Anzahl an Hosts in dem Teilnetz</param>
	public static void VLSM(IPv4 ip, params uint[] hosts)
	{
		IPv4 neueIP = new(ip.NetzAdresse, ip.CIDR);

		ConsoleHelper.WriteColoredLine("# :   Netzadresse   | Broadcastadresse | CIDR", ConsoleColor.Yellow);

		for (int i = 1; i <= hosts.Length; i++) {
			// Ceil(log2(num)) gibt mir den Zweierexponent von "num"
			// passt die gegebene Hostzahl in die Hostzahl der IP?
			while (Math.Log2(neueIP.Hosts) != Math.Ceiling(Math.Log2(hosts[i - 1] + 2))) // plus 2 wegen Netz- und Broadcastadresse
			{
				neueIP.CIDR++; // wenn nein erhöhe die CIDR (halbiere die Hostzahl)
			}

			Console.WriteLine($"{i,-2}: {IPv4.ToDD(neueIP.NetzAdresse),-15} | {IPv4.ToDD(neueIP.BroadcastAdresse),-16} | /{neueIP.CIDR}");
			neueIP.Adresse += neueIP.Hosts; // die Anzahl der hosts ist automatisch das Inkement
		}
	}
}

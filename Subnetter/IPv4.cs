using System;

namespace Subnetter;

public struct IPv4
{
	public uint Adresse { get; set; }
	public uint CIDR { get; set; }

	public readonly uint NetzAdresse => Adresse & Subnetzmaske;
	public readonly uint Subnetzmaske => ~(0xFFFFFFFF >> (int)CIDR);
	public readonly uint BroadcastAdresse => Adresse | (~Subnetzmaske);
	public readonly uint Hosts => (uint)Math.Pow(2, 32u - CIDR); // theoretisch

	public IPv4(uint adresse, uint cidr)
	{
		Adresse = adresse;
		CIDR = cidr;
	}

	public IPv4(string adresseDD, uint cidr)
	{
		string[] oktette = adresseDD.Trim().Split('.');
		if (oktette.Length > 4) {
			throw new ArgumentException("Ungültige IP: Zu viele Oktette");
		}
		else if (oktette.Length < 4) {
			throw new ArgumentException("Ungültige IP: Zu wenig Oktette");
		}

		Adresse = 0;
		for (int i = oktette.Length - 1; i >= 0; i--) {
			if (!uint.TryParse(oktette[i], out uint num)) {
				throw new ArgumentException("Ungültige IP: Ungültige Oktette");
			}

			if (num > 255) {
				throw new ArgumentException("Ungültige IP: Oktette zu groß");
			}

			Adresse |= num << (8 * (3 - i));
		}

		CIDR = cidr;
	}

	public IPv4(string adresseDDmitCIDR)
	{
		string[] splitted = adresseDDmitCIDR.Split("/");
		if (splitted.Length != 2) {
			throw new ArgumentException("Ungültige IP & CIDR Format: Benutzen Sie dieses format: <ip>/<cidr>");
		}

		// ip initialisieren
		string[] oktette = splitted[0].Trim().Split('.');
		if (oktette.Length > 4) {
			throw new ArgumentException("Ungültige IP: Zu viele Oktette");
		}
		else if (oktette.Length < 4) {
			throw new ArgumentException("Ungültige IP: Zu wenig Oktette");
		}

		Adresse = 0;
		for (int i = 0; i < oktette.Length; i++) {
			if (!uint.TryParse(oktette[i], out uint num)) {
				throw new ArgumentException("Ungültige IP: Ungültige Oktette");
			}

			if (num > 255) {
				throw new ArgumentException("Ungültige IP: Oktette zu groß");
			}

			Adresse |= num << (8 * (3 - i));
		}

		// CIDR initializieren
		if (splitted.Length != 2) {
			throw new ArgumentException("Ungültige IP: CIDR fehlt");
		}

		if (!uint.TryParse(splitted[1].Trim(), out uint cidr)) {
			throw new ArgumentException("Ungültige CIDR: Es sind nur ganze Zahlen von 0-32 möglich");
		}

		if (cidr > 32) {
			throw new ArgumentException("Ungültige CIDR: CIDR zu groß");
		}

		CIDR = cidr;
	}

	/// <summary>
	/// Gibt den Wert des gegebenen Oktetts zurück
	/// </summary>
	/// <param name="oktett">das Oktett wessen Wert zurückgegeben werden soll</param>
	/// <returns>den Wert des <paramref name="oktett"/>s</returns>
	public byte this[byte oktett]
	{
		get
		{
			if (oktett < 4) {
				return (byte)((Adresse & (uint)Math.Pow(255, 4 - oktett)) >> (8 * (3 - oktett)));
			}

			throw new IndexOutOfRangeException("IP Adressen haben nur 4 Oktette ([0-3])");
		}
	}

	public override string ToString()
	{
		return $"{this[0]}.{this[1]}.{this[2]}.{this[3]} /{CIDR}";
	}

	/// <summary>
	/// Konvertiert eine IP zu einer "Dotted Decimal" Notation
	/// </summary>
	/// <param name="ip">Die IP die Konvertiert werden soll</param>
	/// <returns>Die gegebene IP in "Dotted Decimal" Schreibweise</returns>
	public static string ToDD(uint ip)
	{
		return $"{(ip & 0xFF000000) >> 24}.{(ip & 0x00FF0000) >> 16}.{(ip & 0x0000FF00) >> 8}.{ip & 0x00000FF}";
	}
}

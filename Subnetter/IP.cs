using System;

namespace Subnetter
{
	public struct IP
	{
		public uint adresse;
		public uint cidr;

		public readonly uint NetzAdresse => adresse & Subnetzmaske;
		public readonly uint Subnetzmaske => ~(0xFFFFFFFF >> (int)cidr);
		public readonly uint BroadcastAdresse => adresse | (~Subnetzmaske);

		public readonly string NetzAdresseDD => $"{(NetzAdresse & 0xFF000000) >> 24}.{(NetzAdresse & 0x00FF0000) >> 16}.{(NetzAdresse & 0x0000FF00) >> 8}.{NetzAdresse & 0x00000FF}";
		public readonly string BroadcastDD => $"{(BroadcastAdresse & 0xFF000000) >> 24}.{(BroadcastAdresse & 0x00FF0000) >> 16}.{(BroadcastAdresse & 0x0000FF00) >> 8}.{BroadcastAdresse & 0x00000FF}";

		/// <summary>
		/// theoretisch
		/// </summary>
		public readonly uint Hosts => (uint)Math.Pow(2, 32u - cidr);

		public IP(uint adresse, uint cidr)
		{
			this.adresse = adresse;
			this.cidr = cidr;
		}

		public IP(string adresseDD, uint cidr)
		{
			string[] oktette = adresseDD.Trim().Split('.');
			if (oktette.Length > 4)
				throw new Exception("Ungültige IP: Zu viele Oktette");
			else if (oktette.Length < 4)
				throw new Exception("Ungültige IP: Zu wenig Oktette");

			adresse = 0;
			for (int i = oktette.Length - 1; i >= 0; i--)
			{
				if (!uint.TryParse(oktette[i], out uint num))
					throw new Exception("Ungültige IP: Ungültige Oktette");

				if (num > 255)
					throw new Exception("Ungültige IP: Oktette zu groß");

				adresse |= num << (8 * (3 - i));
			}

			this.cidr = cidr;
		}

		public IP(string adresseDDmitCIDR)
		{
			string[] splitted = adresseDDmitCIDR.Split("/");
			if (splitted.Length != 2)
				throw new Exception("Ungültige IP & CIDR Format: Benutzen Sie dieses format: <ip>/<cidr>");

			// ip initialisieren
			string[] oktette = splitted[0].Trim().Split('.');
			if (oktette.Length > 4)
				throw new Exception("Ungültige IP: Zu viele Oktette");
			else if (oktette.Length < 4)
				throw new Exception("Ungültige IP: Zu wenig Oktette");

			adresse = 0;
			for (int i = 0; i < oktette.Length; i++)
			{
				if (!uint.TryParse(oktette[i], out uint num))
					throw new Exception("Ungültige IP: Ungültige Oktette");

				if (num > 255)
					throw new Exception("Ungültige IP: Oktette zu groß");

				adresse |= num << (8 * (3 - i));
			}

			// CIDR initializieren
			if (splitted.Length != 2)
				throw new Exception("Ungültige IP: CIDR fehlt");

			if (!uint.TryParse(splitted[1].Trim(), out uint cidr))
				throw new Exception("Ungültige CIDR: Es sind nur ganze Zahlen von 0-32 möglich");

			if (cidr > 32)
				throw new Exception("Ungültige CIDR: CIDR zu groß");

			this.cidr = cidr;
		}

		/// <summary>
		/// Gibt den Wert des gegebenen Oktetts zurück
		/// </summary>
		/// <param name="oktett">das Oktett wessen Wert zurückgegeben werden soll</param>
		/// <returns>den Wert des <paramref name="oktett"/>s</returns>
		public byte this[byte oktett]
		{
			get => (oktett < 4) ? (byte)((adresse & (uint)Math.Pow(255, 4 - oktett)) >> 8 * (3 - oktett))
				: throw new IndexOutOfRangeException("IP Adressen haben nur 4 Oktette ([0-3])");
		}

		public override string ToString()
		{
			return $"{this[0]}.{this[1]}.{this[2]}.{this[3]} /{cidr}";
		}
	}
}

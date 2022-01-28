using System;
using System.Text;

namespace SensorThings.Server.Utils
{
	public class PayloadUtils
	{
		public static string ConvertUTF8BytesToString(byte[] payload)
		{
			if (payload == null || payload.Length == 0)
			{
				return string.Empty;
			}

			return Encoding.UTF8.GetString(payload, 0, payload.Length);
		}

		public static byte[] ConvertStringToUTF8Bytes(string payload)
		{
			return Encoding.UTF8.GetBytes(payload);
		}
	}
}

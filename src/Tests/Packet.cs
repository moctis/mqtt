﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hermes;
using Hermes.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tests
{
	public class Packet
	{
		public const string CommentSymbol = "#";

		public static byte[] ReadAllBytes(string path)
		{
			if (Path.GetExtension (path) != ".packet") {
				throw new ApplicationException (string.Format ("File extension {0} is invalid. .packet file is expected", Path.GetExtension(path)));
			}

			if (!File.Exists (path)) {
				throw new ApplicationException (string.Format ("The file {0} does not exists", path));
			}

			var bytes = new List<byte> ();

			foreach (var line in File.ReadLines (path).Where(l => !string.IsNullOrEmpty(l))) {
				var aux = line;
				var commentIndex = aux.IndexOf (Packet.CommentSymbol);

				if(commentIndex != -1) {
					aux = aux.Substring(0, commentIndex).Trim();
				}

				try {
					if(aux.StartsWith("\"")) {
						aux = aux.Replace ("\"", string.Empty);

						bytes.AddRange (Encoding.UTF8.GetBytes (aux));
					} else {
						var @byte = Convert.ToByte(aux, fromBase: 2);

						bytes.Add (@byte);
					}
				} catch {
					continue;
				}
			}

			return bytes.ToArray ();
		}

		public static T ReadMessage<T> (string path) where T : class, IMessage
		{
			if (Path.GetExtension (path) != ".json") {
				throw new ApplicationException (string.Format ("File extension {0} is invalid. .json file is expected", Path.GetExtension(path)));
			}

			if (!File.Exists (path)) {
				throw new ApplicationException (string.Format ("The file {0} does not exists", path));
			}

			var text = File.ReadAllText (path);

			return Deserialize<T> (text);
		}

		private static T Deserialize<T>(string serialized)
        {
			return JsonConvert.DeserializeObject<T> (serialized, new StringByteArrayConverter());
        }
	}
}

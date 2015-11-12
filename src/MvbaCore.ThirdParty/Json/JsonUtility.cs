using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MvbaCore.ThirdParty.Json
{
	public static class JsonUtility
	{
		public static T Deserialize<T>(string s, bool useTypePropertyToMapConcreteObjects = false)
		{
			return JsonConvert.DeserializeObject<T>(s, GetJsonSerializerSettings(useTypePropertyToMapConcreteObjects));
		}

		public static object Deserialize(string s, Type type, bool useTypePropertyToMapConcreteObjects = false)
		{
			return JsonConvert.DeserializeObject(s, type, GetJsonSerializerSettings(useTypePropertyToMapConcreteObjects));
		}

		public static T DeserializeFromJsonFile<T>(string filePath, bool useTypePropertyToMapConcreteObjects = false)
		{
			return (T)DeserializeFromJsonFile(filePath, typeof(T), useTypePropertyToMapConcreteObjects);
		}

		public static object DeserializeFromJsonFile(string filePath, Type type, bool useTypePropertyToMapConcreteObjects = false)
		{
			var serializer = JsonSerializer.Create(GetJsonSerializerSettings(useTypePropertyToMapConcreteObjects));
			using (var reader = new StreamReader(filePath))
			{
				return serializer.Deserialize(reader, type);
			}
		}

		public static object DeserializeFromStream(Stream stream, Type type, bool useTypePropertyToMapConcreteObjects = false)
		{
			var serializer = JsonSerializer.Create(GetJsonSerializerSettings(useTypePropertyToMapConcreteObjects));
			using (var reader = new StreamReader(stream))
			{
				return serializer.Deserialize(reader, type);
			}
		}

		private static JsonSerializerSettings GetJsonSerializerSettings(bool useTypePropertyToMapConcreteObjects)
		{
			var contractResolver = new HandlePrivateSettersDefaultContractResolver();
			var settings = new JsonSerializerSettings
			               {
				               ContractResolver = contractResolver,
				               ObjectCreationHandling = ObjectCreationHandling.Auto
			               };
			if (useTypePropertyToMapConcreteObjects)
			{
				settings.TypeNameHandling = TypeNameHandling.Auto;
			}
			return settings;
		}

		public static string SerializeForComparison<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
			                                                             {
				                                                             TypeNameHandling = TypeNameHandling.None,
				                                                             ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				                                                             Converters = new List<JsonConverter>
				                                                                          {
					                                                                          new IsoDateTimeConverter()
				                                                                          }
			                                                             });
		}

		public static string SerializeForWebRequest<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
			                                                             {
				                                                             TypeNameHandling = TypeNameHandling.All
			                                                             });
		}

		public static void SerializeToFile<T>(T obj, string filePath)
		{
			var serializer = new JsonSerializer
			                 {
				                 TypeNameHandling = TypeNameHandling.All
			                 };

			using (var streamWriter = new StreamWriter(filePath))
			{
				using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				{
					jsonTextWriter.Formatting = Formatting.Indented;
					using (var writer = jsonTextWriter)
					{
						serializer.Serialize(writer, obj);
					}
				}
			}
		}

		public static void SerializeToStream<T>(T obj, TextWriter fileStream)
		{
			var serializer = new JsonSerializer
			                 {
				                 TypeNameHandling = TypeNameHandling.All,
				                 PreserveReferencesHandling = PreserveReferencesHandling.Objects
			                 };
			serializer.Converters.Add(new IsoDateTimeConverter());

			using (var streamWriter = fileStream)
			{
				using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				{
					jsonTextWriter.Formatting = Formatting.Indented;
					using (var writer = jsonTextWriter)
					{
						serializer.Serialize(writer, obj);
					}
				}
			}
		}
	}
}
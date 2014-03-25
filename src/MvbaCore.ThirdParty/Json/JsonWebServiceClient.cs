//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MvbaCore.ThirdParty.Json
{
	public interface IJsonWebServiceClient
	{
		TOutput Post<TInput, TOutput>(string url, TInput data);
		TOutput Post<TOutput>(string url);
		TOutput PostDataContract<TInput, TOutput>(string url, TInput data);
	}

	public class JsonWebServiceClient : IJsonWebServiceClient
	{
		private const string ApplicationJsonContentType = "application/json";

		public TOutput Post<TInput, TOutput>(string url, TInput data)
		{
			var content = JsonUtility.SerializeForWebRequest(data);
			var result = new WebServiceClient().Post(url, content, ApplicationJsonContentType);
			return GetResponse<TOutput>(result);
		}

		public TOutput PostDataContract<TInput, TOutput>(string url, TInput data)
		{
			var memoryStream = new MemoryStream();
			new DataContractJsonSerializer(typeof(TInput)).WriteObject(memoryStream, data);
			var json = memoryStream.ToArray();
			memoryStream.Close();
			var content = Encoding.UTF8.GetString(json, 0, json.Length);
			var result = new WebServiceClient().Post(url, content, ApplicationJsonContentType);
			return GetResponse<TOutput>(result);
		}

		public TOutput Post<TOutput>(string url)
		{
			var result = new WebServiceClient().Post(url, ApplicationJsonContentType);
			return GetResponse<TOutput>(result);
		}

		private static Notification<TOutput> GetResponse<TOutput>(Notification<string> result)
		{
			if (result.Item == null)
			{
				return new Notification<TOutput>(result);
			}

			TOutput output;
			try
			{
				output = JsonUtility.Deserialize<TOutput>(result);
			}
			catch (Exception exception)
			{
				var notification = new Notification<TOutput>(Notification.ErrorFor("caught exception deserializing:\n" + result.Item + "\n" + exception.Message))
				                   {
					                   Item = default(TOutput)
				                   };
				return notification;
			}
			return output;
		}
	}
}
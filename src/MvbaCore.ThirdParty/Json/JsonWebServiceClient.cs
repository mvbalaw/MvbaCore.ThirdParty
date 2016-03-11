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

using JetBrains.Annotations;

namespace MvbaCore.ThirdParty.Json
{
	public interface IJsonWebServiceClient
	{
		Notification<TOutput> Post<TInput, TOutput>(string url, TInput data);
		Notification<TOutput> Post<TOutput>(string url);
		Notification<TOutput> PostDataContract<TInput, TOutput>(string url, TInput data);
	}

	[UsedImplicitly]
	public class JsonWebServiceClient : IJsonWebServiceClient
	{
		private const string ApplicationJsonContentType = "application/json";

		public Notification<TOutput> Post<TInput, TOutput>(string url, TInput data)
		{
			var content = JsonUtility.SerializeForWebRequest(data);
			var result = new WebServiceClient().Post(url, content, ApplicationJsonContentType);
			return GetResponse<TOutput>(result);
		}

		public Notification<TOutput> PostDataContract<TInput, TOutput>(string url, TInput data)
		{
			var memoryStream = new MemoryStream();
			new DataContractJsonSerializer(typeof(TInput)).WriteObject(memoryStream, data);
			var json = memoryStream.ToArray();
			memoryStream.Close();
			var content = Encoding.UTF8.GetString(json, 0, json.Length);
			var result = new WebServiceClient().Post(url, content, ApplicationJsonContentType);
			return GetResponse<TOutput>(result);
		}

		public Notification<TOutput> Post<TOutput>(string url)
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

			try
			{
				var ex = JsonUtility.Deserialize<RemoteException>(result.Item);
				if (!ex.Message.IsNullOrEmpty() && !ex.StackTrace.IsNullOrEmpty())
				{
					var notification = new Notification<TOutput>(Notification.ErrorFor("Remote returned Exception: "+ex.Message));
					notification.Add(Notification.InfoFor(ex.StackTrace));
					return notification;
				}
			}
			catch
			{
			}

			TOutput output;
			try
			{
				output = JsonUtility.Deserialize<TOutput>(result.Item);
			}
			catch (Exception exception)
			{
				var notification = new Notification<TOutput>(Notification.ErrorFor("caught exception deserializing:\n" + result.Item + "\n" + exception.Message));
				return notification;
			}
			return output;
		}
	}

	public class RemoteException
	{
		public string Message { get; set; }
		public string StackTrace { get; set; }
	}
}
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
using System.Net;
using System.Text;

namespace MvbaCore.ThirdParty
{
	public interface IWebServiceClient
	{
		Notification<string> Post(string url, string data, string contentType);
		Notification<string> Post(string url, string contentType);
		Notification<string> PostDataContract(string url, string data, string contentType);
	}

	public class WebServiceClient : IWebServiceClient
	{
		private static void AddUserCredentials(WebRequest req)
		{
			req.Credentials = CredentialCache.DefaultCredentials;
		}

		private static HttpWebRequest CreateWebRequest(string url, string contentType)
		{
			var req = (HttpWebRequest)WebRequest.Create(url);
			req.ContentType = contentType;
			return req;
		}

		private static Notification<string> GetResponse(WebRequest req)
		{
			WebResponse response;
			try
			{
				response = req.GetResponse();
			}
			catch (Exception exception)
			{
				var notification = new Notification<string>(Notification.ErrorFor("Remote threw Exception: "+exception.Message));
				notification.Add(Notification.InfoFor(exception.StackTrace));
				return notification;
			}
			var responseStream = response.GetResponseStream();
			if (responseStream == null)
			{
				return new Notification<string>(Notification.ErrorFor("received null response stream"));
			}

			using (var reader = new StreamReader(responseStream))
			{
				var s = reader.ReadToEnd();
				return new Notification<string>
				       {
					       Item = s
				       };
			}
		}

		public Notification<string> Post(string url, string content, string contentType)
		{
			var req = CreateWebRequest(url, contentType);
			req.Method = "POST";
			SendRequest(req, content);
			return GetResponse(req);
		}

		public Notification<string> Post(string url, string contentType)
		{
			var req = CreateWebRequest(url, contentType);
			req.Method = "POST";
			AddUserCredentials(req);
			req.GetRequestStream().Close();
			return GetResponse(req);
		}

		public Notification<string> PostDataContract(string url, string content, string contentType)
		{
			var req = CreateWebRequest(url, contentType);
			req.Method = "POST";
			SendRequest(req, content);
			return GetResponse(req);
		}

		private static void SendRequest(WebRequest req, string content)
		{
			AddUserCredentials(req);
			req.ContentLength = content.Length;

			using (var streamWriter = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
			{
				streamWriter.Write(content);
			}
		}
	}
}
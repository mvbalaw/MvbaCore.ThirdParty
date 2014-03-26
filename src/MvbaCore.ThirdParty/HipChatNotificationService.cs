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
using System.Web;

using MvbaCore.Extensions;
using MvbaCore.ThirdParty.Json;

namespace MvbaCore.ThirdParty
{
	public interface IHipChatNotificationService
	{
		Notification<HipChatResult> Notify(HipChatMessage hipChatMessage);
	}

	public class HipChatNotificationService : IHipChatNotificationService
	{
		private const string ApiUrl = "http://api.hipchat.com/v1/rooms/message?auth_token={0}&format=json";

		public HipChatNotificationService(IWebServiceClient webServiceClient)
		{
			_webServiceClient = webServiceClient;
		}

		private readonly IWebServiceClient _webServiceClient;

		public Notification<HipChatResult> Notify(HipChatMessage hipChatMessage)
		{
			try
			{
				var content = "room_id=" + HttpUtility.UrlEncode(hipChatMessage.RoomId)
					+ "&from=" + HttpUtility.UrlEncode(hipChatMessage.From)
					+ "&message=" + HttpUtility.UrlEncode(hipChatMessage.Message)
					+ "&color=" + hipChatMessage.Color.OrDefault().Key;
				var result = _webServiceClient.Post(String.Format(ApiUrl, hipChatMessage.ApiKey), content, "application/x-www-form-urlencoded");
				return JsonUtility.Deserialize<HipChatResult>(result);
			}
			catch (Exception exception)
			{
				return new Notification<HipChatResult>(Notification.ErrorFor(exception.ToString()));
			}
		}
	}

	public class HipChatResult
	{
		public string status;
	}

	public class HipChatTextColor : NamedConstant<HipChatTextColor>
	{
		private HipChatTextColor(string key)
		{
			base.Add(key, this);
		}

		[DefaultKey]
		public static readonly HipChatTextColor Gray = new HipChatTextColor("gray");
		public static readonly HipChatTextColor Green = new HipChatTextColor("green");
		public static readonly HipChatTextColor Purple = new HipChatTextColor("purple");
		public static readonly HipChatTextColor Red = new HipChatTextColor("red");
		public static readonly HipChatTextColor Yellow = new HipChatTextColor("yellow");
	}

	public class HipChatMessage
	{
		public HipChatTextColor Color { get; set; }
		public string From { get; set; }
		public string Message { get; set; }
		public string RoomId { get; set; }
		public string ApiKey { get; set; }
	}
}
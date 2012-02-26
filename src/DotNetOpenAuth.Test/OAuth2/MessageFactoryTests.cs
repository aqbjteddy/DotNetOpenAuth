﻿//-----------------------------------------------------------------------
// <copyright file="MessageFactoryTests.cs" company="Outercurve Foundation">
//     Copyright (c) Outercurve Foundation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Test.OAuth2 {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OAuth2;
	using DotNetOpenAuth.OAuth2.ChannelElements;
	using DotNetOpenAuth.OAuth2.Messages;
	using Moq;
	using NUnit.Framework;

	/// <summary>
	/// Verifies that the WRAP message types are recognized.
	/// </summary>
	public class MessageFactoryTests : OAuth2TestBase {
		private readonly MessageReceivingEndpoint recipient = new MessageReceivingEndpoint("http://who", HttpDeliveryMethods.PostRequest);
		private OAuth2AuthorizationServerChannel channel;
		private IMessageFactory messageFactory;

		public override void SetUp() {
			base.SetUp();

			this.channel = new OAuth2AuthorizationServerChannel(new Mock<IAuthorizationServer>().Object);
			this.messageFactory = this.channel.MessageFactoryTestHook;
		}

		#region End user authorization messages

		[Test]
		public void EndUserAuthorizationRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.response_type, "code" },
				{ Protocol.client_id, "abc" },
				{ Protocol.redirect_uri, "abc" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(EndUserAuthorizationRequest), request);
		}

		[Test]
		public void EndUserAuthorizationImplicitRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.response_type, "token" },
				{ Protocol.client_id, "abc" },
				{ Protocol.redirect_uri, "abc" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(EndUserAuthorizationImplicitRequest), request);
		}

		[Test]
		public void EndUserAuthorizationSuccessResponseWithCode() {
			var fields = new Dictionary<string, string> {
				{ Protocol.code, "abc" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(EndUserAuthorizationSuccessResponseBase), request);
		}

		[Test]
		public void EndUserAuthorizationSuccessResponseWithAccessToken() {
			var fields = new Dictionary<string, string> {
				{ Protocol.access_token, "abc" },
				{ Protocol.token_type, "bearer" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(EndUserAuthorizationSuccessResponseBase), request);
		}

		[Test]
		public void EndUserAuthorizationFailedResponse() {
			var fields = new Dictionary<string, string> {
				{ Protocol.error, "access-denied" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(EndUserAuthorizationFailedResponse), request);
		}

		#endregion

		#region Access token request messages

		[Test]
		public void AccessTokenRefreshRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.client_id, "abc" },
				{ Protocol.refresh_token, "abc" },
				{ Protocol.grant_type, "refresh-token" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(AccessTokenRefreshRequest), request);
		}

		[Test]
		public void AccessTokenAuthorizationCodeRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.client_id, "abc" },
				{ Protocol.code, "code" },
				{ Protocol.grant_type, "authorization-code" },
				{ Protocol.redirect_uri, "http://someUri" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(AccessTokenAuthorizationCodeRequest), request);
		}

		[Test]
		public void AccessTokenBasicCredentialsRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.client_id, "abc" },
				{ Protocol.client_secret, "abc" },
				{ Protocol.grant_type, "basic-credentials" },
				{ Protocol.username, "abc" },
				{ Protocol.password, "abc" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(AccessTokenResourceOwnerPasswordCredentialsRequest), request);
		}

		[Test]
		public void AccessTokenClientCredentialsRequest() {
			var fields = new Dictionary<string, string> {
				{ Protocol.client_id, "abc" },
				{ Protocol.client_secret, "abc" },
				{ Protocol.grant_type, "none" },
			};
			IDirectedProtocolMessage request = this.messageFactory.GetNewRequestMessage(this.recipient, fields);
			Assert.IsInstanceOf(typeof(AccessTokenClientCredentialsRequest), request);
		}

		#endregion
	}
}

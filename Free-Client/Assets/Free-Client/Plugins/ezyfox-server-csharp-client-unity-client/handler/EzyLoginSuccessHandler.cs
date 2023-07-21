﻿using System;
using com.tvd12.ezyfoxserver.client.entity;

namespace com.tvd12.ezyfoxserver.client.handler
{
	public class EzyLoginSuccessHandler : EzyAbstractDataHandler
	{
		public override void handle(EzyArray data)
		{
			EzyData responseData = data.get<EzyData>(4);
			EzyUser user = newUser(data);
			EzyZone zone = newZone(data);
			((EzyMeAware)client).setMe(user);
			((EzyZoneAware)client).setZone(zone);
            handleLoginSuccess(responseData);
            logger.debug("user: " + user + " logged in successfully");
		}

        protected virtual EzyUser newUser(EzyArray data)
		{
			long userId = data.get<long>(2);
			String username = data.get<String>(3);
			EzySimpleUser user = new EzySimpleUser(userId, username);
			return user;
		}

        protected virtual EzyZone newZone(EzyArray data)
		{
			int zoneId = data.get<int>(0);
			String zoneName = data.get<String>(1);
			EzySimpleZone zone = new EzySimpleZone(client, zoneId, zoneName);
			return zone;
		}

        protected virtual void handleLoginSuccess(EzyData responseData)
		{
		}
	}
}


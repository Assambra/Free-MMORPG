﻿using System;
using com.tvd12.ezyfoxserver.client.handler;

namespace com.tvd12.ezyfoxserver.client.setup
{
	public interface EzyAppSetup
	{
		EzyAppSetup addDataHandler(Object cmd, EzyAppDataHandler dataHandler);

		EzySetup done();

	}
}

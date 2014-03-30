using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace DevelopmentWithADot.HttpCache.Test
{
	public class Global : HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			var cache = HttpRuntime.Cache;
			var enumerator = cache.GetEnumerator() as IDictionaryEnumerator;

			cache["A"] = 1;

			var httpCache = HttpCache.Default;

			var xx = httpCache["A"];


		}
	}
}
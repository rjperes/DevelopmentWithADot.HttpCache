using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;

namespace DevelopmentWithADot.HttpCache
{
	public sealed class HttpCache : ObjectCache
	{
		public static readonly ObjectCache Default = new HttpCache();

		private HttpCache()
		{
		}

		private void Add(String key, Object value, DateTime absoluteExpiration, TimeSpan slidingExpiration, System.Web.Caching.CacheItemPriority priority, CacheItemRemovedCallback callback)
		{
			if (value == null)
			{
				this.Delete(key);
			}
			else
			{
				HttpRuntime.Cache.Add(key, value, null, absoluteExpiration, slidingExpiration, priority, callback);
			}
		}

		private void Set(String key, Object value)
		{
			this.Set(key, value, System.Web.Caching.Cache.NoAbsoluteExpiration);
		}

		private void Set(String key, Object value, DateTime absoluteExpiration)
		{
			if (value == null)
			{
				this.Delete(key);
			}
			else
			{
				HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
			}
		}

		private void Delete(String key)
		{
			HttpRuntime.Cache.Remove(key);
		}

		public override Object AddOrGetExisting(String key, Object value, CacheItemPolicy policy, String regionName = null)
		{
			if (value == null)
			{
				this.Delete(key);
				return (null);
			}
			else
			{
				var item = this.Get(key);

				if (item == null)
				{
					CacheItemRemovedCallback callback = (k, v, r) => policy.RemovedCallback(new CacheEntryRemovedArguments(this, (CacheEntryRemovedReason)(Int32)r, new CacheItem(k, v, regionName)));
					this.Add(key, value, policy.AbsoluteExpiration.LocalDateTime, policy.SlidingExpiration, (System.Web.Caching.CacheItemPriority)(Int32)policy.Priority, callback);
				}

				return (item);
			}
		}

		public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
		{
			var item = this.AddOrGetExisting(value.Key, value.Value, policy, value.RegionName);

			if (item == null)
			{
				return (null);
			}

			return (new CacheItem(value.Key, value.Value, value.RegionName));
		}

		public override Object AddOrGetExisting(String key, Object value, DateTimeOffset absoluteExpiration, String regionName = null)
		{
			return (this.AddOrGetExisting(key, value, new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration }, regionName));
		}

		public override Boolean Contains(String key, String regionName = null)
		{
			return (this.Get(key) != null);
		}

		public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<String> keys, String regionName = null)
		{
			return (null);
		}

		public override DefaultCacheCapabilities DefaultCacheCapabilities
		{
			get
			{
				return (DefaultCacheCapabilities.OutOfProcessProvider | DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.SlidingExpirations | DefaultCacheCapabilities.CacheEntryRemovedCallback);
			}
		}

		public override Object Get(String key, String regionName = null)
		{
			return (HttpRuntime.Cache.Get(key));
		}

		public override CacheItem GetCacheItem(String key, String regionName = null)
		{
			var value = this.Get(key);

			return ((value != null) ? new CacheItem(key, value, regionName) : null);
		}

		public override Int64 GetCount(String regionName = null)
		{
			return (HttpRuntime.Cache.Count);
		}

		protected override IEnumerator<KeyValuePair<String, Object>> GetEnumerator()
		{
			var values = new Dictionary<String, Object>();

			foreach (DictionaryEntry entry in HttpRuntime.Cache)
			{
				values[entry.Key.ToString()] = entry.Value;
			}

			return (values.GetEnumerator());
		}

		public override IDictionary<String, Object> GetValues(IEnumerable<String> keys, String regionName = null)
		{
			var values = new Dictionary<String, object>();
			
			foreach (var key in keys)
			{
				var value = this.Get(key);

				if (value != null)
				{
					values[key] = value;
				}
			}

			return (values);
		}

		public override String Name
		{
			get
			{
				return(this.GetType().Name);
			}
		}

		public override Object Remove(String key, String regionName = null)
		{
			var value = this.Get(key);

			if (value != null)
			{
				this.Delete(key);
			}

			return (value);
		}

		public override void Set(String key, Object value, CacheItemPolicy policy, String regionName = null)
		{
			this.Set(key, value, policy.AbsoluteExpiration, regionName);
		}

		public override void Set(CacheItem item, CacheItemPolicy policy)
		{
			this.Set(item.Key, item.Value, policy.AbsoluteExpiration, item.RegionName);
		}

		public override void Set(String key, Object value, DateTimeOffset absoluteExpiration, String regionName = null)
		{
			this.Set(key, value, absoluteExpiration.LocalDateTime);
		}

		public override Object this[String key]
		{
			get
			{
				return (this.Get(key));
			}
			set
			{
				this.Set(key, value);
			}
		}
	}
}
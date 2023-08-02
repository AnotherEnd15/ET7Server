﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace ET
{
	/// <summary>
    /// Config组件会扫描所有的有ConfigAttribute标签的配置,加载进来
    /// </summary>
    public class ConfigComponent: Singleton<ConfigComponent>
    {
        public struct GetAllConfigBytes
        {
        }
        
        public struct GetOneConfigBytes
        {
            public string ConfigName;
        }
		
        private readonly Dictionary<Type, ISingleton> allConfig = new Dictionary<Type, ISingleton>();

		public override void Dispose()
		{
			foreach (var kv in this.allConfig)
			{
				kv.Value.Destroy();
			}
		}

		public object LoadOneConfig(Type configType)
		{
			this.allConfig.TryGetValue(configType, out ISingleton oneConfig);
			if (oneConfig != null)
			{
				oneConfig.Destroy();
			}
			
			byte[] oneConfigBytes = EventSystem.Instance.Invoke<GetOneConfigBytes, byte[]>(new GetOneConfigBytes() {ConfigName = configType.FullName});

			object category = SerializeHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);
			ISingleton singleton = category as ISingleton;
			singleton.Register();
			
			this.allConfig[configType] = singleton;
			return category;
		}
		
		public void Load()
		{
			this.allConfig.Clear();
			Dictionary<Type, string> configBytes = EventSystem.Instance.Invoke<GetAllConfigBytes, Dictionary<Type,string>>(new GetAllConfigBytes());

			foreach (Type type in configBytes.Keys)
			{
				string oneConfigBytes = configBytes[type];
				this.LoadOneInThread(type, oneConfigBytes);
			}
		}
		
		public async ETTask LoadAsync()
		{
			this.allConfig.Clear();
			Dictionary<Type, string> configBytes = EventSystem.Instance.Invoke<GetAllConfigBytes, Dictionary<Type,string>>(new GetAllConfigBytes());

			using ListComponent<Task> listTasks = ListComponent<Task>.Create();
			
			foreach (Type type in configBytes.Keys)
			{
				var oneConfigBytes = configBytes[type];
				Task task = Task.Run(() => LoadOneInThread(type, oneConfigBytes));
				listTasks.Add(task);
			}

			await Task.WhenAll(listTasks.ToArray());
		}
		
		private void LoadOneInThread(Type configType,string oneConfigBytes)
		{
			object category = MongoHelper.FromJson(configType,oneConfigBytes);
			
			lock (this)
			{
				ISingleton singleton = category as ISingleton;
				singleton.Register();
				this.allConfig[configType] = singleton;
			}
		}
	}
}
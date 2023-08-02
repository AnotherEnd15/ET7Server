using System;
using System.Collections.Generic;
using System.IO;

namespace ET.Server
{
    [Invoke]
    public class GetAllConfigBytes: AInvokeHandler<ConfigComponent.GetAllConfigBytes, Dictionary<Type, string>>
    {
        public override Dictionary<Type, string> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<Type, string> output = new Dictionary<Type, string>();
            List<string> startConfigs = new List<string>()
            {
                "StartMachineConfigCategory", 
                "StartProcessConfigCategory", 
                "StartSceneConfigCategory", 
                "StartZoneConfigCategory",
            };
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof (ConfigAttribute));
            foreach (Type configType in configTypes)
            {
                string configFilePath;
                if (startConfigs.Contains(configType.Name))
                {
                    configFilePath = $"../Config/Json/{Options.Instance.StartConfig}/{configType.Name}.txt";    
                }
                else
                {
                    configFilePath = $"../Config/Json/{configType.Name}.txt";
                }
                output[configType] = File.ReadAllText(configFilePath);
            }

            return output;
        }
    }
    
    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigComponent.GetOneConfigBytes, string>
    {
        public override string Handle(ConfigComponent.GetOneConfigBytes args)
        {
            return File.ReadAllText($"../Config/Json/{args.ConfigName}.txt");
        }
    }
}
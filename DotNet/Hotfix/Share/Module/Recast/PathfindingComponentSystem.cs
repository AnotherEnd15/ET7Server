using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{
    [FriendOf(typeof(PathfindingComponent))]
    public static class PathfindingComponentSystem
    {
        [ObjectSystem]
        public class AwakeSystem: AwakeSystem<PathfindingComponent, string>
        {
            protected override void Awake(PathfindingComponent self, string name)
            {
                self.Name = name;
                self.NavMesh = NavmeshComponent.Instance.Get(name);

                if (self.NavMesh == 0)
                {
                    throw new Exception($"nav load fail: {name}");
                }
            }
        }

        [ObjectSystem]
        public class DestroySystem: DestroySystem<PathfindingComponent>
        {
            protected override void Destroy(PathfindingComponent self)
            {
                self.Name = string.Empty;
                self.NavMesh = 0;
            }
        }
        
        public static void Find(this PathfindingComponent self, Vector3 start, Vector3 target, List<Vector3> result)
        {
            if (self.NavMesh == 0)
            {
                Log.Debug("寻路| Find 失败 pathfinding ptr is zero");
                throw new Exception($"pathfinding ptr is zero: {self.DomainScene().Name}");
            }

            self.StartPos[0] = -start.X;
            self.StartPos[1] = start.Y;
            self.StartPos[2] = start.Z;

            self.EndPos[0] = -target.X;
            self.EndPos[1] = target.Y;
            self.EndPos[2] = target.Z;
            //Log.Debug($"start find path: {self.GetParent<Unit>().Id}");
            int n = Recast.RecastFind(self.NavMesh, PathfindingComponent.extents, self.StartPos, self.EndPos, self.Result);
            for (int i = 0; i < n; ++i)
            {
                int index = i * 3;
                result.Add(new Vector3(-self.Result[index], self.Result[index + 1], self.Result[index + 2]));
            }
            //Log.Debug($"finish find path: {self.GetParent<Unit>().Id} {result.ListToString()}");
        }
        
        public static Vector3 RecastFindNearestPoint(this PathfindingComponent self, Vector3 pos)
        {
            if (self.NavMesh == 0)
            {
                throw new Exception($"pathfinding ptr is zero: {self.DomainScene().Name}");
            }

            self.StartPos[0] = -pos.X;
            self.StartPos[1] = pos.Y;
            self.StartPos[2] = pos.Z;

            int ret = Recast.RecastFindNearestPoint(self.NavMesh, PathfindingComponent.extents, self.StartPos, self.EndPos);
            if (ret == 0)
            {
                throw new Exception($"RecastFindNearestPoint fail, 可能是位置配置有问题: sceneName:{self.DomainScene().Name} {pos} {self.Name} {self.GetParent<Unit>().Id} {self.GetParent<Unit>().Config.Id} {self.EndPos.ArrayToString()}");
            }
            
            return new Vector3(-self.EndPos[0], self.EndPos[1], self.EndPos[2]);
        }
    }
}
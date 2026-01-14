using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Rift.UnityUnmanaged
{
	[BurstCompile]
	public partial struct RiftExecutingRealtimeEditingSystem : ISystem
	{
		private EntityQuery _proxed;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_proxed = new EntityQueryBuilder(Allocator.Temp).WithAll<RiftBlitVariables, RiftBlitByteCode, RiftScriptRealtimeEditing>().Build(ref state);
		}

		private Entity Copy(EntityManager em, Entity e)
		{
			var copy = em.CreateEntity();
			em.AddComponentData(copy, em.GetComponentData<RiftScriptComponent>(e));
			em.AddComponentData(copy, em.GetComponentData<RiftScriptMetaComponent>(e));
			em.AddBuffer<RiftBlitByteCode>(copy).AddRange(em.GetBuffer<RiftBlitByteCode>(e).ToNativeArray(Allocator.Temp));
			em.AddBuffer<RiftBlitVariables>(copy).AddRange(em.GetBuffer<RiftBlitVariables>(e).ToNativeArray(Allocator.Temp));
			em.AddBuffer<RiftBlitSettings>(copy).AddRange(em.GetBuffer<RiftBlitSettings>(e).ToNativeArray(Allocator.Temp));
			em.AddBuffer<RiftEntitySetting>(copy).AddRange(em.GetBuffer<RiftEntitySetting>(e).ToNativeArray(Allocator.Temp));
			return copy;
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
			var scripts = _proxed.ToEntityArray(Allocator.Temp);
			for (int i = 0; i < scripts.Length; i++)
			{
				var e = scripts[i];
				var script = state.EntityManager.GetComponentData<RiftScriptComponent>(e);
				var proxy = state.EntityManager.GetComponentData<RiftScriptRealtimeEditing>(e);
				if (proxy.script.Equals(Entity.Null))
				{
					RiftLog.Log($"Proxy script is null, create new ");
					proxy.script = Copy(state.EntityManager, e);
					state.EntityManager.SetComponentData(e, proxy);
				}
				else if (!script.bakeHash.Equals(state.EntityManager.GetComponentData<RiftScriptComponent>(proxy.script).bakeHash))
				{
					RiftLog.Log($"Proxy script hash mismatch {script.bakeHash} != {state.EntityManager.GetComponentData<RiftScriptComponent>(e).bakeHash}, create new");
					state.EntityManager.AddComponent<RiftScriptInvalidate>(proxy.script);
					proxy.script = Copy(state.EntityManager, e);
					state.EntityManager.SetComponentData(e, proxy);
				}
			}
		}
	}
}
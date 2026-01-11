#if UNITY_2020_1_OR_NEWER
using Unity.Entities;

namespace Rift.UnityUnmanaged
{
	public struct RiftScriptInvalidate : IComponentData
	{
	}

	public struct RiftScriptRealtimeEditing : IComponentData
	{
		public Entity script;
	}

	[InternalBufferCapacity(1024)] //todo research perf
	public struct RiftBlitByteCode : IBufferElementData
	{
		public byte value;
	}

	[InternalBufferCapacity(512)] //todo research perf
	public struct RiftBlitVariables : IBufferElementData
	{
		public byte value;
	}

	[InternalBufferCapacity(1024)] //todo research perf
	public struct RiftBlitSettings : IBufferElementData
	{
		public byte value;
	}

	[InternalBufferCapacity(3)] //todo research perf
	public struct RiftEntitySetting : IBufferElementData
	{
		public Entity value;
		public int offset;
	}

	public struct RiftScriptComponent : IComponentData
	{
		public long bakeHash;
	}

	public struct RiftScriptMetaComponent : IComponentData
	{
		public RiftScriptMeta value;
	}

	public struct RiftScriptExecuting : IComponentData
	{
	}
}
#endif
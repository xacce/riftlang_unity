using Unity.Entities;

namespace Rift.UnityUnmanaged
{
	public struct RiftLocalized
	{
		public ulong table;
		public ulong key;
	}

	public struct RiftScriptInvalidate : IComponentData
	{
	}


	public struct RiftScriptLinkedEntity : ICleanupBufferElementData
	{
		public Entity value;
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
		public byte cleanRequired;
	}

	public struct RiftScriptMetaComponent : IComponentData
	{
		public RiftScriptMeta value;
	}

	public struct RiftScriptExecuting : IComponentData
	{
	}
}
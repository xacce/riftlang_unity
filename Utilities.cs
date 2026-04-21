using System.Runtime.CompilerServices;
using Rift.Externals.Unity;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Rift.UnityUnmanaged
{
	public static class Utilities
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RiftMem Merge(ref DynamicBuffer<byte> settings, in DynamicBuffer<RiftEntitySetting> links)
		{
			var mem = new RiftMem(settings.AsNativeArray());
			for (int i = 0; i < links.Length; i++)
			{
				mem.WriteBlind(links[i].value, links[i].offset);
			}

			return mem;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Update(ref DynamicBuffer<RiftBlitSettings> settings, in DynamicBuffer<RiftEntitySetting> links)
		{
			var settingsRaw = (byte*)settings.Reinterpret<byte>().AsNativeArray().GetUnsafePtr();
			for (int i = 0; i < links.Length; i++)
			{
				*(Entity*)(settingsRaw + links[i].offset) = links[i].value;
			}
		}
	}
}
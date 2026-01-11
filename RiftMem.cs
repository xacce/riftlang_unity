using System;
using Rift;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Rift
{
	public unsafe struct RiftMem : IRiftMem
	{
		private NativeArray<byte> _data;
		private byte* _ptr;
		private int _offset;

		public RiftMem(NativeArray<byte> source)
		{
			_offset = 0;
			_data = source;
			_ptr = (byte*)_data.GetUnsafePtr();
		}

		public unsafe void WriteBlind<T>(T v) where T : unmanaged
		{
			*(T*)((byte*)_ptr + _offset) = v;
			_offset += RiftUtility.SizeOf<T>();
		}

		public unsafe void WriteBlind<T>(T v, int offset) where T : unmanaged
		{
			*(T*)(_ptr + offset) = v;
		}


		public unsafe ref readonly T ReadRoBlit<T>(int offset) where T : unmanaged
		{
			return ref *(T*)((byte*)_ptr + offset);
		}

		public unsafe ref T ReadRwBlit<T>(int offset) where T : unmanaged
		{
			return ref *(T*)((byte*)_ptr + offset);
		}

		public byte* ReadRwRaw(int offset)
		{
			return (byte*)_ptr + offset;
		}

		public void Dispose()
		{
			_data.Dispose();
		}
	}
}
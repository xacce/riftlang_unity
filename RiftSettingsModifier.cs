using System;
using System.Runtime.InteropServices;
using Rift;
using Rift.UnityUnmanaged;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DefaultNamespace
{
	public static class RiftSettingsModifier
	{
		[Serializable]
		public struct RiftSettingBlindValue
		{
			public string fieldName;
			public byte[] value;
		}

		public static void Update(NativeArray<byte> settings, NativeArray<RiftDebugSettingsVariable> debugSettings, RiftSettingBlindValue[] updates)
		{
			for (int i = 0; i < updates.Length; i++)
			{
				var update = updates[i];
				Update(settings, debugSettings, update);
			}
		}

		public static unsafe void Update(NativeArray<byte> settings, NativeArray<RiftDebugSettingsVariable> debugSettings, RiftSettingBlindValue update)
		{
			var settingsPtr = (byte*)settings.GetUnsafePtr();

			for (int j = 0; j < debugSettings.Length; j++)
			{
				if (debugSettings[j].name.Equals(update.fieldName))
				{
					var f = debugSettings[j];
					var offset = f.offset;
					if (update.value.Length != RiftUtility.SizeOf(f.type))
					{
						Debug.LogError($"Invalid size for {update.fieldName} expected {RiftUtility.SizeOf(f.type)} but got {update.value.Length}");
					}

					switch (f.type)
					{
						case RiftCompiledOpCode.Int:
							*(int*)(settingsPtr + offset) = BitConverter.ToInt32(update.value, 0);
							Debug.Log($"Updated {update.fieldName} to {*(int*)(settingsPtr + offset)}");
							break;
						case RiftCompiledOpCode.Float:
							*(float*)(settingsPtr + offset) = BitConverter.ToSingle(update.value, 0);
							Debug.Log($"Updated {update.fieldName} to {*(float*)(settingsPtr + offset)}");
							break;
						case RiftCompiledOpCode.Bool:
							*(bool*)(settingsPtr + offset) = BitConverter.ToBoolean(update.value, 0);
							Debug.Log($"Updated {update.fieldName} to {*(bool*)(settingsPtr + offset)}");
							break;
						case RiftCompiledOpCode.Struct:
							Debug.Log($"Updated {update.fieldName} to Struct");
							Marshal.Copy(update.value, 0, (IntPtr)(settingsPtr + offset), update.value.Length);
							break;
					}

					break;
				}
			}
		}

		public static unsafe void Update<T>(NativeArray<byte> settings, RiftDebugSettingsVariable f, T value) where T : unmanaged
		{
			byte* settingsPtr = (byte*)settings.GetUnsafePtr();
			int offset = f.offset;
			*(T*)(settingsPtr + offset) = value;
#if UNITY_EDITOR
			Debug.Log($"Updated {f.name.Value} to {value}");
#endif
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Rift;
using Rift.Externals.Unity.Authoring;
using Rift.UnityUnmanaged;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rift.Externals.Unity.Authoring
{
	public abstract class RiftScriptSettings : MonoBehaviour
	{
		[Serializable]
		public struct SerializedBlitValue
		{
			public byte[] value;
			public int offset;
			public int length;
			[FormerlySerializedAs("hash")] public ulong typHash;
			public string fieldName;
		}

		[Serializable]
		public struct SerializedGameObjectValue
		{
			public GameObject gameObject;
			public int offset;
			public int length;
			public string fieldName;
			public ulong typHash;
		}

		public RiftScriptSo script;
		[FormerlySerializedAs("settings")] public List<SerializedBlitValue> blit = new List<SerializedBlitValue>();
		public List<SerializedGameObjectValue> gameObjects = new List<SerializedGameObjectValue>();

		public abstract Dictionary<string, RiftCompiler.VariableData> GetSettings();

		public bool Validate()
		{
			var scriptSettings = GetSettings();
			for (int i = 0; i < blit.Count; i++)
			{
				var setting = blit[i];
				if (!scriptSettings.TryGetValue(setting.fieldName, out var cur) || setting.typHash != GetTypeHash(cur.type))
				{
					return false;
				}
			}

			for (int i = 0; i < gameObjects.Count; i++)
			{
				var setting = gameObjects[i];
				if (!scriptSettings.TryGetValue(setting.fieldName, out var cur) || setting.typHash != GetTypeHash(cur.type))
				{
					return false;
				}
			}

			return scriptSettings.Count == blit.Count + gameObjects.Count;
		}

		public unsafe void WriteBlit(NativeArray<byte> buffer)
		{
			foreach (var value in blit)
			{
				NativeArray<byte>.Copy(value.value, 0, buffer, value.offset, value.length);
			}
		}

		public void WriteEntities(IBaker baker, DynamicBuffer<RiftEntitySetting> buffer)
		{
			var sorted = gameObjects.OrderBy(s => s.offset).ToList();
			for (int i = 0; i < sorted.Count; i++)
			{
				var setting = sorted[i];
				buffer.Add(new RiftEntitySetting { value = baker.GetEntity(setting.gameObject, TransformUsageFlags.None), offset = setting.offset });
			}
		}

		public static ulong GetTypeHash(Type type)
		{
			var u2 = xxHash3.Hash64(new FixedString128Bytes(type.Name));
			return ((ulong)u2.y << 32) | u2.x;
		}

		public Dictionary<string, RiftCompiler.VariableData> CleanupAndExtract(SerializedObject serializedObject)
		{
			var scriptSettings = GetSettings();
			HashSet<string> exists = new HashSet<string>();

			SerializedProperty blitArray = serializedObject.FindProperty("blit");
			SerializedProperty gameObjectsArray = serializedObject.FindProperty("gameObjects");

			// Cleanup blit array
			for (int i = blitArray.arraySize - 1; i >= 0; i--)
			{
				SerializedProperty blitElement = blitArray.GetArrayElementAtIndex(i);
				SerializedProperty fieldNameProperty = blitElement.FindPropertyRelative("fieldName");
				SerializedProperty typHashProperty = blitElement.FindPropertyRelative("typHash");
				SerializedProperty offsetProp = blitElement.FindPropertyRelative("offset");

				string fieldName = fieldNameProperty.stringValue;
				var typHash = typHashProperty.ulongValue;
				
				if (!scriptSettings.TryGetValue(fieldName, out var cur) || typHash != GetTypeHash(cur.type))
				{
					blitArray.DeleteArrayElementAtIndex(i);
				}
				else
				{
					offsetProp.intValue = scriptSettings[fieldName].offset;
					exists.Add(fieldName);
				}
			}

			// Cleanup gameObjects array
			for (int i = gameObjectsArray.arraySize - 1; i >= 0; i--)
			{
				SerializedProperty goElement = gameObjectsArray.GetArrayElementAtIndex(i);
				SerializedProperty fieldNameProperty = goElement.FindPropertyRelative("fieldName");
				SerializedProperty typHashProperty = goElement.FindPropertyRelative("typHash");
				SerializedProperty offsetProp = goElement.FindPropertyRelative("offset");
				string fieldName = fieldNameProperty.stringValue;
				ulong typHash = typHashProperty.ulongValue;
				

				if (!scriptSettings.TryGetValue(fieldName, out var cur) || typHash != GetTypeHash(cur.type))
				{
					gameObjectsArray.DeleteArrayElementAtIndex(i);
				}
				else
				{
					offsetProp.intValue = scriptSettings[fieldName].offset;
					exists.Add(fieldName);
				}
			}

			// Add new entries
			foreach (var kv in scriptSettings)
			{
				if (exists.Contains(kv.Key))
				{
					continue;
				}

				Debug.Log($"hashcode {kv.Value.type.ToString()} {kv.Value.type.GetHashCode()}");
				if (kv.Value.type == typeof(Entity))
				{
					int index = gameObjectsArray.arraySize;
					gameObjectsArray.InsertArrayElementAtIndex(index);
					SerializedProperty newElement = gameObjectsArray.GetArrayElementAtIndex(index);

					newElement.FindPropertyRelative("offset").intValue = kv.Value.offset;
					newElement.FindPropertyRelative("length").intValue = kv.Value.size;
					newElement.FindPropertyRelative("fieldName").stringValue = kv.Key;
					var p = Encoding.UTF8.GetBytes(kv.Value.type.Name);
					var s = xxHash3.Hash64(new FixedString128Bytes(kv.Value.type.Name));
					newElement.FindPropertyRelative("typHash").ulongValue = GetTypeHash(kv.Value.type);
					newElement.FindPropertyRelative("gameObject").objectReferenceValue = null;
				}
				else
				{
					int index = blitArray.arraySize;
					blitArray.InsertArrayElementAtIndex(index);
					SerializedProperty newElement = blitArray.GetArrayElementAtIndex(index);

					newElement.FindPropertyRelative("fieldName").stringValue = kv.Key;
					newElement.FindPropertyRelative("typHash").ulongValue = GetTypeHash(kv.Value.type);
					newElement.FindPropertyRelative("offset").intValue = kv.Value.offset;
					newElement.FindPropertyRelative("length").intValue = kv.Value.size;

					SerializedProperty valueProperty = newElement.FindPropertyRelative("value");
					valueProperty.arraySize = kv.Value.size;
					for (int i = 0; i < kv.Value.size; i++)
					{
						valueProperty.GetArrayElementAtIndex(i).intValue = 0;
					}
				}
			}

			serializedObject.ApplyModifiedProperties();

			return scriptSettings;
		}


		public static T GetBlitValue<T>(SerializedObject serializedObject, string fname) where T : unmanaged
		{
			SerializedProperty blitArray = serializedObject.FindProperty("blit");
			for (int i = 0; i < blitArray.arraySize; i++)
			{
				SerializedProperty blitElement = blitArray.GetArrayElementAtIndex(i);
				SerializedProperty fieldNameProperty = blitElement.FindPropertyRelative("fieldName");

				if (fieldNameProperty.stringValue == fname)
				{
					SerializedProperty valueProperty = blitElement.FindPropertyRelative("value");
					int valueSize = UnsafeUtility.SizeOf<T>();

					if (valueProperty.arraySize < valueSize)
					{
						return default(T);
					}

					// Read bytes from serialized property
					byte[] bytes = new byte[valueSize];
					for (int j = 0; j < valueSize; j++)
					{
						bytes[j] = (byte)valueProperty.GetArrayElementAtIndex(j).intValue;
					}

					return MemoryMarshal.Read<T>(bytes);
				}
			}

			return default(T);
		}

		public static unsafe void SetBlitValue<T>(SerializedObject serializedObject, string fname, int offset, T value,bool save=true) where T : unmanaged
		{
			SerializedProperty blitArray = serializedObject.FindProperty("blit");
			for (int i = 0; i < blitArray.arraySize; i++)
			{
				SerializedProperty blitElement = blitArray.GetArrayElementAtIndex(i);
				SerializedProperty fieldNameProperty = blitElement.FindPropertyRelative("fieldName");

				if (fieldNameProperty.stringValue == fname)
				{
					SerializedProperty valueProperty = blitElement.FindPropertyRelative("value");
					SerializedProperty lengthProperty = blitElement.FindPropertyRelative("length");
					SerializedProperty typHashP = blitElement.FindPropertyRelative("typHash");
					SerializedProperty offsetP = blitElement.FindPropertyRelative("offset");

					int valueSize = UnsafeUtility.SizeOf<T>();

					// Ensure the value array is large enough
					if (valueProperty.arraySize < valueSize)
					{
						// Resize the array if needed
						int newSize = Math.Max(lengthProperty.intValue, valueSize);
						valueProperty.arraySize = newSize;
						lengthProperty.intValue = newSize;
					}

					typHashP.ulongValue = GetTypeHash(typeof(T));
					offsetP.intValue = offset;

					// Write the value at the specified offset
					byte[] bytes = new byte[valueSize];
					fixed (byte* ptr = bytes)
					{
						*(T*)ptr = value;
					}

					// Update the byte array in the serialized property
					for (int j = 0; j < valueSize; j++)
					{
						valueProperty.GetArrayElementAtIndex(j).intValue = bytes[j];
					}

					if (save)
					{
						serializedObject.ApplyModifiedProperties();
					}

					return;
				}
			}
		}
		//
		// private unsafe void AddValue<T>(string fname, int offset, T value) where T : unmanaged
		// {
		// 	byte[] bytes = new byte[UnsafeUtility.SizeOf<T>()];
		// 	fixed (byte* ptr = bytes)
		// 	{
		// 		*(T*)ptr = value;
		// 	}
		//
		// 	blit.Add(new SerializedBlitValue { fieldName = fname, value = bytes, typHash = typeof(T).GetHashCode(), offset = offset, length = UnsafeUtility.SizeOf<T>() });
		// 	EditorUtility.SetDirty(this);
		// }
	}
}
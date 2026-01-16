using Rift.UnityUnmanaged;
using Unity.Collections;
using Unity.Entities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Rift.Externals.Unity.Authoring
{
	[CreateAssetMenu(menuName = "Rift/New script")]
	public class RiftScriptSo : ScriptableObject
	{
		[HideInInspector] [SerializeField] private RiftScriptSerializable m_script;
		[TextArea(50, 1000)] [SerializeField] public string code;
#if UNITY_EDITOR
		public RiftCompiler Bake(IBaker baker, RiftScriptSettings settings, Entity t, IRiftEnvironment environment)
		{
			var compiler = Compile(environment);
			baker.AddBuffer<RiftBlitByteCode>(t).Reinterpret<byte>().AddRange(new NativeArray<byte>(m_script.bytecode, Allocator.Temp));
			baker.AddBuffer<RiftBlitVariables>(t).Reinterpret<byte>().AddRange(new NativeArray<byte>(m_script.meta.variablesSize, Allocator.Temp));
			var blitSettingsBuffer = baker.AddBuffer<RiftBlitSettings>(t).Reinterpret<byte>();
			var gameObjectBuffer = baker.AddBuffer<RiftEntitySetting>(t);
			blitSettingsBuffer.ResizeUninitialized(m_script.meta.settingsSize);
			settings.WriteBlit(blitSettingsBuffer.AsNativeArray());
			settings.WriteEntities(baker, gameObjectBuffer);

			baker.AddComponent(t, new RiftScriptMetaComponent { value = m_script.meta });
			return compiler;
		}

		public RiftCompiler Compile(IRiftEnvironment environment)
		{
			var tokens = new RiftTokenizer().Tokenize(code);
			var parsed = new RiftParser().Parse(tokens);
			var compiler = new RiftCompiler(environment);
			// compiler.RegisterStruct<LocalTransform>("LocalTransform");
			// compiler.RegisterStruct<float3>("float3");
			var script = compiler.Compile(parsed);
			m_script = script;
			EditorUtility.SetDirty(this);
			return compiler;
		}
#endif
	}
}
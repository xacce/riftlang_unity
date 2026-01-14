using Rift;
using Rift.Externals.Unity.Authoring;
using Rift.Externals.Unity.Authoring.Editor;
using Unity.Entities;
using UnityEditor;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	public abstract class RiftEditorByteableField
	{
		protected readonly RiftCompiler.VariableData data;
		protected readonly RiftScriptSettings so;

		protected RiftEditorByteableField(RiftScriptSettings so, RiftCompiler.VariableData data)
		{
			this.data = data;
			this.so = so;
		}

		public abstract VisualElement Render(SerializedObject so);
	}
}
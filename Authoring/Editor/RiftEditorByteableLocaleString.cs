using Rift.UnityUnmanaged;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	public class RiftEditorByteableLocaleString : RiftEditorByteableField
	{
		private LocalTransform _c;
		private readonly RiftScriptSettingsEditor _editor;

		public RiftEditorByteableLocaleString(RiftScriptSettingsEditor editor, RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
		{
			_editor = editor;
		}

		public override VisualElement Render(SerializedObject so)
		{
			// Main container with styling
			var root = new VisualElement();
			root.style.marginTop = 5;
			root.style.marginBottom = 5;
			root.style.paddingLeft = 10;
			root.style.paddingRight = 10;
			root.style.paddingTop = 8;
			root.style.paddingBottom = 8;
			root.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);
			root.style.borderBottomLeftRadius = 4;
			root.style.borderBottomRightRadius = 4;
			root.style.borderTopLeftRadius = 4;
			root.style.borderTopRightRadius = 4;

			// Field
			var serialziedValue = RiftScriptSettings.GetBlitValue<RiftLocalized>(so, data.name);
			// field.RegisterValueChangedCallback(evt => { RiftScriptSettings.SetBlitValue(so, data.name, data.offset, evt.newValue); });
			// root.Add(field);
			return root;
		}
	}
}
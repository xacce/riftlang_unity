using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	public class RiftEditorByteableToggleField : RiftEditorByteableField
	{
		public RiftEditorByteableToggleField(RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
		{
		}

		public override VisualElement Render(SerializedObject so)
		{
			// Main container with styling
			var root = new VisualElement();
			root.style.marginTop = 3;
			root.style.marginBottom = 3;
			root.style.paddingLeft = 10;
			root.style.paddingRight = 10;
			root.style.paddingTop = 6;
			root.style.paddingBottom = 6;
			root.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);
			root.style.borderBottomLeftRadius = 4;
			root.style.borderBottomRightRadius = 4;
			root.style.borderTopLeftRadius = 4;
			root.style.borderTopRightRadius = 4;

			// Toggle field with label
			var field = new Toggle(data.name)
			{
				value = RiftScriptSettings.GetBlitValue<bool>(so, data.name)
			};
			field.style.marginLeft = 8;
			field.RegisterValueChangedCallback(evt => { RiftScriptSettings.SetBlitValue(so, data.name, data.offset, evt.newValue); });
			root.Add(field);

			return root;
		}
	}
}
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	public class RiftEditorByteableUIntField : RiftEditorByteableField
	{
		public RiftEditorByteableUIntField(RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
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

			// Label
			var label = new Label(data.name);
			label.style.fontSize = 11;
			label.style.marginBottom = 3;
			label.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			root.Add(label);

			// Field
			var field = new UnsignedIntegerField()
			{
				value = RiftScriptSettings.GetBlitValue<uint>(so, data.name)
			};
			field.style.marginLeft = 8;
			field.RegisterValueChangedCallback(evt => { RiftScriptSettings.SetBlitValue<uint>(so, data.name, data.offset, evt.newValue); });
			root.Add(field);

			return root;
		}
	}
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	public class RiftEditorByteableLocalTransformField : RiftEditorByteableField
	{
		private LocalTransform _c;
		private bool _posEditing = false;
		private bool _rotEditing = false;
		private bool _scaleEditing = false;
		private readonly RiftScriptSettingsEditor _editor;

		public RiftEditorByteableLocalTransformField(RiftScriptSettingsEditor editor, RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
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

			// Header label
			var header = new Label(data.name);
			header.style.unityFontStyleAndWeight = FontStyle.Bold;
			header.style.fontSize = 12;
			header.style.marginBottom = 6;
			header.style.color = new Color(0.8f, 0.9f, 1f, 1f);
			root.Add(header);

			// Get current value
			var value = RiftScriptSettings.GetBlitValue<LocalTransform>(so, data.name);
			_c = new LocalTransform { Position = value.Position, Rotation = value.Rotation, Scale = value.Scale };
			if (_c.Rotation.value.Equals(float4.zero))
			{
				_c.Rotation = quaternion.identity;
			}

			if (_c.Scale == 0)
			{
				_c.Scale = 1f;
			}
			// Position field with label
			var posContainer = new VisualElement();
			posContainer.style.marginBottom = 4;
			var posLabel = new Label("Position");
			posLabel.style.fontSize = 11;
			posLabel.style.marginBottom = 2;
			posLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			posContainer.Add(posLabel);

			var pos = new Vector3Field()
			{
				value = _c.Position
			};
			pos.style.marginLeft = 8;
			posContainer.Add(pos);
			root.Add(posContainer);

			// Rotation field with label
			var rotContainer = new VisualElement();
			rotContainer.style.marginBottom = 4;
			var rotLabel = new Label("Rotation");
			rotLabel.style.fontSize = 11;
			rotLabel.style.marginBottom = 2;
			rotLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			rotContainer.Add(rotLabel);


			var rot = new Vector3Field()
			{
				value = math.degrees(Quaternion.ToEulerAngles(_c.Rotation))
			};
			rot.style.marginLeft = 8;
			rotContainer.Add(rot);
			root.Add(rotContainer);

			// Scale field with label
			var scaleContainer = new VisualElement();
			var scaleLabel = new Label("Scale");
			scaleLabel.style.fontSize = 11;
			scaleLabel.style.marginBottom = 2;
			scaleLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			scaleContainer.Add(scaleLabel);

			var scale = new FloatField()
			{
				value = _c.Scale
			};
			scale.style.marginLeft = 8;
			scaleContainer.Add(scale);
			root.Add(scaleContainer);

			pos.RegisterCallback<FocusInEvent>(evt =>
			{
				_editor.ClearHandles();
				Debug.Log($"Position editing started");
				_posEditing = true;
			});
			rot.RegisterCallback<FocusInEvent>(evt =>
			{
				_editor.ClearHandles();
				_rotEditing = true;
			});
			scale.RegisterCallback<FocusInEvent>(evt =>
			{
				_editor.ClearHandles();
				_scaleEditing = true;
			});
			// Register callbacks
			pos.RegisterValueChangedCallback(evt =>
			{
				_c.Position = evt.newValue;
				Apply(so);
			});
			rot.RegisterValueChangedCallback(evt =>
			{
				_c.Rotation = Quaternion.Euler(evt.newValue);
				Apply(so);
			});
			scale.RegisterValueChangedCallback(evt =>
			{
				_c.Scale = evt.newValue;
				Apply(so);
			});

			return root;
		}

		public void ClearHandles()
		{
			_posEditing = false;
			_rotEditing = false;
			_scaleEditing = false;
		}

		public void Apply(SerializedObject so, bool save = true)
		{
			RiftScriptSettings.SetBlitValue(so, data.name, data.offset, _c, save);
		}

		public void DrawSceneHandles(SerializedObject so)
		{
			Handles.DrawWireCube(_c.Position + 0.5f, Vector3.one);
			Handles.Label(_c.Position, data.name);
			if (_posEditing)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 newPosition = Handles.PositionHandle(_c.Position, _c.Rotation);
				if (EditorGUI.EndChangeCheck())
				{
					Debug.Log($"Position changed {newPosition}");
					_c.Position = newPosition;
				}
			}
		}
	}
}
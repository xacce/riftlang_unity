using Rift;
using Rift.Externals.Unity.Authoring;
using Rift.Externals.Unity.Authoring.Editor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
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
		public abstract void Write(DynamicBuffer<byte> buffer);
	}

	public class RiftEditorByteableIntField : RiftEditorByteableField
	{
		public RiftEditorByteableIntField(RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
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
			var field = new IntegerField()
			{
				value = RiftScriptSettings.GetBlitValue<int>(so, data.name)
			};
			field.style.marginLeft = 8;
			field.RegisterValueChangedCallback(evt => { RiftScriptSettings.SetBlitValue<int>(so, data.name, data.offset, evt.newValue); });
			root.Add(field);

			return root;
		}

		public override void Write(DynamicBuffer<byte> buffer)
		{
			throw new System.NotImplementedException();
		}
	}

	public class RiftEditorByteableFloatField : RiftEditorByteableField
	{
		public RiftEditorByteableFloatField(RiftScriptSettings so, RiftCompiler.VariableData data) : base(so, data)
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
			var field = new FloatField()
			{
				value = RiftScriptSettings.GetBlitValue<float>(so, data.name)
			};
			field.style.marginLeft = 8;
			field.RegisterValueChangedCallback(evt => { RiftScriptSettings.SetBlitValue(so, data.name, data.offset, evt.newValue); });
			root.Add(field);

			return root;
		}

		public override void Write(DynamicBuffer<byte> buffer)
		{
			throw new System.NotImplementedException();
		}
	}

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

		public override void Write(DynamicBuffer<byte> buffer)
		{
			throw new System.NotImplementedException();
		}
	}

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

		// public void SetPosition(Vector3 position)
		// {
		// 	_c.Position = position;
		// 	Apply();
		// }
		//
		// public void SetRotation(Quaternion rotation)
		// {
		// 	_c.Rotation = rotation;
		// 	Apply();
		// }
		//
		// public void SetScale(float scale)
		// {
		// 	_c.Scale = scale;
		// 	Apply();
		// }
		//
		// public LocalTransform GetTransform()
		// {
		// 	return _c;
		// }

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
				value = value.Position
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
				value = math.degrees(Quaternion.ToEulerAngles(value.Rotation))
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
				value = value.Scale
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

		public void Apply(SerializedObject so,bool save=true)
		{
			RiftScriptSettings.SetBlitValue(so, data.name, data.offset, _c,save);
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
			// Refresh current value
			// _c = RiftScriptSettings.GetBlitValue<LocalTransform>(so, data.name);
			//
			// // Draw position handle
			// EditorGUI.BeginChangeCheck();
			// Vector3 newPosition = Handles.PositionHandle(_c.Position, _c.Rotation);
			// if (EditorGUI.EndChangeCheck())
			// {
			// 	Debug.Log($"Position changed {newPosition}");
			// 	SetPosition(newPosition);
			// }
			//
			// // Draw rotation handle
			// EditorGUI.BeginChangeCheck();
			// Quaternion newRotation = Handles.RotationHandle(_c.Rotation, _c.Position);
			// if (EditorGUI.EndChangeCheck())
			// {
			// 	SetRotation(newRotation);
			// }
			//
			// // Draw scale handle
			// EditorGUI.BeginChangeCheck();
			// Vector3 scaleVector = Vector3.one * _c.Scale;
			// Vector3 newScaleVector = Handles.ScaleHandle(scaleVector, _c.Position, _c.Rotation, HandleUtility.GetHandleSize(_c.Position));
			// if (EditorGUI.EndChangeCheck())
			// {
			// 	SetScale(newScaleVector.x);
			// }
			//
			// // Draw label
			// Vector3 labelOffset = new Vector3(0, HandleUtility.GetHandleSize(_c.Position) * 0.5f, 0);
			// Handles.Label((Vector3)_c.Position + labelOffset, data.name);
		}

		public override void Write(DynamicBuffer<byte> buffer)
		{
			throw new System.NotImplementedException();
		}
	}
}
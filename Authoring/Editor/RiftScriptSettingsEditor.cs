#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Transforms;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Rift.Externals.Unity.Authoring.Editor
{
	[CustomEditor(typeof(RiftScriptSettings), true)]
	public class RiftScriptSettingsEditor : UnityEditor.Editor
	{
		private VisualElement _fields;
		private bool _extracted = false;
		private List<RiftEditorByteableLocalTransformField> _transformFields = new List<RiftEditorByteableLocalTransformField>();

		public override VisualElement CreateInspectorGUI()
		{
			_extracted = false;
			var root = new VisualElement();
			_fields = new VisualElement();
			var o = new ObjectField("Script") { objectType = typeof(RiftScriptSo), value = ((RiftScriptSettings)target).script };
			o.BindProperty(serializedObject.FindProperty("script"));
			root.Add(o);
			var btn = new Button(ClickEvent) { text = "Extract settings" };
			root.Add(btn);
			root.Add(_fields);
			return root;
		}

		private void OnDisable()
		{
			//force save
			Debug.Log("OnDisable");
			foreach (var transformField in _transformFields)
			{
				transformField.Apply(serializedObject, false);
				transformField.ClearHandles();
			}
			serializedObject.ApplyModifiedProperties();
		}

		private void ClickEvent()
		{
			_fields.Clear();
			_transformFields.Clear();
			var goList = new ListView()
			{
				showFoldoutHeader = true,
				headerTitle = "Game objects",
				reorderable = false,
				virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
			};
			var script = target as RiftScriptSettings;
			var goProp = serializedObject.FindProperty("gameObjects");
			goList.BindProperty(goProp);
			_fields.Add(goList);
			var settings = script.CleanupAndExtract(serializedObject);
			foreach (var pair in settings)
			{
				switch (pair.Value.opType)
				{
					case RiftCompiledOpCode.Struct:
						if (pair.Value.type == typeof(LocalTransform))
						{
							var transformField = new RiftEditorByteableLocalTransformField(this, script, pair.Value);
							_transformFields.Add(transformField);
							_fields.Add(transformField.Render(serializedObject));
						}

						break;
					case RiftCompiledOpCode.Int:
						_fields.Add(new RiftEditorByteableIntField(script, pair.Value).Render(serializedObject));
						break;
					case RiftCompiledOpCode.Float:
						_fields.Add(new RiftEditorByteableFloatField(script, pair.Value).Render(serializedObject));
						break;
					case RiftCompiledOpCode.Bool:
						_fields.Add(new RiftEditorByteableToggleField(script, pair.Value).Render(serializedObject));
						break;
				}
			}

			_fields.Add(new Label($"Stored blit fields count: {script.blit.Count}"));
			_fields.Add(new Label($"Stored game object fields count: {script.gameObjects.Count}"));
			_extracted = true;
			SceneView.RepaintAll();
		}

		public void ClearHandles()
		{
			if (!_extracted || _transformFields.Count == 0)
				return;
			foreach (var transformField in _transformFields)
			{
				transformField.Apply(serializedObject, false);
				transformField.ClearHandles();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void OnSceneGUI()
		{
			if (!_extracted || _transformFields.Count == 0)
				return;

			var script = target as RiftScriptSettings;
			if (script == null)
				return;
			int pi = 0;
			int ri = 0;
			int si = 0;
			foreach (var transformField in _transformFields)
			{
				transformField.DrawSceneHandles(serializedObject);
			}
		}
	}
}
#endif
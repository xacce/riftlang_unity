#if UNITY_EDITOR
using Rift.Externals.Unity.Authoring;
using Rift.UnityUnmanaged;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Rift.UnityUnmanaged
{
	[ScriptedImporter(1, "rift")]
	public class RiftImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var txt = System.IO.File.ReadAllText(ctx.assetPath);
			var asset = ScriptableObject.CreateInstance<RiftScriptSo>();
			asset.code = txt;
			ctx.AddObjectToAsset("main obj", asset);
			ctx.SetMainObject(asset);
		}
	}
}
#endif
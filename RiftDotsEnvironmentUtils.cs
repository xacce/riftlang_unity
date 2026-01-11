using Rift;
using Unity.Entities;
using Unity.Mathematics;

namespace Rift.Externals.Unity
{
	public static class RiftDotsEnvironmentUtils
	{
		public static void SetupDotsTypes(IRiftCompiler compiler, bool onlyEnvironment)
		{
			var types = TypeManager.AllTypes;
			foreach (var type in types)
			{
				if (type.BakingOnlyType || type.TemporaryBakingType || type.IsZeroSized || type.HasBlobAssetRefs || type.HasWeakAssetRefs || type.HasUnityObjectRefs)
				{
					continue;
				}

				if (!RiftUtility.IsStructureType(type.Type))
				{
					continue;
				}

				compiler.RegisterEnvironmentDataType(type.StableTypeHash, type.Type.Name, RiftUtility.SizeOf(type.Type));
				if (!onlyEnvironment)
				{
					compiler.RegisterStructRecursive(type.Type);
				}
			}
		}

		public static void SetupMathTypes(IRiftCompiler compiler)
		{
			compiler.RegisterStructRecursive(typeof(int3));
			compiler.RegisterStructRecursive(typeof(float3));
			compiler.RegisterStructRecursive(typeof(float2));
			compiler.RegisterStructRecursive(typeof(int2));
		}
	}
}
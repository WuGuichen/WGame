using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"AIPlugins.dll",
		"Animancer.dll",
		"Antlr4.Runtime.dll",
		"CrashKonijn.Goap.dll",
		"DOTween.dll",
		"DesperateDevs.Caching.dll",
		"Entitas.dll",
		"Sirenix.Serialization.dll",
		"Sirenix.Utilities.dll",
		"System.Core.dll",
		"System.dll",
		"Unity.InputSystem.dll",
		"Unity.Netcode.Runtime.dll",
		"Unity.RenderPipelines.Core.Runtime.dll",
		"UnityEngine.CoreModule.dll",
		"WGame.Runtime.dll",
		"WGame.Utils.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Animancer.ICopyable<object>
	// Animancer.MixerState<UnityEngine.Vector2>
	// Antlr4.Runtime.IAntlrErrorListener<int>
	// Antlr4.Runtime.IAntlrErrorListener<object>
	// Antlr4.Runtime.ProxyErrorListener<int>
	// Antlr4.Runtime.ProxyErrorListener<object>
	// Antlr4.Runtime.Recognizer<int,object>
	// Antlr4.Runtime.Recognizer<object,object>
	// Antlr4.Runtime.Tree.AbstractParseTreeVisitor<Symbol>
	// Antlr4.Runtime.Tree.AbstractParseTreeVisitor<object>
	// Antlr4.Runtime.Tree.IParseTreeVisitor<Symbol>
	// Antlr4.Runtime.Tree.IParseTreeVisitor<object>
	// CrashKonijn.Goap.Behaviours.ActionBase<object>
	// CrashKonijn.Goap.Classes.Builders.KeyBuilderBase<object>
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector2>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector2>
	// DesperateDevs.Caching.ObjectPool<object>
	// Entitas.AbstractEntityIndex<object,int>
	// Entitas.Collector<object>
	// Entitas.Context.<>c<object>
	// Entitas.Context<object>
	// Entitas.Group<object>
	// Entitas.GroupChanged<object>
	// Entitas.GroupSingleEntityException.<>c<object>
	// Entitas.GroupSingleEntityException<object>
	// Entitas.GroupUpdated<object>
	// Entitas.IAllOfMatcher<object>
	// Entitas.ICollector<object>
	// Entitas.IContext<object>
	// Entitas.IGroup<object>
	// Entitas.IMatcher<object>
	// Entitas.Matcher<object>
	// Entitas.MultiReactiveSystem<object,object>
	// Entitas.PrimaryEntityIndex<object,int>
	// Entitas.ReactiveSystem<object>
	// Entitas.TriggerOnEvent<object>
	// InstanceDB<object>
	// Sirenix.Serialization.Serializer<object>
	// Sirenix.Serialization.Utilities.Cache<object>
	// SparseSet<object>
	// System.Action<BoundsOctreeNode.OctreeObject<object>>
	// System.Action<HitInfo>
	// System.Action<PlayerRoomInfo>
	// System.Action<Symbol>
	// System.Action<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Action<System.ValueTuple<int,int>>
	// System.Action<TWY.Physics.CapsuleF>
	// System.Action<Unity.Mathematics.float4>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.Matrix4x4>
	// System.Action<UnityEngine.Vector3>
	// System.Action<WLangImporter>
	// System.Action<byte>
	// System.Action<int>
	// System.Action<object,int>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ulong>
	// System.ArraySegment.Enumerator<UnityEngine.Vector2>
	// System.ArraySegment.Enumerator<UnityEngine.Vector3>
	// System.ArraySegment.Enumerator<int>
	// System.ArraySegment<UnityEngine.Vector2>
	// System.ArraySegment<UnityEngine.Vector3>
	// System.ArraySegment<int>
	// System.ByReference<UnityEngine.Vector2>
	// System.ByReference<UnityEngine.Vector3>
	// System.ByReference<int>
	// System.Collections.Generic.ArraySortHelper<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.ArraySortHelper<HitInfo>
	// System.Collections.Generic.ArraySortHelper<PlayerRoomInfo>
	// System.Collections.Generic.ArraySortHelper<Symbol>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<int,int>>
	// System.Collections.Generic.ArraySortHelper<TWY.Physics.CapsuleF>
	// System.Collections.Generic.ArraySortHelper<Unity.Mathematics.float4>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<WLangImporter>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.Comparer<HitInfo>
	// System.Collections.Generic.Comparer<PlayerRoomInfo>
	// System.Collections.Generic.Comparer<Symbol>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.Comparer<TWY.Physics.CapsuleF>
	// System.Collections.Generic.Comparer<Unity.Mathematics.float4>
	// System.Collections.Generic.Comparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<WLangImporter>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary.Enumerator<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Symbol>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Symbol>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Symbol>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Symbol>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Symbol>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,object>
	// System.Collections.Generic.Dictionary<int,AbilityTriggerInfo>
	// System.Collections.Generic.Dictionary<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.Dictionary<int,float>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,Symbol>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<ulong,PlayerRoomInfo>
	// System.Collections.Generic.Dictionary<ulong,object>
	// System.Collections.Generic.EqualityComparer<AbilityTriggerInfo>
	// System.Collections.Generic.EqualityComparer<HatePointInfo.HateInfo>
	// System.Collections.Generic.EqualityComparer<PlayerRoomInfo>
	// System.Collections.Generic.EqualityComparer<Symbol>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<ulong>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.ICollection<HitInfo>
	// System.Collections.Generic.ICollection<PlayerRoomInfo>
	// System.Collections.Generic.ICollection<Symbol>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,AbilityTriggerInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,HatePointInfo.HateInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Symbol>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,PlayerRoomInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<int,int>>
	// System.Collections.Generic.ICollection<TWY.Physics.CapsuleF>
	// System.Collections.Generic.ICollection<Unity.Mathematics.float4>
	// System.Collections.Generic.ICollection<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<WLangImporter>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.IComparer<HitInfo>
	// System.Collections.Generic.IComparer<PlayerRoomInfo>
	// System.Collections.Generic.IComparer<Symbol>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.IComparer<TWY.Physics.CapsuleF>
	// System.Collections.Generic.IComparer<Unity.Mathematics.float4>
	// System.Collections.Generic.IComparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<WLangImporter>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<float>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,int>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.IEnumerable<HitInfo>
	// System.Collections.Generic.IEnumerable<PlayerRoomInfo>
	// System.Collections.Generic.IEnumerable<Symbol>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,AbilityTriggerInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,HatePointInfo.HateInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Symbol>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,PlayerRoomInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<int,int>>
	// System.Collections.Generic.IEnumerable<TWY.Physics.CapsuleF>
	// System.Collections.Generic.IEnumerable<Unity.Mathematics.float4>
	// System.Collections.Generic.IEnumerable<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<WLangImporter>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.IEnumerator<HitInfo>
	// System.Collections.Generic.IEnumerator<PlayerRoomInfo>
	// System.Collections.Generic.IEnumerator<Symbol>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,AbilityTriggerInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,HatePointInfo.HateInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Symbol>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,PlayerRoomInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<int,int>>
	// System.Collections.Generic.IEnumerator<TWY.Physics.CapsuleF>
	// System.Collections.Generic.IEnumerator<Unity.Mathematics.float4>
	// System.Collections.Generic.IEnumerator<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<WLangImporter>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<ulong>
	// System.Collections.Generic.IList<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.IList<HitInfo>
	// System.Collections.Generic.IList<PlayerRoomInfo>
	// System.Collections.Generic.IList<Symbol>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IList<System.ValueTuple<int,int>>
	// System.Collections.Generic.IList<TWY.Physics.CapsuleF>
	// System.Collections.Generic.IList<Unity.Mathematics.float4>
	// System.Collections.Generic.IList<UnityEngine.Matrix4x4>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<WLangImporter>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,AbilityTriggerInfo>
	// System.Collections.Generic.KeyValuePair<int,HatePointInfo.HateInfo>
	// System.Collections.Generic.KeyValuePair<int,float>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,Symbol>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<ulong,PlayerRoomInfo>
	// System.Collections.Generic.KeyValuePair<ulong,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.List.Enumerator<HitInfo>
	// System.Collections.Generic.List.Enumerator<PlayerRoomInfo>
	// System.Collections.Generic.List.Enumerator<Symbol>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<int,int>>
	// System.Collections.Generic.List.Enumerator<TWY.Physics.CapsuleF>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float4>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Matrix4x4>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<WLangImporter>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.List<HitInfo>
	// System.Collections.Generic.List<PlayerRoomInfo>
	// System.Collections.Generic.List<Symbol>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.List<System.ValueTuple<int,int>>
	// System.Collections.Generic.List<TWY.Physics.CapsuleF>
	// System.Collections.Generic.List<Unity.Mathematics.float4>
	// System.Collections.Generic.List<UnityEngine.Matrix4x4>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<WLangImporter>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.Generic.ObjectComparer<HitInfo>
	// System.Collections.Generic.ObjectComparer<PlayerRoomInfo>
	// System.Collections.Generic.ObjectComparer<Symbol>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<int,int>>
	// System.Collections.Generic.ObjectComparer<TWY.Physics.CapsuleF>
	// System.Collections.Generic.ObjectComparer<Unity.Mathematics.float4>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Matrix4x4>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<WLangImporter>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<AbilityTriggerInfo>
	// System.Collections.Generic.ObjectEqualityComparer<HatePointInfo.HateInfo>
	// System.Collections.Generic.ObjectEqualityComparer<PlayerRoomInfo>
	// System.Collections.Generic.ObjectEqualityComparer<Symbol>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<ulong>
	// System.Collections.Generic.Queue.Enumerator<HatePointInfo.HateInfo>
	// System.Collections.Generic.Queue<HatePointInfo.HateInfo>
	// System.Collections.Generic.Stack.Enumerator<int>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<int>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<BoundsOctreeNode.OctreeObject<object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<HitInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<PlayerRoomInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<Symbol>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<int,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<TWY.Physics.CapsuleF>
	// System.Collections.ObjectModel.ReadOnlyCollection<Unity.Mathematics.float4>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Matrix4x4>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<WLangImporter>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<BoundsOctreeNode.OctreeObject<object>>
	// System.Comparison<HitInfo>
	// System.Comparison<PlayerRoomInfo>
	// System.Comparison<Symbol>
	// System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Comparison<System.ValueTuple<int,int>>
	// System.Comparison<TWY.Physics.CapsuleF>
	// System.Comparison<Unity.Mathematics.float4>
	// System.Comparison<UnityEngine.Matrix4x4>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<WLangImporter>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
	// System.Func<WGame.Trigger.WTrigger.Context,byte>
	// System.Func<int,byte>
	// System.Func<int,object>
	// System.Func<int>
	// System.Func<object,UnityEngine.Vector2>
	// System.Func<object,byte>
	// System.Func<object,float>
	// System.Func<object,int,int,object>
	// System.Func<object,int>
	// System.Func<object,object,System.UIntPtr>
	// System.Func<object,object,int>
	// System.Func<object,object>
	// System.Func<object>
	// System.IComparable<object>
	// System.IEquatable<UnityEngine.Quaternion>
	// System.IEquatable<UnityEngine.Vector3>
	// System.IEquatable<byte>
	// System.IEquatable<float>
	// System.IEquatable<object>
	// System.Lazy<object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<CastIterator>d__99<object>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<int,object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<int,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<int,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.EnumerableSorter<object,float>
	// System.Linq.EnumerableSorter<object>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
	// System.Linq.OrderedEnumerable<object,float>
	// System.Linq.OrderedEnumerable<object>
	// System.Nullable<byte>
	// System.Nullable<double>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Nullable<long>
	// System.Nullable<short>
	// System.Predicate<BoundsOctreeNode.OctreeObject<object>>
	// System.Predicate<HitInfo>
	// System.Predicate<PlayerRoomInfo>
	// System.Predicate<Symbol>
	// System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Predicate<System.ValueTuple<int,int>>
	// System.Predicate<TWY.Physics.CapsuleF>
	// System.Predicate<Unity.Mathematics.float4>
	// System.Predicate<UnityEngine.InputSystem.InputBinding>
	// System.Predicate<UnityEngine.InputSystem.InputControlScheme>
	// System.Predicate<UnityEngine.Matrix4x4>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<WLangImporter>
	// System.Predicate<byte>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector2>
	// System.ReadOnlySpan.Enumerator<UnityEngine.Vector3>
	// System.ReadOnlySpan.Enumerator<int>
	// System.ReadOnlySpan<UnityEngine.Vector2>
	// System.ReadOnlySpan<UnityEngine.Vector3>
	// System.ReadOnlySpan<int>
	// System.Runtime.CompilerServices.ConditionalWeakTable.CreateValueCallback<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable.Enumerator<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable<object,object>
	// System.Span.Enumerator<UnityEngine.Vector2>
	// System.Span.Enumerator<UnityEngine.Vector3>
	// System.Span.Enumerator<int>
	// System.Span<UnityEngine.Vector2>
	// System.Span<UnityEngine.Vector3>
	// System.Span<int>
	// System.ValueTuple<float,float,float>
	// System.ValueTuple<int,byte>
	// System.ValueTuple<int,int>
	// System.ValueTuple<object,int>
	// Unity.Collections.NativeArray.Enumerator<UnityEngine.Vector2>
	// Unity.Collections.NativeArray.Enumerator<UnityEngine.Vector3>
	// Unity.Collections.NativeArray.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<UnityEngine.Vector2>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<UnityEngine.Vector3>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly<UnityEngine.Vector2>
	// Unity.Collections.NativeArray.ReadOnly<UnityEngine.Vector3>
	// Unity.Collections.NativeArray.ReadOnly<int>
	// Unity.Collections.NativeArray<UnityEngine.Vector2>
	// Unity.Collections.NativeArray<UnityEngine.Vector3>
	// Unity.Collections.NativeArray<int>
	// Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerReader>
	// Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerWriter>
	// Unity.Netcode.BufferSerializer<object>
	// Unity.Netcode.FallbackSerializer<UnityEngine.Quaternion>
	// Unity.Netcode.FallbackSerializer<UnityEngine.Vector3>
	// Unity.Netcode.FallbackSerializer<byte>
	// Unity.Netcode.FallbackSerializer<float>
	// Unity.Netcode.FallbackSerializer<int>
	// Unity.Netcode.INetworkVariableSerializer<UnityEngine.Quaternion>
	// Unity.Netcode.INetworkVariableSerializer<UnityEngine.Vector3>
	// Unity.Netcode.INetworkVariableSerializer<byte>
	// Unity.Netcode.INetworkVariableSerializer<float>
	// Unity.Netcode.INetworkVariableSerializer<int>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<UnityEngine.Quaternion>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<UnityEngine.Vector3>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<byte>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<float>
	// Unity.Netcode.NetworkVariable.OnValueChangedDelegate<int>
	// Unity.Netcode.NetworkVariable<UnityEngine.Quaternion>
	// Unity.Netcode.NetworkVariable<UnityEngine.Vector3>
	// Unity.Netcode.NetworkVariable<byte>
	// Unity.Netcode.NetworkVariable<float>
	// Unity.Netcode.NetworkVariable<int>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<UnityEngine.Quaternion>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<UnityEngine.Vector3>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<byte>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<float>
	// Unity.Netcode.NetworkVariableSerialization.EqualsDelegate<int>
	// Unity.Netcode.NetworkVariableSerialization<UnityEngine.Quaternion>
	// Unity.Netcode.NetworkVariableSerialization<UnityEngine.Vector3>
	// Unity.Netcode.NetworkVariableSerialization<byte>
	// Unity.Netcode.NetworkVariableSerialization<float>
	// Unity.Netcode.NetworkVariableSerialization<int>
	// Unity.Netcode.UnmanagedTypeSerializer<UnityEngine.Quaternion>
	// Unity.Netcode.UnmanagedTypeSerializer<UnityEngine.Vector3>
	// Unity.Netcode.UnmanagedTypeSerializer<byte>
	// Unity.Netcode.UnmanagedTypeSerializer<float>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<UnityEngine.Quaternion>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<UnityEngine.Vector3>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<byte>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.DuplicateValueDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<UnityEngine.Quaternion>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<UnityEngine.Vector3>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<byte>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.ReadValueDelegate<int>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<UnityEngine.Quaternion>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<UnityEngine.Vector3>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<byte>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<float>
	// Unity.Netcode.UserNetworkVariableSerialization.WriteValueDelegate<int>
	// UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
	// UnityEngine.InputSystem.Utilities.InlinedArray<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<UnityEngine.InputSystem.InputBinding>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<UnityEngine.InputSystem.InputControlScheme>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray.Enumerator<object>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.InputBinding>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.InputControlScheme>
	// UnityEngine.InputSystem.Utilities.ReadOnlyArray<object>
	// UnityEngine.Rendering.VolumeParameter<float>
	// UnityHFSM.ActionState<int,int>
	// UnityHFSM.ActionStorage<int>
	// UnityHFSM.IActionable<int>
	// UnityHFSM.ITriggerable<int>
	// UnityHFSM.ReverseTransition<int>
	// UnityHFSM.State<int,int>
	// UnityHFSM.StateBase<int>
	// UnityHFSM.StateMachine.PendingTransition<int,int,int>
	// UnityHFSM.StateMachine.StateBundle<int,int,int>
	// UnityHFSM.StateMachine<int,int,int>
	// UnityHFSM.Transition<int>
	// UnityHFSM.TransitionAfter<int>
	// UnityHFSM.TransitionBase<int>
	// WGame.Runtime.Singleton.<>c<object>
	// WGame.Runtime.Singleton<object>
	// WGame.Runtime.SingletonMono<object>
	// }}

	public void RefMethods()
	{
		// object Antlr4.Runtime.ParserRuleContext.GetChild<object>(int)
		// object Antlr4.Runtime.ParserRuleContext.GetRuleContext<object>(int)
		// object[] Antlr4.Runtime.ParserRuleContext.GetRuleContexts<object>()
		// Symbol Antlr4.Runtime.RuleContext.Accept<Symbol>(Antlr4.Runtime.Tree.IParseTreeVisitor<Symbol>)
		// object[] Antlr4.Runtime.Sharpen.Collections.EmptyList<object>()
		// Symbol Antlr4.Runtime.Tree.IParseTree.Accept<Symbol>(Antlr4.Runtime.Tree.IParseTreeVisitor<Symbol>)
		// System.Void CrashKonijn.Goap.Behaviours.AgentBehaviour.SetGoal<object>(bool)
		// CrashKonijn.Goap.Classes.Builders.ActionBuilder CrashKonijn.Goap.Classes.Builders.ActionBuilder.AddCondition<object>(CrashKonijn.Goap.Resolver.Comparison,int)
		// CrashKonijn.Goap.Classes.Builders.ActionBuilder CrashKonijn.Goap.Classes.Builders.ActionBuilder.AddEffect<object>(CrashKonijn.Goap.Enums.EffectType)
		// CrashKonijn.Goap.Classes.Builders.ActionBuilder CrashKonijn.Goap.Classes.Builders.ActionBuilder.Create<object>(CrashKonijn.Goap.Classes.Builders.WorldKeyBuilder,CrashKonijn.Goap.Classes.Builders.TargetKeyBuilder)
		// CrashKonijn.Goap.Classes.Builders.ActionBuilder CrashKonijn.Goap.Classes.Builders.ActionBuilder.SetTarget<object>()
		// CrashKonijn.Goap.Classes.Builders.GoalBuilder CrashKonijn.Goap.Classes.Builders.GoalBuilder.AddCondition<object>(CrashKonijn.Goap.Resolver.Comparison,int)
		// CrashKonijn.Goap.Classes.Builders.GoalBuilder CrashKonijn.Goap.Classes.Builders.GoalBuilder.Create<object>(CrashKonijn.Goap.Classes.Builders.WorldKeyBuilder)
		// CrashKonijn.Goap.Classes.Builders.ActionBuilder CrashKonijn.Goap.Classes.Builders.GoapSetBuilder.AddAction<object>()
		// CrashKonijn.Goap.Classes.Builders.GoalBuilder CrashKonijn.Goap.Classes.Builders.GoapSetBuilder.AddGoal<object>()
		// CrashKonijn.Goap.Classes.Builders.TargetSensorBuilder CrashKonijn.Goap.Classes.Builders.GoapSetBuilder.AddTargetSensor<object>()
		// CrashKonijn.Goap.Classes.Builders.WorldSensorBuilder CrashKonijn.Goap.Classes.Builders.GoapSetBuilder.AddWorldSensor<object>()
		// CrashKonijn.Goap.Classes.Builders.GoapSetBuilder CrashKonijn.Goap.Classes.Builders.GoapSetBuilder.SetAgentDebugger<object>()
		// object CrashKonijn.Goap.Classes.Builders.KeyBuilderBase<object>.GetKey<object>()
		// CrashKonijn.Goap.Classes.Builders.TargetSensorBuilder CrashKonijn.Goap.Classes.Builders.TargetSensorBuilder.Create<object>(CrashKonijn.Goap.Classes.Builders.TargetKeyBuilder)
		// CrashKonijn.Goap.Classes.Builders.TargetSensorBuilder CrashKonijn.Goap.Classes.Builders.TargetSensorBuilder.SetTarget<object>()
		// CrashKonijn.Goap.Classes.Builders.WorldSensorBuilder CrashKonijn.Goap.Classes.Builders.WorldSensorBuilder.Create<object>(CrashKonijn.Goap.Classes.Builders.WorldKeyBuilder)
		// CrashKonijn.Goap.Classes.Builders.WorldSensorBuilder CrashKonijn.Goap.Classes.Builders.WorldSensorBuilder.SetKey<object>()
		// object CrashKonijn.Goap.Interfaces.IComponentReference.GetCachedComponent<object>()
		// object CrashKonijn.Goap.Interfaces.IGoapSet.ResolveGoal<object>()
		// object CrashKonijn.Goap.Interfaces.IMonoBehaviour.GetComponent<object>()
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object DG.Tweening.TweenSettingsExtensions.SetTarget<object>(object,object)
		// object DG.Tweening.TweenSettingsExtensions.SetUpdate<object>(object,bool)
		// Entitas.ICollector<object> Entitas.CollectorContextExtension.CreateCollector<object>(Entitas.IContext<object>,Entitas.IMatcher<object>)
		// Entitas.ICollector<object> Entitas.CollectorContextExtension.CreateCollector<object>(Entitas.IContext<object>,Entitas.TriggerOnEvent<object>[])
		// Entitas.TriggerOnEvent<object> Entitas.TriggerOnEventMatcherExtension.Added<object>(Entitas.IMatcher<object>)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(Sirenix.Serialization.IDataReader)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(System.IO.Stream,Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// object Sirenix.Serialization.SerializationUtility.DeserializeValue<object>(byte[],Sirenix.Serialization.DataFormat,Sirenix.Serialization.DeserializationContext)
		// Sirenix.Serialization.Serializer<object> Sirenix.Serialization.Serializer.Get<object>()
		// bool Sirenix.Utilities.LinqExtensions.IsNullOrEmpty<object>(System.Collections.Generic.IList<object>)
		// PlayerRoomInfo System.Activator.CreateInstance<PlayerRoomInfo>()
		// object System.Activator.CreateInstance<object>()
		// byte[] System.Array.Empty<byte>()
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,int)
		// System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// Unity.Mathematics.float4 System.Linq.Enumerable.Last<Unity.Mathematics.float4>(System.Collections.Generic.IEnumerable<Unity.Mathematics.float4>)
		// int System.Linq.Enumerable.Last<int>(System.Collections.Generic.IEnumerable<int>)
		// int System.Linq.Enumerable.Max<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// object System.Linq.Enumerable.Min<int,object>(System.Collections.Generic.IEnumerable<int>,System.Func<int,object>)
		// object System.Linq.Enumerable.Min<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,float>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<int,object>(System.Collections.Generic.IEnumerable<int>,System.Func<int,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<Symbol> System.Linq.Enumerable.ToList<Symbol>(System.Collections.Generic.IEnumerable<Symbol>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<int>.Select<object>(System.Func<int,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<UnityEngine.Vector2>(Unity.Collections.NativeArray<UnityEngine.Vector2>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<UnityEngine.Vector3>(Unity.Collections.NativeArray<UnityEngine.Vector3>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<int>(Unity.Collections.NativeArray<int>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector3>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<int>()
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.BufferSerializer<object>.SerializeValue<ulong>(ulong&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferReader.ReadNetworkSerializable<PlayerRoomInfo>(PlayerRoomInfo&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<byte>(byte&)
		// System.Void Unity.Netcode.FastBufferReader.ReadUnmanagedSafe<float>(float&)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<PlayerRoomInfo>(PlayerRoomInfo&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferReader.ReadValueSafe<float>(float&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferWriter.WriteNetworkSerializable<PlayerRoomInfo>(PlayerRoomInfo&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<byte>(byte&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteUnmanagedSafe<float>(float&)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<PlayerRoomInfo>(PlayerRoomInfo&,Unity.Netcode.FastBufferWriter.ForNetworkSerializable)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.FastBufferWriter.WriteValueSafe<float>(float&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.INetworkSerializable.NetworkSerialize<Unity.Netcode.BufferSerializerReader>(Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerReader>)
		// System.Void Unity.Netcode.INetworkSerializable.NetworkSerialize<Unity.Netcode.BufferSerializerWriter>(Unity.Netcode.BufferSerializer<Unity.Netcode.BufferSerializerWriter>)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<byte>(byte&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<int>(int&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// System.Void Unity.Netcode.IReaderWriter.SerializeValue<ulong>(ulong&,Unity.Netcode.FastBufferWriter.ForPrimitives)
		// bool Unity.Netcode.NetworkVariableSerialization<UnityEngine.Quaternion>.EqualityEquals<UnityEngine.Quaternion>(UnityEngine.Quaternion&,UnityEngine.Quaternion&)
		// bool Unity.Netcode.NetworkVariableSerialization<UnityEngine.Vector3>.EqualityEquals<UnityEngine.Vector3>(UnityEngine.Vector3&,UnityEngine.Vector3&)
		// bool Unity.Netcode.NetworkVariableSerialization<byte>.EqualityEquals<byte>(byte&,byte&)
		// bool Unity.Netcode.NetworkVariableSerialization<float>.EqualityEquals<float>(float&,float&)
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<UnityEngine.Quaternion>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<UnityEngine.Vector3>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<byte>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedIEquatable<float>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<UnityEngine.Quaternion>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<UnityEngine.Vector3>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<byte>()
		// System.Void Unity.Netcode.NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedByMemcpy<float>()
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.Component.TryGetComponent<object>(object&)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.ReadValue<UnityEngine.Vector2>()
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ApplyProcessors<UnityEngine.Vector2>(int,UnityEngine.Vector2,UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
		// System.Void UnityEngine.Mesh.SetIndices<int>(Unity.Collections.NativeArray<int>,int,int,UnityEngine.MeshTopology,int,bool,int)
		// System.Void UnityEngine.Mesh.SetUVs<UnityEngine.Vector2>(int,Unity.Collections.NativeArray<UnityEngine.Vector2>,int,int)
		// System.Void UnityEngine.Mesh.SetUVs<UnityEngine.Vector2>(int,Unity.Collections.NativeArray<UnityEngine.Vector2>,int,int,UnityEngine.Rendering.MeshUpdateFlags)
		// System.Void UnityEngine.Mesh.SetVertices<UnityEngine.Vector3>(Unity.Collections.NativeArray<UnityEngine.Vector3>,int,int)
		// System.Void UnityEngine.Mesh.SetVertices<UnityEngine.Vector3>(Unity.Collections.NativeArray<UnityEngine.Vector3>,int,int,UnityEngine.Rendering.MeshUpdateFlags)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object[] UnityEngine.Object.FindObjectsOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// bool UnityEngine.Rendering.VolumeProfile.TryGet<object>(System.Type,object&)
		// bool UnityEngine.Rendering.VolumeProfile.TryGet<object>(object&)
		// object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
		// object UnityEngine.Resources.Load<object>(string)
		// System.Void UnityHFSM.StateMachineShortcuts.AddExitTransition<int,int,int>(UnityHFSM.StateMachine<int,int,int>,int,System.Func<UnityHFSM.Transition<int>,bool>,System.Action<UnityHFSM.Transition<int>>,System.Action<UnityHFSM.Transition<int>>,bool)
		// System.Void UnityHFSM.StateMachineShortcuts.AddTransition<int,int,int>(UnityHFSM.StateMachine<int,int,int>,int,int,System.Func<UnityHFSM.Transition<int>,bool>,System.Action<UnityHFSM.Transition<int>>,System.Action<UnityHFSM.Transition<int>>,bool)
		// System.Void UnityHFSM.StateMachineShortcuts.AddTransitionFromAny<int,int,int>(UnityHFSM.StateMachine<int,int,int>,int,System.Func<UnityHFSM.Transition<int>,bool>,System.Action<UnityHFSM.Transition<int>>,System.Action<UnityHFSM.Transition<int>>,bool)
		// System.Void UnityHFSM.StateMachineShortcuts.AddTriggerTransition<int,int,int>(UnityHFSM.StateMachine<int,int,int>,int,int,int,System.Func<UnityHFSM.Transition<int>,bool>,System.Action<UnityHFSM.Transition<int>>,System.Action<UnityHFSM.Transition<int>>,bool)
		// UnityHFSM.TransitionBase<int> UnityHFSM.StateMachineShortcuts.CreateOptimizedTransition<int>(int,int,System.Func<UnityHFSM.Transition<int>,bool>,System.Action<UnityHFSM.Transition<int>>,System.Action<UnityHFSM.Transition<int>>,bool)
		// int WGame.Utils.JsonHelper.ReadEnum<int>(LitJson.JsonData)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}
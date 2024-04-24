using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"MessagePack.dll",
		"System.Core.dll",
		"UnityEngine.CoreModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// MessagePack.Formatters.IMessagePackFormatter<object>
	// Nerdbank.Streams.Sequence.SequenceSegment<byte>
	// Nerdbank.Streams.Sequence<byte>
	// System.Action<Goblin.Common.TickEvent>
	// System.Action<Goblin.Common.Ticker.TimerInfo>
	// System.Action<float>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment<byte>
	// System.Buffers.ArrayMemoryPool.ArrayMemoryPoolBuffer<byte>
	// System.Buffers.ArrayMemoryPool<byte>
	// System.Buffers.ArrayPool<byte>
	// System.Buffers.ConfigurableArrayPool.Bucket<byte>
	// System.Buffers.ConfigurableArrayPool<byte>
	// System.Buffers.IMemoryOwner<byte>
	// System.Buffers.MemoryManager<byte>
	// System.Buffers.MemoryPool<byte>
	// System.Buffers.ReadOnlySequence.<>c<byte>
	// System.Buffers.ReadOnlySequence.Enumerator<byte>
	// System.Buffers.ReadOnlySequence<byte>
	// System.Buffers.ReadOnlySequenceSegment<byte>
	// System.Buffers.SpanAction<ushort,System.Buffers.ReadOnlySequence<ushort>>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.LockedStack<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool.PerCoreLockedStacks<byte>
	// System.Buffers.TlsOverPerCoreLockedStacksArrayPool<byte>
	// System.ByReference<byte>
	// System.Collections.Generic.ArraySortHelper<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.UIntPtr,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<System.UIntPtr,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Goblin.Common.Ticker.TimerInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Goblin.Common.Ticker.TimerInfo>
	// System.Comparison<object>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,byte>
	// System.Func<UnityEngine.SceneManagement.Scene>
	// System.Func<object,UnityEngine.SceneManagement.Scene>
	// System.Func<object,byte>
	// System.Func<object,object>
	// System.Func<object>
	// System.Memory<byte>
	// System.Nullable<byte>
	// System.Nullable<int>
	// System.Predicate<Goblin.Common.Ticker.TimerInfo>
	// System.Predicate<object>
	// System.ReadOnlyMemory<byte>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan<byte>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<UnityEngine.SceneManagement.Scene>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<UnityEngine.SceneManagement.Scene>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<UnityEngine.SceneManagement.Scene>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Span<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<UnityEngine.SceneManagement.Scene>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<UnityEngine.SceneManagement.Scene>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory<UnityEngine.SceneManagement.Scene>
	// System.Threading.Tasks.TaskFactory<object>
	// }}

	public void RefMethods()
	{
		// MessagePack.Formatters.IMessagePackFormatter<object> MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<object>(MessagePack.IFormatterResolver)
		// MessagePack.Formatters.IMessagePackFormatter<object> MessagePack.IFormatterResolver.GetFormatter<object>()
		// System.Void MessagePack.MessagePackSerializer.Serialize<object>(MessagePack.MessagePackWriter&,object,MessagePack.MessagePackSerializerOptions)
		// byte[] MessagePack.MessagePackSerializer.Serialize<object>(object,MessagePack.MessagePackSerializerOptions,System.Threading.CancellationToken)
		// object System.Activator.CreateInstance<object>()
		// bool System.Enum.TryParse<int>(string,bool,int&)
		// bool System.Enum.TryParse<int>(string,int&)
		// System.Collections.Generic.KeyValuePair<int,object> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<int,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,bool>)
		// object System.Linq.Enumerable.Last<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Goblin.Common.Res.GameRes.<LoadSceneASync>d__6>(System.Runtime.CompilerServices.TaskAwaiter&,Goblin.Common.Res.GameRes.<LoadSceneASync>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Goblin.Common.Res.GameRes.<LoadAssetAsync>d__2<object>>(System.Runtime.CompilerServices.TaskAwaiter&,Goblin.Common.Res.GameRes.<LoadAssetAsync>d__2<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Goblin.Common.Res.GameRes.<LoadRawFileAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,Goblin.Common.Res.GameRes.<LoadRawFileAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Common.Res.GameResLocation.<LoadConfigAsync>d__10>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Common.Res.GameResLocation.<LoadConfigAsync>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Common.Res.GameResLocation.<LoadSpriteAsync>d__7>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Common.Res.GameResLocation.<LoadSpriteAsync>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Common.Res.GameResLocation.<LoadUIPrefabAsync>d__5>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Common.Res.GameResLocation.<LoadUIPrefabAsync>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Sys.Common.GameUI.<Load>d__8<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Sys.Common.GameUI.<Load>d__8<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Sys.Common.UIBaseView.<Load>d__15>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Sys.Common.UIBaseView.<Load>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.Start<Goblin.Common.Res.GameRes.<LoadSceneASync>d__6>(Goblin.Common.Res.GameRes.<LoadSceneASync>d__6&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Common.Res.GameRes.<LoadAssetAsync>d__2<object>>(Goblin.Common.Res.GameRes.<LoadAssetAsync>d__2<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Common.Res.GameRes.<LoadRawFileAsync>d__4>(Goblin.Common.Res.GameRes.<LoadRawFileAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Common.Res.GameResLocation.<LoadConfigAsync>d__10>(Goblin.Common.Res.GameResLocation.<LoadConfigAsync>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Common.Res.GameResLocation.<LoadSpriteAsync>d__7>(Goblin.Common.Res.GameResLocation.<LoadSpriteAsync>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Common.Res.GameResLocation.<LoadUIPrefabAsync>d__5>(Goblin.Common.Res.GameResLocation.<LoadUIPrefabAsync>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Sys.Common.GameUI.<Load>d__8<object>>(Goblin.Sys.Common.GameUI.<Load>d__8<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Goblin.Sys.Common.UIBaseView.<Load>d__15>(Goblin.Sys.Common.UIBaseView.<Load>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Sys.Common.GameUI.<Open>d__12<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Sys.Common.GameUI.<Open>d__12<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Sys.Common.UIBase.<SetSprite>d__23<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Sys.Common.UIBase.<SetSprite>d__23<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Goblin.Sys.Common.UIBase.<SetSprite>d__24<object>>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Goblin.Sys.Common.UIBase.<SetSprite>d__24<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Goblin.Core.Engine.<OnCreate>d__9>(Goblin.Core.Engine.<OnCreate>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Goblin.Sys.Common.GameUI.<Open>d__12<object>>(Goblin.Sys.Common.GameUI.<Open>d__12<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Goblin.Sys.Common.UIBase.<SetSprite>d__23<object>>(Goblin.Sys.Common.UIBase.<SetSprite>d__23<object>&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Goblin.Sys.Common.UIBase.<SetSprite>d__24<object>>(Goblin.Sys.Common.UIBase.<SetSprite>d__24<object>&)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// YooAsset.AssetOperationHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetAsync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
	}
}
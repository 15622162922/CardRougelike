//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

#pragma warning disable 649

using System;
using UnityEngine;

public partial class TileDataConfig : ScriptableObject {

	[NonSerialized]
	private int mVersion = 1;

	[SerializeField]
	private TileData[] _TileDataItems;

	public TileData GetTileData(int id) {
		int min = 0;
		int max = _TileDataItems.Length;
		while (min < max) {
			int index = (min + max) >> 1;
			TileData item = _TileDataItems[index];
			if (item.id == id) { return item.Init(mVersion, DataGetterObject); }
			if (id < item.id) {
				max = index;
			} else {
				min = index + 1;
			}
		}
		return null;
	}

	public void Reset() {
		mVersion++;
	}

	public interface IDataGetter {
		TileData GetTileData(int id);
	}

	private class DataGetter : IDataGetter {
		private Func<int, TileData> _GetTileData;
		public TileData GetTileData(int id) {
			return _GetTileData(id);
		}
		public DataGetter(Func<int, TileData> getTileData) {
			_GetTileData = getTileData;
		}
	}

	[NonSerialized]
	private DataGetter mDataGetterObject;
	private DataGetter DataGetterObject {
		get {
			if (mDataGetterObject == null) {
				mDataGetterObject = new DataGetter(GetTileData);
			}
			return mDataGetterObject;
		}
	}
}

[Serializable]
public class TileData {

	[SerializeField]
	private int _Id;
	public int id { get { return _Id; } }

	[SerializeField]
	private int _Group;
	public int group { get { return _Group; } }

	[SerializeField]
	private string _TileName;
	public string tileName { get { return _TileName; } }

	[SerializeField]
	private string _TilePath;
	public string tilePath { get { return _TilePath; } }

	[NonSerialized]
	private int mVersion = 0;
	[NonSerialized]
	private TileDataConfig.IDataGetter mGetter;

	public TileData Init(int version, TileDataConfig.IDataGetter getter) {
		if (mVersion == version) { return this; }
		mGetter = getter;
		mVersion = version;
		return this;
	}

	public override string ToString() {
		return string.Format("[TileData]{{id:{0}, group:{1}, tileName:{2}, tilePath:{3}}}",
			id, group, tileName, tilePath);
	}

}


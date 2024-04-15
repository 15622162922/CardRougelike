//----------------------------------------------
//    Auto Generated. DO NOT edit manually!
//----------------------------------------------

#pragma warning disable 649

using System;
using UnityEngine;

public partial class MapDataConfig : ScriptableObject {

	[NonSerialized]
	private int mVersion = 1;

	[SerializeField]
	private MapData[] _MapDataItems;

	public MapData GetMapData(int id) {
		int min = 0;
		int max = _MapDataItems.Length;
		while (min < max) {
			int index = (min + max) >> 1;
			MapData item = _MapDataItems[index];
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
		MapData GetMapData(int id);
	}

	private class DataGetter : IDataGetter {
		private Func<int, MapData> _GetMapData;
		public MapData GetMapData(int id) {
			return _GetMapData(id);
		}
		public DataGetter(Func<int, MapData> getMapData) {
			_GetMapData = getMapData;
		}
	}

	[NonSerialized]
	private DataGetter mDataGetterObject;
	private DataGetter DataGetterObject {
		get {
			if (mDataGetterObject == null) {
				mDataGetterObject = new DataGetter(GetMapData);
			}
			return mDataGetterObject;
		}
	}
}

[Serializable]
public class MapData {

	[SerializeField]
	private int _Id;
	public int id { get { return _Id; } }

	[SerializeField]
	private int _Group;
	public int group { get { return _Group; } }

	[SerializeField]
	private string _MapName;
	public string mapName { get { return _MapName; } }

	[SerializeField]
	private int _TileGroup;
	public int tileGroup { get { return _TileGroup; } }

	[NonSerialized]
	private int mVersion = 0;
	[NonSerialized]
	private MapDataConfig.IDataGetter mGetter;

	public MapData Init(int version, MapDataConfig.IDataGetter getter) {
		if (mVersion == version) { return this; }
		mGetter = getter;
		mVersion = version;
		return this;
	}

	public override string ToString() {
		return string.Format("[MapData]{{id:{0}, group:{1}, mapName:{2}, tileGroup:{3}}}",
			id, group, mapName, tileGroup);
	}

}


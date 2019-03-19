//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class PlayerAttrBase : JW_Table.Binary, JW_Table.IKey
{
	private int m_ID;
	private int m_Level;
	private int m_MaxHp;
	private int m_HpRestore;
	private int m_MaxMp;
	private int m_MpRestore;
	private int m_MaxDp;
	private int m_DpRestore;
	private int m_Attack;
	private int m_Defense;
	private int m_Speed;

	public int ID
	{
		get { return m_ID; }
		set { m_ID = value; }
	}

	public int Level
	{
		get { return m_Level; }
		set { m_Level = value; }
	}

	public int MaxHp
	{
		get { return m_MaxHp; }
		set { m_MaxHp = value; }
	}

	public int HpRestore
	{
		get { return m_HpRestore; }
		set { m_HpRestore = value; }
	}

	public int MaxMp
	{
		get { return m_MaxMp; }
		set { m_MaxMp = value; }
	}

	public int MpRestore
	{
		get { return m_MpRestore; }
		set { m_MpRestore = value; }
	}

	public int MaxDp
	{
		get { return m_MaxDp; }
		set { m_MaxDp = value; }
	}

	public int DpRestore
	{
		get { return m_DpRestore; }
		set { m_DpRestore = value; }
	}

	public int Attack
	{
		get { return m_Attack; }
		set { m_Attack = value; }
	}

	public int Defense
	{
		get { return m_Defense; }
		set { m_Defense = value; }
	}

	public int Speed
	{
		get { return m_Speed; }
		set { m_Speed = value; }
	}

	public long Key()
	{
		return JW_Table.TableUtility.Combine(m_ID, m_Level, 4);
	}

	public override void Read(JW_Table.Reader reader)
	{
		m_ID = reader.ReadInt32();
		m_Level = reader.ReadInt32();
		m_MaxHp = reader.ReadInt32();
		m_HpRestore = reader.ReadInt32();
		m_MaxMp = reader.ReadInt32();
		m_MpRestore = reader.ReadInt32();
		m_MaxDp = reader.ReadInt32();
		m_DpRestore = reader.ReadInt32();
		m_Attack = reader.ReadInt32();
		m_Defense = reader.ReadInt32();
		m_Speed = reader.ReadInt32();
	}
}

//PlayerAttrBase.xlsx
public sealed class PlayerAttrBaseManager : JW_Table.TableManager<PlayerAttrBase>
{
	public const uint VERSION = 3327966586;

	private PlayerAttrBaseManager()
	{
	}

	private static readonly PlayerAttrBaseManager ms_instance = new PlayerAttrBaseManager();

	public static PlayerAttrBaseManager instance
	{
		get { return ms_instance; }
	}

	public string source
	{
		get { return "PlayerAttrBase.tbl"; }
	}

	public bool Load(string path)
	{
		return Load(path, source, VERSION);
	}

	public bool Load(byte[] buffer)
	{
		return Load(buffer, VERSION, source);
	}

	public PlayerAttrBase Find(int k1, int k2)
	{
		return FindInternal(JW_Table.TableUtility.Combine(k1, k2, 4));
	}
}

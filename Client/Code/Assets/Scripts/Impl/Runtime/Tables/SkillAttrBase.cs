//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SkillAttrBase : JW_Table.Binary, JW_Table.IKey
{
	private int m_ID;
	private int m_Level;
	private int m_CD;
	private int m_DamageBase;
	private int m_DamageCoff;
	private int m_DpDamageCoff;

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

	public int CD
	{
		get { return m_CD; }
		set { m_CD = value; }
	}

	public int DamageBase
	{
		get { return m_DamageBase; }
		set { m_DamageBase = value; }
	}

	public int DamageCoff
	{
		get { return m_DamageCoff; }
		set { m_DamageCoff = value; }
	}

	public int DpDamageCoff
	{
		get { return m_DpDamageCoff; }
		set { m_DpDamageCoff = value; }
	}

	public long Key()
	{
		return JW_Table.TableUtility.Combine(m_ID, m_Level, 4);
	}

	public override void Read(JW_Table.Reader reader)
	{
		m_ID = reader.ReadInt32();
		m_Level = reader.ReadInt32();
		m_CD = reader.ReadInt32();
		m_DamageBase = reader.ReadInt32();
		m_DamageCoff = reader.ReadInt32();
		m_DpDamageCoff = reader.ReadInt32();
	}
}

//SkillAttrBase.xlsx
public sealed class SkillAttrBaseManager : JW_Table.TableManager<SkillAttrBase>
{
	public const uint VERSION = 3644501696;

	private SkillAttrBaseManager()
	{
	}

	private static readonly SkillAttrBaseManager ms_instance = new SkillAttrBaseManager();

	public static SkillAttrBaseManager instance
	{
		get { return ms_instance; }
	}

	public string source
	{
		get { return "SkillAttrBase.tbl"; }
	}

	public bool Load(string path)
	{
		return Load(path, source, VERSION);
	}

	public bool Load(byte[] buffer)
	{
		return Load(buffer, VERSION, source);
	}

	public SkillAttrBase Find(int k1, int k2)
	{
		return FindInternal(JW_Table.TableUtility.Combine(k1, k2, 4));
	}
}

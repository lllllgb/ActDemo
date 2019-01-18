using System.Collections.Generic;

namespace Model
{
	public class FileVersionInfo
	{
		public string File;
		public string MD5;
		public long Size;
	}

	public class VersionConfig
	{
		public int Version;
		
		public long TotalSize;

		public List<FileVersionInfo> FileVersionInfos = new List<FileVersionInfo>();
	}
}
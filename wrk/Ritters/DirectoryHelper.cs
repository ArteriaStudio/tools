using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.OS.Base
{
	public static class DirectoryHelper
	{
		public static bool CreateDirectorySafe(String pPath)
		{
			if (Directory.Exists(pPath) == true)
			{
				return(true);
			}
			try
			{
				var pDirectoryInfo = Directory.CreateDirectory(pPath);
				return(true);
			} catch {
				;
			}
			return (false);
		}
	}
}

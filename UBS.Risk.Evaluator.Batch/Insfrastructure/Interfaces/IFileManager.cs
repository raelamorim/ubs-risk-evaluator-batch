using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBS.Risk.Evaluator.Batch.Insfrastructure.Interfaces
{
	public interface IFileManager
	{
		StreamReader CreateReader(string path);
		StreamWriter CreateWriter(string path);
	}

}

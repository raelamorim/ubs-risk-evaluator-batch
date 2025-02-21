using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBS.Risk.Evaluator.Batch.Insfrastructure.Interfaces;

namespace UBS.Risk.Evaluator.Batch.Insfrastructure
{
	public class FileManager : IFileManager
	{
		public StreamReader CreateReader(string path) => new StreamReader(path);

		public StreamWriter CreateWriter(string path) => new StreamWriter(path);
	}
}

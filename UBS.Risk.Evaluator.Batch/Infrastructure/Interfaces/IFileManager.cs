
namespace UBS.Risk.Evaluator.Batch.Infrastructure.Interfaces
{
	public interface IFileManager
	{
		StreamReader CreateReader(string path);
		StreamWriter CreateWriter(string path);
	}

}

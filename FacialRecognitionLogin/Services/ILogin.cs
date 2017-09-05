using System.Threading.Tasks;

namespace FacialRecognitionLogin
{
	public interface ILogin
	{
		Task<bool> SetPasswordForUsername(string username, string password);
		Task<bool> CheckLogin(string username, string password);
	}
}


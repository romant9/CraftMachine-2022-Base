using System.Threading.Tasks;

public interface IInterceptor
{
	Task<bool> Intercept();
}

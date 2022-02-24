namespace Glucy.Utils;

public interface IConfig<in T> {

	public void OnLoad(T config);

}

using LinqToDB;
using me.admin.api.Utils;
using Microsoft.Extensions.Options;

namespace me.admin.api.Data;

public class AppDbContext
{
	readonly string _connectionString;

	public AppDbContext(IOptions<DatabaseSettings> databaseSettings)
	{
		var databaseSettingsValue = databaseSettings.Value;
		_connectionString = $"Server={databaseSettingsValue.Host};Port={databaseSettingsValue.Port};Database={databaseSettingsValue.Name};Uid={databaseSettingsValue.Username};Pwd={databaseSettingsValue.Password};";
	}

	public DataContext GetDatabase()
	{
		return new DataContext(new DataOptions().UsePostgreSQL(_connectionString));
	}
}
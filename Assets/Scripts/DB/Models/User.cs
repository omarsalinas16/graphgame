using SQLite4Unity3d;

public class User {

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

	public string Username { get; set; }

}

using SQLite4Unity3d;

namespace Model {
	public class User {

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		override public string ToString() {
			return " Username: " + Username + " Password: " + Password;
		}

	}
}

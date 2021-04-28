namespace AlphaBookingRooms.Models
{
    // Модель пользователя с полем роли
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
		public int Role { get; set; }       // 0 - менеджер, 1 - сотрудник
    }
}

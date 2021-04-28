using Microsoft.EntityFrameworkCore;

namespace AlphaBookingRooms.Models
{
	// Контекст данных приложения
	public class ApplicationContext : DbContext
	{
		// Пользователи
		public DbSet<User> Users { get; set; }

		// Комнаты
		public DbSet<Room> Rooms { get; set; }
		public DbSet<BookingTime> BookingTimes { get; set; }

		public ApplicationContext(DbContextOptions<ApplicationContext> options)
			: base(options)
		{
			// Если БД нет - создать
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
	}
}
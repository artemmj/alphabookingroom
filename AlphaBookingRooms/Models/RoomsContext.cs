using System;
using System.Collections.Generic;

namespace AlphaBookingRooms.Models
{
	// TODO перед удалением
	// Добавляется ограничение "Players_Teams" в таблицу игроков, которая называется, как правило, dbo.Players или просто Players.
	// Это ограничение указывает, что для внешнего ключа TeamId из таблицы Players, который связан со столбцом Id из таблицы dbo.Teams,
	// устанавливается правило "ON DELETE SET NULL", то есть установка null по удалению.
	// Пример: 
	// db.Database.ExecuteSqlCommand("ALTER TABLE dbo.Players ADD CONSTRAINT Players_Teams FOREIGN KEY (TeamId) REFERENCES dbo.Teams (Id) ON DELETE CASCADE");
	// В данном случае неактуально, но стоит оставить

	// Модель комнаты
	public class Room
	{
		public int Id { get; set; }

		public string Name { get; set; }			// название/номер комнаты
		public int SeatsNumber { get; set; }		// количество мест
		public bool HasProjector { get; set; }		// наличие проектора
		public bool HasTable { get; set; }			// наличие доски
		public string Description { get; set; }		// описание комнаты
		
		public List<BookingTime> BookingTimes { get; set; }
	}

	// Модель забитых времен
	public class BookingTime
	{
		public int Id { get; set; }

		public string Description { get; set; }		// название мероприятия
		public DateTime StartTime { get; set; }		// начало мероприятия
		public DateTime EndTime { get; set; }       // конец мероприятия
		public bool IsConfirmed { get; set; }       // подтверждено ли время

		public int RoomId { get; set; }
		public Room Room { get; set; }
	}
}

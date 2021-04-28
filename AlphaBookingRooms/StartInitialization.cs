using AlphaBookingRooms.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlphaBookingRooms
{
	// Класс исключительно для начального заполнения БД данными
	public class StartInitialization
	{
		// Статический метод, вызывается единожды в Program.Main()
		public static void UsersInitialize(ApplicationContext appContext)
		{
			/* Пока данных Пользователей нет, добавить данные в БД */
			if (!appContext.Users.Any() && !appContext.Users.Any())
			{
				// Три пользователя
				User admin = new User { Email = "admin", Password = "admin", Role = 0 };
				User manager = new User { Email = "manager@mail.ru", Password = "manager123", Role = 0 };
				User employee = new User { Email = "employee@mail.ru", Password = "employee123", Role = 1 };
				appContext.AddRange(new List<User> { admin, manager, employee });
				appContext.SaveChanges();
			}

			/* Аналогично для комнат */
			if (!appContext.Rooms.Any() && !appContext.BookingTimes.Any())
			{
				// И комнаты с событиями, подтвержденными и нет
				Room room2001 = new Room() { Name = "200.1", SeatsNumber = 20, HasProjector = true, HasTable = true, Description = "Комната обучения" };
				Room room2002 = new Room() { Name = "200.2", SeatsNumber = 25, HasProjector = true, HasTable = false, Description = "Презентационная комната" };
				Room room2003 = new Room() { Name = "200.3", SeatsNumber = 15, HasProjector = false, HasTable = true, Description = "Комната для собеседований" };
				Room room2004 = new Room() { Name = "200.4", SeatsNumber = 20, HasProjector = true, HasTable = true, Description = "Универсальная комната" };
				Room room2005 = new Room() { Name = "200.5", SeatsNumber = 30, HasProjector = true, HasTable = true, Description = "Презентационная комната" };
				Room room3001 = new Room() { Name = "300.1", SeatsNumber = 10, HasProjector = false, HasTable = true, Description = "Кабинет заседания начальства" };
				Room room3002 = new Room() { Name = "300.2", SeatsNumber = 30, HasProjector = true, HasTable = false, Description = "Переговорная" };
				Room room3003 = new Room() { Name = "300.3", SeatsNumber = 50, HasProjector = true, HasTable = true, Description = "Переговорная" };
				appContext.Rooms.AddRange(new List<Room> { room2001, room2002, room2003, room2004, room2005, room3001, room3002, room3003 });
				appContext.SaveChanges();

				BookingTime bookingTime20011 = new BookingTime()
				{
					Description = "Инструктаж",
					StartTime = new DateTime(2019, 3, 21, 8, 30, 0),
					EndTime = new DateTime(2019, 3, 21, 9, 30, 0),
					IsConfirmed = true,
					RoomId = room2001.Id
				};

				BookingTime bookingTime20012 = new BookingTime()
				{
					Description = "Основы профессии",
					StartTime = new DateTime(2019, 3, 21, 9, 30, 0),
					EndTime = new DateTime(2019, 3, 21, 10, 30, 0),
					IsConfirmed = true,
					RoomId = room2001.Id
				};

				BookingTime bookingTime20013 = new BookingTime()
				{
					Description = "Вопросы-ответы",
					StartTime = new DateTime(2019, 3, 21, 10, 30, 0),
					EndTime = new DateTime(2019, 3, 21, 12, 00, 0),
					IsConfirmed = false,
					RoomId = room2001.Id
				};

				BookingTime bookingTime30011 = new BookingTime()
				{
					Description = "Заседание директоров",
					StartTime = new DateTime(2019, 3, 22, 9, 00, 0),
					EndTime = new DateTime(2019, 3, 22, 12, 00, 0),
					IsConfirmed = true,
					RoomId = room3001.Id
				};

				BookingTime bookingTime30012 = new BookingTime()
				{
					Description = "Обсуждение отчетов",
					StartTime = new DateTime(2019, 3, 21, 13, 30, 0),
					EndTime = new DateTime(2019, 3, 21, 15, 00, 0),
					IsConfirmed = false,
					RoomId = room3001.Id
				};

				appContext.BookingTimes.AddRange(new List<BookingTime>()
				{
					bookingTime20011, bookingTime20012, bookingTime20013, bookingTime30011, bookingTime30012
				});
				appContext.SaveChanges();
			}
		}
	}
}

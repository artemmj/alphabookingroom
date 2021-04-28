using AlphaBookingRooms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AlphaBookingRooms.Controllers
{
    public class HomeController : Controller
    {
		// Контекст данных
        private ApplicationContext db;
		// Текущий пользователь
		private static User CurrentUser = null;
		
        public HomeController(ApplicationContext context)
        {
            db = context;
        }
		
		[HttpGet]
		[Authorize]
		public IActionResult Index()
		{
			// Роль пользователя для View и сам пользователь
			CurrentUser = db.Users.Where(u => u.Email == User.Identity.Name).Single();
			ViewBag.Role = CurrentUser.Role;

			// В лист комнат отбираем комнаты и дочерние записи времен
			// В цикле-подцикле удаляем из листа неподтвержденные времена
			List<Room> roomsList = db.Rooms.Include(u => u.BookingTimes).ToList();
			for (int i = 0; i < roomsList.Count; i++)
			{
				for (int j = 0; j < roomsList[i].BookingTimes.Count; j++)
				{
					if (roomsList[i].BookingTimes[j].IsConfirmed == false)
						roomsList[i].BookingTimes.Remove(roomsList[i].BookingTimes[j]);
				}
			}
			// Если менеджер - отобрать отдельно неподтвержденные времена для View
			if (CurrentUser.Role == 0)
				ViewBag.UnconfirmedTimes = db.BookingTimes.Where(u => u.IsConfirmed == false).ToList();

			return View("Index", roomsList);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Index(bool confirm, int roomId)
		{
			BookingTime time = db.BookingTimes.Where(t => t.Room.Id == roomId).Single();
			// Если подтверждаем заявку - переставляем флаг IsConfirmed на true
			if (confirm)
			{
				if (time != null)
				{
					time.IsConfirmed = true;
					db.SaveChanges();
				}
				return View();
			}
			// Если отклоняем заявку - удаляем запись события из БД
			else
			{
				db.BookingTimes.Remove(time);
				db.SaveChanges();
				return View();
			}
		}

		[HttpGet]
		[Authorize]
		public IActionResult RoomInfo(int roomId)
		{
			// Отбираем необходимую комнату по айди
			Room room = db.Rooms
						.Include(u => u.BookingTimes)
						.ToList()
						.Where(u => u.Id == roomId)
						.Single();
			// В цикле из внутреннего листа времен удаляем те, которые неподтверждены
			for (int i = 0; i < room.BookingTimes.Count; i++)
			{
				if (room.BookingTimes[i].IsConfirmed == false)
					room.BookingTimes.Remove(room.BookingTimes[i]);
			}
			// Роль пользователя для View
			ViewBag.Role = CurrentUser.Role;

			return View(room);
		}

		[HttpGet]
		[Authorize]
		public IActionResult BookARoom(int roomId)
		{
			// Id комнаты для дальнейшего бронирования времени
			ViewBag.RoomId = roomId;
			return View();
		}

		[HttpPost]
		[Authorize]
		public IActionResult BookARoom(int roomId, DateTime startTime, DateTime endTime, string description)
		{
			// Отбираем времена для контроля непересечения бронируемого времени с уже существующими
			List<BookingTime> currentTimes = db.BookingTimes.Where(u => u.RoomId == roomId).ToList();

			foreach(BookingTime time in currentTimes)
			{
				// Если бронируемое время как-либо пересекается с существующим/ими
				if (time.StartTime == startTime && time.EndTime == endTime ||
					time.StartTime < startTime && time.EndTime == endTime ||
					time.StartTime == startTime && time.EndTime > endTime ||
					time.StartTime < startTime && time.EndTime > endTime ||
					time.StartTime > startTime && time.EndTime < endTime)
				{
					ViewBag.TimeBusy = "Время занято, повторите ввод";
					return View();
				}
			}

			// Иначе новый неподтвержденный экземпляр времени в БД с возвратом в Index
			BookingTime newBookingTime = new BookingTime()
			{
				StartTime = startTime,
				EndTime = endTime,
				Description = description,
				IsConfirmed = false,
				RoomId = roomId
			};
			db.BookingTimes.Add(newBookingTime);
			db.SaveChanges();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult EditRoom(int roomId)
		{
			// Отбираем айди комнаты для редактирования
			Room room = db.Rooms.Where(r => r.Id == roomId).Single();
			ViewBag.Id = roomId;
			return View(room);
		}

		[HttpPost]
		[Authorize]
		/* bool hasProjector, bool hasTable - не работают корректно типы checkbox */
		public IActionResult EditRoom(int roomId, string roomName, int seatsNumber, string description)
		{
			// По айди комнаты находим нужную, и заполняем нужными значениями
			Room room = db.Rooms.FirstOrDefault(p => p.Id == roomId);
			if (room != null)
			{
				room.Name = roomName;
				room.SeatsNumber = seatsNumber;
				room.Description = description;
				db.SaveChanges();
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult EditUser()
		{
			// Отбираем список пользователей во view
			List<User> users = db.Users.ToList();
			return View(users);
		}

		[HttpPost]
		[Authorize]
		public IActionResult EditUser(int userId, int roleId)
		{
			// Находим нужного пользователя по айди и выставляем переданный айди роли
			User user = db.Users.Where(t => t.Id == userId).Single();
			if (user != null)
			{
				user.Role = roleId;
				db.SaveChanges();
			}
			return RedirectToAction("Index", "Home");
		}

		// Метод для периодического отбора неподтвержденных времен (ajax-запрос), для отображения возвращает json
		[HttpGet]
		[Authorize]
		public string GetNewRequests()
		{
			List<BookingTime> bookingTimes = db.BookingTimes.Include(t => t.Room).Where(t => t.IsConfirmed == false).ToList();
			for(int i = 0; i < bookingTimes.Count; i++)
				bookingTimes[i].Room.BookingTimes = null;
			return Newtonsoft.Json.JsonConvert.SerializeObject(bookingTimes);	// возвращаем cериализованный json
		}

		// Подтверждение времени по его айди
		[HttpPut]
		[Authorize]
		public void ConfirmTime(int id)
		{
			BookingTime time = db.BookingTimes.Where(t => t.Id == id).Single();
			time.IsConfirmed = true;
			db.SaveChanges();
		}

		// Не подтверждение времени по его айди
		[HttpPut]
		[Authorize]
		public void NotConfirmTime(int id)
		{
			BookingTime time = db.BookingTimes.Where(t => t.Id == id).Single();
			db.BookingTimes.Remove(time);
			db.SaveChanges();
		}

		// Стандартный метод для ошибок
		[Authorize]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

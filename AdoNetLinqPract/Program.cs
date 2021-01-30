using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetLinqPract
{
    class Program
    {
        static void Main(string[] args)
        {
            BarberShop bs = new BarberShop();
            
            //1
             bs.ShowBarber(bs.OldestBarber());
            Console.WriteLine(new string('-',140));
            //2
            bs.ShowBarber(bs.MaxCountBarbers(new DateTime(2016,1,1),new DateTime(2020,1,1)));
            Console.WriteLine(new string('-', 140));
            //3
            bs.ShowClient(bs.MaxCountClientVisit());
            Console.WriteLine(new string('-', 140));
            //4
            bs.ShowClient(bs.MaxTotalPriceClient());
            Console.WriteLine(new string('-', 140));
            //5
            bs.ShowService(bs.LongestServices());
            Console.WriteLine(new string('-', 140));
            //2.1
            bs.ShowBarber(bs.MostPopularBarber());
            Console.WriteLine(new string('-', 140));
            //2.2
            bs.ShowBarbers(bs.Top3TotalPriceClientForMonth());
            Console.WriteLine(new string('-', 140));
            //2.3 
            bs.ShowBarbers(bs.Top3AvgMarkClientvisitMore30());
            Console.WriteLine(new string('-', 140));
            //2.4
            bs.ScheduleBarberInfoDay(1,new DateTime(2021,2,11));
            Console.WriteLine(new string('-', 140));
            //2.5
            bs.ScheduleBarberInfoWeek(1, new DateTime(2021, 2, 11));
            Console.WriteLine(new string('-', 140));
            //2.9
            bs.ShowClients(bs.ClientsWithoutFeedbacks());
            Console.WriteLine(new string('-', 140));
            //2.10
            bs.ShowClients(bs.ClientNotVisitedMore1Year());
           
        }
    }
    class BarberShop
    {
        private DataClasses1DataContext context;
        public BarberShop()
        {
            context = new DataClasses1DataContext();
        }
        //   1.Вернуть информацию о барбере, который работает в бар-бершопе дольше всех
       public  Barbers OldestBarber()
        {
            return context.Barbers.OrderBy(b => b.HireDate).FirstOrDefault();
        }

        //  2.Вернуть информацию о барбере, который обслужил мак-симальное количество  клиентов в  указанном диапазонедат.
        //Даты передаются в качестве параметра
        public  Barbers MaxCountBarbers( DateTime startDate, DateTime endDate)
        {
            return context.Barbers.OrderByDescending(b => b.VisitArchive.Count(va=>va.Date>=startDate && va.Date<=endDate)).FirstOrDefault();
        }

        // 3.Вернуть информацию о клиенте, который посетил барбер-шоп максимальное количество раз
        public Clients MaxCountClientVisit()
        {
            return context.Clients.OrderByDescending(c => c.VisitArchive.Count()).FirstOrDefault();
        }

        //4.Вернуть информацию о клиенте, который потратил в бар-бершопе максимальное количество денег
        public Clients MaxTotalPriceClient()
        {
            return context.Clients.OrderByDescending(c => c.VisitArchive.Sum(v=>v.TotalPrice)).FirstOrDefault();
        }

        // 5.Вернуть информацию о самой длинной по времени услугев барбершопе
        public Services LongestServices()
        {
            return context.Services.OrderByDescending(s => s.Duration).FirstOrDefault();
        }

        //1. Вернуть информацию  о самом  популярном барбере(по количеству клиентов)
        public Barbers MostPopularBarber()
        {
            return context.Barbers.OrderByDescending(b => b.VisitArchive.Count()).FirstOrDefault();
        }

        //    2. Вернуть топ-3 барберов за месяц(по сумме денег, потра-ченной клиентами)
        public IEnumerable<Barbers> Top3TotalPriceClientForMonth()
        {
            return context.Barbers.Where(b=>b.VisitArchive.Count(va=> (DateTime.Now.Date-va.Date).TotalDays <= 30)>0).OrderByDescending(b => b.VisitArchive.Sum(va => va.TotalPrice)).Take(3);   
        }
        //    3. Вернуть топ-3 барберов за всё время(по средней оценке). Количество посещений клиентов не меньше 30
        public IEnumerable<Barbers> Top3AvgMarkClientvisitMore30()
        {
            return context.Barbers.Where(b => b.VisitArchive.Count()>=30).OrderByDescending(b => b.Feedbacks.Average(f => f.Mark));
        }
        //    4. Показать расписание  на день  конкретного барбера.Ин-формация о барбере и дне передаётся в качестве параметра
        public void  ScheduleBarberInfoDay(int BarberId,DateTime dateTime)
        {
            IEnumerable<Schedules> res = context.Schedules.Where(s => s.BarberId == BarberId && s.Date == dateTime);
            foreach (var item in res)
            {
                Console.WriteLine($"{item.Date.ToShortDateString()} {item.StartTime} {item.EndTime}");
            }
        }
        //   5.Показать свободные временные слоты на неделю конкрет-ного барбера.  Информация о  барбере и  дне передаётся  в качестве параметра
        public void ScheduleBarberInfoWeek(int BarberId, DateTime dateTime)
        {
            IEnumerable<Schedules> res = context.Schedules.Where(s => s.BarberId == BarberId && (s.Date-dateTime).TotalDays<=6);
            foreach (var item in res)
            {
                Console.WriteLine($"{item.Date.ToShortDateString()} {item.StartTime} {item.EndTime}");
            }
        }

        //    9.Вернуть информацию о клиентах, которые не поставили ни одного фидбека и ни одной оценки
        public IEnumerable<Clients> ClientsWithoutFeedbacks()
        {
            return context.Clients.Where(c => c.Feedbacks.Count() == 0);
        }
        // 10.Вернуть информацию  о клиентах, которые  не посещали  барбершоп свыше одного года
        public IEnumerable<Clients> ClientNotVisitedMore1Year()
        {
            return context.Clients.Where(c => c.VisitArchive.Count(va => (DateTime.Now-va.Date).TotalDays>=365 )>0);
        }
        public void ShowBarbers(IEnumerable<Barbers> barbers)
        {
            foreach (var barber in barbers)
            {
                Console.WriteLine("{0,-20}\t{1,-20}\t{2,-5}\t{3,-20}\t{4,-35}{5,10}\t{6,10}", barber.Name, barber.Surname, barber.Gender, barber.Phone, barber.Email, barber.BirthDate.ToShortDateString(), barber.HireDate.ToShortDateString());
            }
        }
        public void ShowBarber(Barbers barber)
        {
            Console.WriteLine("{0,-20}\t{1,-20}\t{2,-5}\t{3,-20}\t{4,-35}{5,10}\t{6,10}", barber.Name, barber.Surname, barber.Gender, barber.Phone, barber.Email, barber.BirthDate.ToShortDateString(), barber.HireDate.ToShortDateString());
        }
        public void ShowClients(IEnumerable<Clients> clients)
        {
            foreach (var client in clients)
            {
                Console.WriteLine("{0,-20}\t{1,-20}\t{2,-20}\t{3,-35}", client.Name, client.Surname, client.Phone, client.Email);
            }
        }
        public void ShowClient(Clients client)
        {
            Console.WriteLine("{0,-20}\t{1,-20}\t{2,-20}\t{3,-35}",client.Name,client.Surname,client.Phone,client.Email);
        }
        public void ShowService(Services service)
        {
            Console.WriteLine("{0,-10}\t{1,5}\t{2,10}",service.Name,service.Price,service.Duration);
        }
    }
}

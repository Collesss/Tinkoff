using ConsoleAppTestDBEnyity.Model;
using DBTinkoff;
using Microsoft.EntityFrameworkCore;
using System;

namespace ConsoleAppTestDBEnyity
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            using (DBTinkoffContext context1 = new DBTinkoffContext(new DbContextOptionsBuilder<DBTinkoffContext>().UseSqlite("Data source=test1.db").Options))
            {
                

            }
            */



            //return;

            using (MyDbContext context = new MyDbContext())
            {
                //Console.WriteLine("Users:");

                PrintUserInfo(context);

                //context.Users.RemoveRange(context.Users);
                //context.SaveChanges();

                /*
                foreach (User user in context.Users)
                    Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");
                */

                Console.WriteLine();

                User user1 = new User { Id = 1, Name = "test111", DateOfBirth = DateTime.Now };
                User user2 = new User { Id = 2, Name = "test222", DateOfBirth = DateTime.Now };

                //var ent1 = context.Users.Add(user1);
                //context.Users.Add(user2);

                context.Users.Remove(user1);
                context.Users.Remove(user2);

                //context.SaveChanges();

                PrintUserInfo(context);

                //user1.DateOfBirth = DateTime.Now;
                //user2.DateOfBirth = DateTime.Now;

                PrintUserInfo(context);

                context.SaveChanges();

                PrintUserInfo(context);

            }

            Console.WriteLine("\npress any key...");
            Console.ReadKey();
        }

        static void PrintUserInfo(MyDbContext context)
        {
            Console.WriteLine("Users:");
            foreach (User user in context.Users)
                Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");
        }
    }
}

using ConsoleAppTestDBEnyity.Model;
using System;

namespace ConsoleAppTestDBEnyity
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MyDbContext context = new MyDbContext())
            {
                Console.WriteLine("Users:");

                //context.Users.RemoveRange(context.Users);
                //context.SaveChanges();

                /*
                foreach (User user in context.Users)
                    Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");
                */

                Console.WriteLine();

                User user1 = new User { Name = "test111", DateOfBirth = DateTime.Now };
                User user2 = new User { Name = "test222", DateOfBirth = DateTime.Now };

                //var ent1 = context.Users.Add(user1);
                //context.Users.Add(user2);

                context.Users.Add(user1);
                context.Users.Add(user2);

                //context.SaveChanges();

                Console.WriteLine("Users:");
                foreach (User user in context.Users)
                    Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");

                //user1.DateOfBirth = DateTime.Now;
                //user2.DateOfBirth = DateTime.Now;

                Console.WriteLine("Users:");
                foreach (User user in context.Users)
                    Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");

                context.SaveChanges();

                Console.WriteLine("Users:");
                foreach (User user in context.Users)
                    Console.WriteLine($"{user.Id}; {user.Name}; {user.DateOfBirth};");

            }

            Console.WriteLine("\npress any key...");
            Console.ReadKey();
        }
    }
}

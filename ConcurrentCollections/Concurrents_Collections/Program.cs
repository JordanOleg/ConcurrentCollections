using Concurrent_Collections.PersonallyConcurrent;

PersonallyBag<int> bag = new PersonallyBag<int>();
//for (int i = 0; i < 10; i++)
//{
//    bool tryAdd = bag.TryAdd(i);
//    Console.WriteLine($"try_add#{i} = {tryAdd}");
//}
//for (int i = 0; i < 10; i++)
//{
//    bool tryAdd = bag.TryTake(out int item);
//    Console.WriteLine($"try_take#{item} = {tryAdd}");
//}
//Parallel.For(0, 10, (i) =>
//{
//    bool tryAdd = bag.TryAdd(i);
//    Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_add#{i} = {tryAdd}");
//});
//Task taskAdd = Task.Run(() =>
//{
//    for( int i = 0; i < 10; i++)
//    {
//        bool tryAdd = bag.TryAdd(i);
//        Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_add#{i} = {tryAdd}");
//    }
//});
//Task taskTake = Task.Run(() =>
//{
//    for(int i = 0; i < 10; i++)
//    {
//        bool tryTake = bag.TryTake(out int a);
//        Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_take#{a} = {tryTake}");
//    }
//});
//Parallel.For(0, 10, (i) =>
//{
//    bool tryTake = bag.TryTake(out int a);
//    Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_take#{a} = {tryTake}");
//});

Thread threadAdd1 = new Thread(MethodAdd);
Thread threadAdd2 = new Thread(MethodAdd);
threadAdd2.Start();
threadAdd1.Start();
Thread threadTake = new Thread(MethodTake);
//threadTake.Start();

Thread.Sleep(3000);
//MethodTake();


//await taskTake;
//Thread.Sleep(3000);
Console.WriteLine(bag.Count);
Console.WriteLine(bag.CountWorkSpaces);
Console.WriteLine(new string('-', 20));
foreach (int item in bag.Values())
{
    Console.WriteLine(item);
}
Console.ReadLine();


void MethodTake()
{
    for (int i = 0; i < 10; i++)
    {
        bool tryTake = bag.TryTake(out int a);
        Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_take#{a} = {tryTake}");
    }
}
void MethodAdd()
{
    for (int i = 0; i < 10; i++)
    {
        bool tryTake = bag.TryAdd(i);
        Console.WriteLine($"Thread *id %{Thread.CurrentThread.ManagedThreadId} : try_add#{i} = {tryTake}");
    }
}
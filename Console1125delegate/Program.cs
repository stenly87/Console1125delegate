using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;

namespace Console1125delegate
{
    delegate void MyFirstDelegate(string[] args);
    delegate void Operation(int x, int y);

    internal class Program
    {
        static int count = 0;
        static void Main(string[] args)
        {   // Делегаты и лямбда-выражения в C#
            // Делегат - тип объекта, который может ссылаться
            // на метод. Т.е. можно оперировать методом, как 
            // обычной переменной - передавать по ссылке
            // читать знания и вызывать на исполнение и тп
            // для создания делегата используется слово
            // delegatе. Создается делегат в пространстве 
            // имен или в классе
            // объявленный делегат представляет собой тип,
            // объект которого можно создать, и в этот объект
            // можно назначить ссылку на метод
            // пример создания ссылки на метод Main
            if (count > 10)
                return;

            MyFirstDelegate reference = new MyFirstDelegate(Main);

            Console.WriteLine(count++);
            // вызов делегата (а значит метода, на который он ссылается)
            //reference(null);

            // такое назначение невозможно, поскольку метод не соответствует делегату
            //reference = new MyFirstDelegate(Sum);
            // для того, чтобы делегат мог ссылать на метод, объявление
            // метода должно полностью соответствовать объявлению
            // делегата

            // один делегат может ссылаться на несколько методов
            // при вызове делегате на исполнение методы выполняются
            // в порядке назначения

            Operation operation = Sum; // назначение первого метода
            operation += Mult; // добавили второй метод

            operation(10, 10);

            operation -= Sum; // убираем первый метод

            operation(50, 50);

            Sample(15, 15, operation);

            int[] ints = new int[] { 0, 0, 0, 0, 1, 2, -3, 4, -5, 6, -7, 8, 9, 10 };

            int x = ints.First(TestIsNotZero);
            Console.WriteLine(x);

            // стандартные типы делегатов:
            //Action - делегат для ссылок на методы с типом void
            Action action;     // delegate void Some();
            Action<int> action1;// delegate void Some(int x);
            Action<int, int> action2;// delegate void Some(int x, int x);

            action2 = Sum;
            //Func - делегат для ссылок на методы с типом НЕ void
            // тип возвращаемого значения для Func указывается последним
            // в списке типов
            Func<bool> func;// delegate bool Some();
            Func<float, bool> func1;// delegate bool Some(float x);
            Func<int, bool> funcInt;
            //Predicate
            // вариация Func - возращает всегда bool, имеет один
            // аргумент
            Predicate<int> predicate = TestIsNotZero;

            funcInt = TestIsNotZero;
            int xByFunc = ints.First(funcInt);

            Func<int, int, int> funcInt2 = Sum2;
            funcInt2 += Mult2;

            // при вызове делегата с возвратом значения
            // результатом будет последний вызыванный метод
            // методы выполняются все
            int result = funcInt2(10, 10);
            Console.WriteLine(result);

            // первая попытка избавиться от мусорных методов 
            // в классе - анонимные методы
            func = delegate ()
            {
                return true;
            };

            bool y = func();

            // на смену анонимкам пришли лямда-выражения
            // синтаксис лямбд:
            //аргументы => тело

            // копия анонимного метода выше
            func = () => true;

            // еще один пример анонимного метода
            funcInt2 = delegate (int x, int y)
            {
                return x + y;
            };
            // тоже самое в лямбде
            // тип аргументов зависит от контекста
            // т.к. funcInt2 требует два аргумента типа int
            // то аргументы в лямбде автоматически имеют тип int
            funcInt2 = (x, y) => { return x + y; };
            // либо более короткий вариант
            funcInt2 = (x, y) => x + y;

            // вызов делегата с конкретными аргументами
            funcInt2(10, 10);

            // если делегат не ссылается на метод, а мы его
            // вызвали, то будет NullReferenceException
            // можно во время вызова добавить проверку на наличие
            // значения:
            var sum = funcInt2?.Invoke(10,10);
            // тоже самое:
            if (funcInt2 != null)
                sum = funcInt2(10, 10);
            else
                sum = null;
            // ? - означает nullable тип, т.е. тип, который помимо
            // своего обычного диапазона может иметь значение null
            funcInt2 = null;
            sum = funcInt2?.Invoke(10,10); // sum будет иметь значение null


            // если тело лямбды больше одной строки, мы не можем
            // убрать return и фигурные скобки
            funcInt2 = (x, y) =>
            {
                if (x > 0)
                    return x + y * 2;
                else
                    return x - y - 10;
            };
            int z = funcInt2(15, 15);
            // Польза от лямбд:
            // 1. они классно выглядят (функциональный стиль)
            // 2. они позволяют не засорять классы лишними методами
            // 3. они позволяют нам не придумывать лишние имена методам
            // 4. раз нет лишних методов в классе - класс становится проще

            var четныеЧисла = ints.Where(s => s % 2 == 0);
            if (четныеЧисла.Count() == 0)
                Console.WriteLine("Четных чисел нет");

            // кол-во положительных, отрицательных и нулевых элементов
            int positive =  ints.Count(s => s > 0);
            int negative =  ints.Count(s => s < 0);
            int zero = ints.Count(s => s == 0);
        }

        static bool TestIsNotZero(int arg)
        {
            return arg > 5;
        }

        static void Sample(int x, int y, Operation operation)
        {
            Console.WriteLine("Тут происходят личные дела метода Sample");
            operation(x, y);
        }

        static void Sum(int x, int y)
        {
            Console.WriteLine(x + y);
        }

        static void Mult(int x, int y)
        {
            Console.WriteLine(x * y);
        }

        static int Sum2(int x, int y)
        {
            return x + y;
        }

        static int Mult2(int x, int y)
        {
            return x * y;
        }
    }
}
